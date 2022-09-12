using Business.Abstract;
using Business.Constans;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Transaction;
using Core.CrossCuttingConcerns.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;
        private readonly ICompanyService _companyService;
        private readonly IMailParameterService _mailParameterService;
        private readonly IMailService _mailService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IUserOperationClaimService _userOperationClaimService;
        private readonly IOperationClaimService _operationClaimService;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper, ICompanyService companyService, IMailParameterService mailParameterService, IMailService mailService, IMailTemplateService mailTemplateService,IUserOperationClaimService userOperationClaimService, IOperationClaimService operationClaimService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _companyService = companyService;
            _mailParameterService = mailParameterService;
            _mailService = mailService;
            _mailTemplateService = mailTemplateService;
            _userOperationClaimService = userOperationClaimService;
            _operationClaimService = operationClaimService;
        }



        public IResult CompanyExists(Company company)
        {
            var result = _companyService.CompanyExists(company);
            if (result.Success == false)
            {
                return new ErrorResult(Message.CompanyAlreadyExists);
            }
            return new SuccessResult();
        }

        public IDataResult<AccessToken> CreateAccessToken(User user, int companyId)
        {
            var claims = _userService.GetClaims(user, companyId);
            var accesstoken = _tokenHelper.CreateToken(user, claims, companyId);
            return new SuccessDataResult<AccessToken>(accesstoken);
        }

        public IDataResult<User> GetById(int Id)
        {
            return new SuccessDataResult<User>(_userService.GetById(Id));
        }

        public IDataResult<User> GetByMailConfirmValue(string value)
        {
            return new SuccessDataResult<User>(_userService.GetByMailConfirmValue(value));
        }

        public IDataResult<User> Login(UserForLoginDto userForLogin)
        {
            var userToCheck = _userService.GetByMail(userForLogin.Email);
            if (userToCheck == null)
            {
                return new ErrorDataResult<User>(Message.UserNotFound);
            }
            if (!HashingHelper.VerifyPasswordHash(userForLogin.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>(Message.PasswordError);
            }
            return new SuccessDataResult<User>(userToCheck, Message.SuccessFullLogin);
        }

        [TransactionScopeAspect]
        public IDataResult<UserCompanyDto> Register(UserForRegisterDto userForRegister, string password, Company company)
        {
      
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User()
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                IsActive = true,
                MailConfirm = false,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = Guid.NewGuid().ToString(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Name = userForRegister.Name,
            };
          
            
            //ValidationTool.Validate(new UserValidator(),user);
            //ValidationTool.Validate(new CompanyValidator(),company);
            _userService.Add(user);
            _companyService.Add(company);
            _companyService.UserCompanyAdd(user.Id, company.Id);

            UserCompanyDto userCompanyDto = new UserCompanyDto()
            {
                Id = user.Id,
                Name = user.Name,
                AddedAt = DateTime.Now,
                IsActive = true,
                CompanyId = company.Id,
                Email = user.Email,
                MailConfirm = user.MailConfirm,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = user.MailConfirmValue,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
            };

            SendConfirmEmail(user);
            return new SuccessDataResult<UserCompanyDto>(userCompanyDto, Message.UserRegistered);
        }
        void SendConfirmEmail(User user)
        {
            string subject = "Kullanıcı kayıt onay maili";
            string body = "Kullanıcı Sisteme Kayıt Oldu Kaydınızı Tamamlamak İçin Linke Tıklayınız.";
            string link = "https://localhost:7023/api/Auth/confirmuser?value=" + user.MailConfirmValue;
            string linkdescription = "Kaydı onaylamak için tıklayın";
            var mailTemplate = _mailTemplateService.GetByTemplateName(4, "Deneme");
            string templateBody = mailTemplate.Data.Value;
            templateBody = templateBody.Replace("{{title}}", subject);
            templateBody = templateBody.Replace("{{message}}", body);
            templateBody = templateBody.Replace("{{link}}", link);
            templateBody = templateBody.Replace("{{linkDescription}}", linkdescription);

            //Mail Servisi Çalıştırma.
            var mailParameter = _mailParameterService.Get(4);
            SendMailDto sendMailDto = new SendMailDto()
            {
                mailParameter = mailParameter.Data,
                email = user.Email,
                subject = "Kullanıcı kayıt onay maili",
                body = templateBody

            };
            _mailService.SendMail(sendMailDto);
            user.MailConfirmDate = DateTime.Now;
            _userService.Update(user);
        }

        public IDataResult<User> RegisterSecondAccount(UserForRegisterDto userForRegister, string Password,int companyId)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(Password, out passwordHash, out passwordSalt);
            var user = new User()
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                IsActive = true,
                MailConfirm = false,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = Guid.NewGuid().ToString(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Name = userForRegister.Name,
            };
            _userService.Add(user);
            _companyService.UserCompanyAdd(user.Id, companyId);
            SendComfirmEmail(user);

            return new SuccessDataResult<User>(user, Message.UserRegistered);
        }

        public IResult Update(User user)
        {
            _userService.Update(user);
            return new SuccessResult(Message.UserMailConfirmUpdate);
        }

        public IResult UserExits(string email)
        {
            var result = _userService.GetByMail(email);
            if(result != null)
            {
                return new ErrorResult(Message.UserAlreadyExits);
            }
            return new SuccessResult();
        }

        public IResult SendComfirmEmail(User user)
        {
            if(user.MailConfirm==true)
            {
                return new ErrorResult(Message.MailAlreadyConfirm);
            }
            DateTime ConfirmMailDate = user.MailConfirmDate;
            DateTime now = DateTime.Now;
            if(ConfirmMailDate.ToShortDateString()==now.ToShortDateString())
            {
                if(ConfirmMailDate.Hour==now.Hour && ConfirmMailDate.AddMinutes(5).Minute <=now.Minute)
                {
                    SendComfirmEmail(user);
                    return new SuccessResult(Message.MailConfirmSendSuccessful);
                }
                else
                {
                    return new ErrorResult(Message.MailConfirmTimeHasNotExpired);
                }
            }
            SendComfirmEmail(user);
            return new SuccessResult(Message.MailConfirmSendSuccessful);
        }

        public IDataResult<UserCompany> GetCompany(int userId)
        {
            return new SuccessDataResult<UserCompany>(_companyService.GetCompany(userId).Data);
        }

        public IDataResult<User> GetByEmail(string email)
        {
            return new SuccessDataResult<User>(_userService.GetByMail(email));
        }
    }
}
