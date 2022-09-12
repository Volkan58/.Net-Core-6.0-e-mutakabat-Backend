using Business.Abstract;
using Business.BussinessAspect;
using Business.Constans;
using Core.Entities.Concrete;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserOperationClaimManager : IUserOperationClaimService
    {
        private readonly IUserOperationClaimDal _userOperationClaimDal;

        public UserOperationClaimManager(IUserOperationClaimDal userOperationClaimDal)
        {
            _userOperationClaimDal = userOperationClaimDal;
        }

        [SecuredOperation("Admin,UserOperationClaim.Add")]
        public IResult Add(UserOperationClaim userOperationClaim)
        {
            _userOperationClaimDal.Add(userOperationClaim);
            return new SuccessResult(Message.AddedUserOperationClaim);
        }
        [SecuredOperation("Admin")]
        public IResult Delete(UserOperationClaim userOperationClaim)
        {
            _userOperationClaimDal.Delete(userOperationClaim);
            return new SuccessResult(Message.DeletedUserOperationClaim);
        }
        [SecuredOperation("Admin")]
        public IDataResult<UserOperationClaim> GetById(int Id)
        {
            return new SuccessDataResult<UserOperationClaim>(_userOperationClaimDal.Get(p => p.Id == Id));
        }
        [SecuredOperation("Admin,UserOperationClaim.GetList")]
        public IDataResult<List<UserOperationClaim>> GetList(int userId,int companyId)
        {
            return new SuccessDataResult<List<UserOperationClaim>>(_userOperationClaimDal.GetList(p => p.UserId == userId && p.CompanyId == companyId));
        }
        [SecuredOperation("Admin")]
        public IResult Update(UserOperationClaim userOperationClaim)
        {
            _userOperationClaimDal.Update(userOperationClaim);
            return new SuccessResult(Message.UpdatedUserOperationClaim);
        }
    }
}
