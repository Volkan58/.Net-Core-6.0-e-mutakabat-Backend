using Business.Abstract;
using Business.BussinessAspect;
using Business.Constans;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Caching;
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
    public class AccountReconciliationManager : IAccountReconciliationService
    {
        private readonly IAccountReconciliationDal _accountReconciliation;
        private readonly ICurrencyAccountService _currencyAccountService;

        public AccountReconciliationManager(IAccountReconciliationDal accountReconciliation, ICurrencyAccountService currencyAccountService)
        {
            _accountReconciliation = accountReconciliation;
            _currencyAccountService = currencyAccountService;
        }
        [SecuredOperation("AccountReconciliation.Add")]
        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        public IResult Add(AccountReconciliation accountReconciliation)
        {
            _accountReconciliation.Add(accountReconciliation);
            return new SuccessResult(Message.AddedAccountReconciliation);
        }
        [CacheRemoveAspect("IAccountReconciliationService.Get")]
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

                        if (code != "Cari Kodu" && code != null)
                        {
                            DateTime startingDate = reader.GetDateTime(1);
                            DateTime endingDate = reader.GetDateTime(2);
                            double currencyId = reader.GetDouble(3);
                            double debit = reader.GetDouble(4);
                            double credit = reader.GetDouble(5);

                            int currencyAccountId = _currencyAccountService.GetByCode(code, companyId).Data.Id;
                        

                            AccountReconciliation accountReconciliation = new AccountReconciliation()
                            {
                                CompanyId = companyId,
                                CurrencyAccountId = currencyAccountId,
                                CurrencyCredit = Convert.ToDecimal(credit),
                                CurencyDebit = Convert.ToDecimal(debit),
                                CurrencyId = Convert.ToInt16(currencyId),
                                StartingDate = startingDate,
                                EndingDate = endingDate,
                                
                               
                            };

                            _accountReconciliation.Add(accountReconciliation);
                        }
                    }
                }
            }

            File.Delete(filePath);

            return new SuccessResult(Message.AddedAccountReconciliation);
        }


        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        public IResult Delete(AccountReconciliation accountReconciliation)
        {
            _accountReconciliation.Delete(accountReconciliation);
            return new SuccessResult(Message.DeletedAccountReconciliation);
        }



        public IDataResult<AccountReconciliation> GetById(int Id)
        {
            return new SuccessDataResult<AccountReconciliation>(_accountReconciliation.Get(p => p.Id == Id));
        }
        [CacheAspect(60)]
        public IDataResult<List<AccountReconciliation>> GetList(int companyId)
        {
            return new SuccessDataResult<List<AccountReconciliation>>(_accountReconciliation.GetList(p => p.CompanyId == companyId));
        }
        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        public IResult Update(AccountReconciliation accountReconciliation)
        {
            _accountReconciliation.Update(accountReconciliation);
            return new SuccessResult(Message.UpdatedAccountReconciliation);
        }
    }
}
