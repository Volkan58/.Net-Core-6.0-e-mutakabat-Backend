using Business.Abstract;
using Business.Constans;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CurrencyAccountManager : ICurrencyAccountService
    {
        private readonly ICurrencyAccountDal _currencyAccount;

        public CurrencyAccountManager(ICurrencyAccountDal currencyAccount)
        {
            _currencyAccount = currencyAccount;
        }

        [CacheRemoveAspect("ICurrencyAccountService.Get")]
        [ValidationAspect(typeof(CurrencyAccountValidator))]
       // [PerformanceAspect(2)]
        public IResult Add(CurrencyAccount currencyAccount)
        {
            _currencyAccount.Add(currencyAccount);
            return new SuccessResult(Message.AddedCurrencyAccount);
        }
        [CacheRemoveAspect("ICurrencyAccountService.Get")]
        [ValidationAspect(typeof(CurrencyAccountValidator))]
        [TransactionScopeAspect]
        public IResult AddToExcel(string filePath, int companyId)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        string code = reader.GetString(0);
                        string name = reader.GetString(1);
                        string adress = reader.GetString(2);
                        string taxDepartment = reader.GetString(3);
                        string taxIdNumber = reader.GetString(4);
                        string IdentityNumber = reader.GetString(5);
                        string email = reader.GetString(6);
                        string authorized = reader.GetString(7);
                        if (code != "Cari Kodu")
                        {
                            CurrencyAccount currencyAccount = new CurrencyAccount()
                            {
                                Code = code,
                                Name = name,
                                Adress = adress,
                                TaxDepartment = taxDepartment,
                                TaxIdNumber = taxIdNumber,
                                AddedAt = DateTime.Now,
                                Authorized = authorized,
                                Email = email,
                                IdentityNumber = IdentityNumber,
                                IsActive = true,
                                CompanyId = companyId

                            };
                            _currencyAccount.Add(currencyAccount);
                        }
                    }
                }
            }
            File.Delete(filePath);
            return new SuccessResult(Message.AddedCurrencyAccount);
        }
        [CacheRemoveAspect("ICurrencyAccountService.Get")]
        public IResult Delete(CurrencyAccount currencyAccount)
        {
            _currencyAccount.Delete(currencyAccount);
            return new SuccessResult(Message.DeletedCurrencyAccount);
        }

        [CacheAspect(60)]
        public IDataResult<CurrencyAccount> Get(int Id)
        {
            return new SuccessDataResult<CurrencyAccount>(_currencyAccount.Get(p => p.Id == Id));
        }

        [CacheAspect(60)]
        public IDataResult<CurrencyAccount> GetByCode(string code, int companyId)
        {
            return new SuccessDataResult<CurrencyAccount>(_currencyAccount.Get(p => p.CompanyId == companyId));
        }
        [CacheAspect(60)]
        public IDataResult<List<CurrencyAccount>> GetList(int companyId)
        {
            return new SuccessDataResult<List<CurrencyAccount>>(_currencyAccount.GetList(p => p.CompanyId == companyId));
        }
        [CacheRemoveAspect("ICurrencyAccountService.Get")]
        [ValidationAspect(typeof(CurrencyAccountValidator))]
        public IResult Update(CurrencyAccount currencyAccount)
        {
            _currencyAccount.Update(currencyAccount);
            return new SuccessResult(Message.UpdatedCurrencyAccount);
        }
    }
}
