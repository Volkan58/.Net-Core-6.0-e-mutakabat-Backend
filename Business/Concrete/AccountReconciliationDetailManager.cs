﻿using Business.Abstract;
using Business.BussinessAspect;
using Business.Constans;
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
    public class AccountReconciliationDetailManager : IAccountReconciliationDetailService
    {
        private readonly IAccountReconciliationDetailDal _acountReconciliationDetail;

        public AccountReconciliationDetailManager(IAccountReconciliationDetailDal acountReconciliationDetail)
        {
            _acountReconciliationDetail = acountReconciliationDetail;
        }
        [CacheRemoveAspect("AccountReconciliationDetail.Get")]
        [PerformanceAspect(3)]
        [SecuredOperation("AccountReconciliationDetail.Add")]
        public IResult Add(AccountReconciliationDetail accountReconciliationDetail)
        {
            _acountReconciliationDetail.Add(accountReconciliationDetail);
            return new SuccessResult(Message.AddedAccountReconciliationDetail);
        }
        [CacheRemoveAspect("AccountReconciliationDetail.Get")]
        [SecuredOperation("AccountReconciliationDetail.Add")]
        public IResult AddToExcel(string filePath, int accountReconciliationId)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        string description = reader.GetString(1);

                        if (description != "Açıklama" && description != null)
                        {
                            DateTime date = reader.GetDateTime(0);
                            double currencyId = reader.GetDouble(2);
                            double debit = reader.GetDouble(3);
                            double credit = reader.GetDouble(4);

                            AccountReconciliationDetail accountReconciliationDetail = new AccountReconciliationDetail()
                            {
                                AccountReconciliationId = accountReconciliationId,
                                Description = description,
                                Date = date,
                                CurrencyCredit = Convert.ToDecimal(credit),
                                CurrencyDebit = Convert.ToDecimal(debit),
                                CurrencyId = Convert.ToInt16(currencyId)

                            };
                            _acountReconciliationDetail.Add(accountReconciliationDetail);


                            
                        }
                    }
                }
            }

            File.Delete(filePath);

            return new SuccessResult(Message.AddedAccountReconciliation);
        }

        [CacheRemoveAspect("AccountReconciliationDetail.Get")]
        public IResult Delete(AccountReconciliationDetail accountReconciliationDetail)
        {
            _acountReconciliationDetail.Delete(accountReconciliationDetail);
            return new SuccessResult(Message.DeletedAccountReconciliationDetail);
        }
        [CacheAspect(60)]
        public IDataResult<AccountReconciliationDetail> GetById(int Id)
        {
            return new SuccessDataResult<AccountReconciliationDetail>(_acountReconciliationDetail.Get(p => p.Id == Id));
        }

        public IDataResult<List<AccountReconciliationDetail>> GetList(int accountreconciliationId)
        {
            return new SuccessDataResult<List<AccountReconciliationDetail>>(_acountReconciliationDetail.GetList(p => p.AccountReconciliationId == accountreconciliationId));
        }
        [CacheRemoveAspect("AccountReconciliationDetail.Get")]
        public IResult Update(AccountReconciliationDetail accountReconciliationDetail)
        {
            _acountReconciliationDetail.Update(accountReconciliationDetail);
            return new SuccessResult(Message.UpdatedAccountReconciliationDetail);
        }
    }
}
