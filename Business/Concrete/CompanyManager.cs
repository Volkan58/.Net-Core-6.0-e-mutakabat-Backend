using Business.Abstract;
using Business.Constans;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Caching;
using Core.Entities.Concrete;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CompanyManager : ICompanyService
    {
        private readonly ICompanyDal _companydal;

        public CompanyManager(ICompanyDal companydal)
        {
            _companydal = companydal;
        }
        [CacheRemoveAspect("ICompanyService.Get")]
        [ValidationAspect(typeof(CompanyValidator))]
        public IResult Add(Company company)
        {
           _companydal.Add(company);
            return new SuccessResult(Message.AddedCompany);
        }
        [ValidationAspect(typeof(CompanyValidator))]
        [TransactionScopeAspect]
        public IResult AddCompanyAndUserCompany(CompanyDto companyDto)
        {
            _companydal.Add(companyDto.company);
            _companydal.UserCompanyAdd(companyDto.UserId, companyDto.company.Id);
            return new SuccessResult(Message.AddedCompany);
        }

        public IResult CompanyExists(Company company)
        {
            var result = _companydal.Get(c => c.Name == company.Name && c.TaxDepartment == company.TaxDepartment && c.TaxIdNumber == company.TaxIdNumber && c.IdentityNumber==company.IdentityNumber);
            if (result != null)
            {
                return new ErrorResult(Message.CompanyAlreadyExists);
            }
            return new SuccessResult();

        }
        [CacheAspect(60)]
        public IDataResult<Company> GetById(int Id)
        {
            return new SuccessDataResult<Company>(_companydal.Get(p => p.Id == Id));
        }
        [CacheAspect(60)]
        public IDataResult<UserCompany> GetCompany(int userId)
        {
            return new SuccessDataResult<UserCompany>(_companydal.GetCompany(userId));
        }
        [CacheAspect(60)]
        public IDataResult<List<Company>> GetList()
        {
            return new SuccessDataResult<List<Company>>(_companydal.GetList(),"Listeleme İşlemi Başarılı");
        }
        [CacheRemoveAspect("ICompanyService.Get")]
        public IResult Update(Company company)
        {
            _companydal.Update(company);
            return new SuccessResult(Message.UpdatedCompany);
        }
        [CacheRemoveAspect("ICompanyService.Get")]
        public IResult UserCompanyAdd(int userId, int companyId)
        {
            _companydal.UserCompanyAdd(userId, companyId);
            return new SuccessResult();
        }
    }
}
