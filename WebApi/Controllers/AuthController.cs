using Business.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authservice;

        public AuthController(IAuthService authservice)
        {
            _authservice = authservice;
        }
        [HttpPost("Register")]
        public IActionResult Register(UserAndCompanyRegisterDto userAndCompanyRegisterDto)
        {
            var userExists = _authservice.UserExits(userAndCompanyRegisterDto.userForRegister.Email);
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }
            var companyExists = _authservice.CompanyExists(userAndCompanyRegisterDto.company);
            if (!companyExists.Success)
            {
                return BadRequest(userExists.Message);
            }
            //Direk Kullanıcıyı Versin.
            var registerResult = _authservice.Register(userAndCompanyRegisterDto.userForRegister, userAndCompanyRegisterDto.userForRegister.Password, userAndCompanyRegisterDto.company);

            //if (registerResult.Success)
            //{
            //    return Ok(registerResult);
            //}
            //Token Vermesi için.
            var result = _authservice.CreateAccessToken(registerResult.Data,registerResult.Data.CompanyId);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(registerResult.Message);
        }
        [HttpPost("RegisterSecondAccount")]
        public IActionResult RegisterSecondAccount(UserForRegisterToSecondAccountDto userForRegisterDto)
        {
            var userExists = _authservice.UserExits(userForRegisterDto.Email);
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }
            //Direk Kullanıcıyı Versin.
            var registerResult = _authservice.RegisterSecondAccount(userForRegisterDto,userForRegisterDto.Password,userForRegisterDto.CompanyId);
            //if (registerResult.Success)
            //{
            //    return Ok(registerResult);
            //}
            //Token Vermesi için.
            var result = _authservice.CreateAccessToken(registerResult.Data,userForRegisterDto.CompanyId);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(registerResult.Message);
        }
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            var userToLogin = _authservice.Login(userForLogin);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }
            if(userToLogin.Data.IsActive)
            {
                var userCompany = _authservice.GetCompany(userToLogin.Data.Id).Data;
            var result = _authservice.CreateAccessToken(userToLogin.Data, userCompany.CompanyId);
            if (result.Success)
            {
                return Ok(result.Data);
            }
                return BadRequest(result.Message);
            }
            return BadRequest("Kullanıcı Pasif Durumda Aktif Etmek İçin Yöneticiye Danışın");   
            

        }
        [HttpGet("comfirmuser")]
        public IActionResult ConfirmUser(string value)
        {
            var user = _authservice.GetByMailConfirmValue(value).Data;
            user.MailConfirm = true;
            user.MailConfirmDate = DateTime.Now;
           var result= _authservice.Update(user);
            if (result.Success)
            {
                return Ok(result);

            }
            return BadRequest(result.Message);
           
        }
        [HttpGet("sendconfirmemail")]
        public IActionResult SendConfirmEmail(int userId)
        {
            var user=_authservice.GetById(userId).Data;
            var result = _authservice.SendComfirmEmail(user);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);


        }
    }
}
