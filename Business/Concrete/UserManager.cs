using Business.Abstract;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Caching;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userdal;

        public UserManager(IUserDal userdal)
        {
            _userdal = userdal;
        }
        [CacheRemoveAspect("IUserService.Get")]
        [ValidationAspect(typeof(UserValidator))]
        public void Add(User user)
        {
          
            _userdal.Add(user);
        }
        [CacheAspect(60)]
        public User GetById(int Id)
        {
            return _userdal.Get(m=>m.Id==Id);
        }
        [CacheAspect(60)]
        public User GetByMail(string email)
        {
            return _userdal.Get(p => p.Email == email);
        }
       
        public User GetByMailConfirmValue(string Value)
        {
            return _userdal.Get(m => m.MailConfirmValue == Value);
        }

        public List<OperationClaim> GetClaims(User user, int companyId)
        {
            return _userdal.GetClaims(user, companyId);
        }
        [CacheRemoveAspect("IUserService.Get")]
        public void Update(User user)
        {
             _userdal.Update(user);
        }
    }
}
