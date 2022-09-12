using Core.Entities.Concrete;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public  interface IAuthService
    {
        IDataResult<UserCompanyDto> Register(UserForRegisterDto userForRegister, string Password, Company company);
        IDataResult<User> RegisterSecondAccount(UserForRegisterDto userForRegister, string Password,int companyId);
        IDataResult<User> Login(UserForLoginDto userForLogin);
        IDataResult<User> GetByMailConfirmValue(string value);
        IDataResult<User> GetById(int Id);
        IDataResult<UserCompany> GetCompany(int userId);
        IDataResult<User> GetByEmail(string email);
        IResult UserExits(string email);
        IResult Update(User user);
        IResult CompanyExists(Company company);
        IResult SendComfirmEmail(User user);
        IDataResult<AccessToken> CreateAccessToken(User user,int companyId);
       
       
      
      
       
    
    
        
     
       
    
        
       
    }
}
