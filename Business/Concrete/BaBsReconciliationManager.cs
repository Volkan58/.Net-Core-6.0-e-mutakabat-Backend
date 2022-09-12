using Business.Abstract;
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
    public class BaBsReconciliationManager : IBaBsReconciliationService
    {
        private readonly IBaBsReconciliationDal _baBsReconciliation;
        private readonly ICurrencyAccountService _currencyAccountService;

        public BaBsReconciliationManager(IBaBsReconciliationDal baBsReconciliation, ICurrencyAccountService currencyAccountService)
        {
            _baBsReconciliation = baBsReconciliation;
            _currencyAccountService = currencyAccountService;
        }
        [CacheRemoveAspect("IBaBsReconciliationService.Get")]
        public IResult Add(BaBsReconciliation baBsReconciliation)
        {
            _baBsReconciliation.Add(baBsReconciliation);
            return new SuccessResult(Message.AddedBaBsReconciliation);
        }
        [CacheRemoveAspect("IBaBsReconciliationService.Get")]
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
                            string type = reader.GetString(1);
                            double mounth = reader.GetDouble(2);
                            double year = reader.GetDouble(3);
                            double quantit = reader.GetDouble(4);
                            double total = reader.GetDouble(5);

                            int currencyAccountId = _currencyAccountService.GetByCode(code, companyId).Data.Id;


                            BaBsReconciliation baBsReconciliation = new BaBsReconciliation()
                            {
                                CompanyId = companyId,
                                CurrencyAccountId = currencyAccountId,
                                Type = type,
                                Mounth = Convert.ToInt16(mounth),
                                Year = Convert.ToInt16(year),
                                Quantity = Convert.ToInt16(quantit),
                                Total = Convert.ToDecimal(total)

                            };

                            _baBsReconciliation.Add(baBsReconciliation);
                        }
                    }
                }
            }
            File.Delete(filePath);

            return new SuccessResult(Message.AddedBaBsReconciliation);
        }
        [CacheRemoveAspect("IBaBsReconciliationService.Get")]
        public IResult Delete(BaBsReconciliation baBsReconciliation)
        {
            _baBsReconciliation.Delete(baBsReconciliation);
            return new SuccessResult(Message.DeletedBaBsReconciliation);
        }
        [CacheAspect(60)]
        public IDataResult<BaBsReconciliation> GetById(int Id)
        {
            return new SuccessDataResult<BaBsReconciliation>(_baBsReconciliation.Get(p => p.Id == Id));
        }

        public IDataResult<List<BaBsReconciliation>> GetList(int companyId)
        {
            return new SuccessDataResult<List<BaBsReconciliation>>(_baBsReconciliation.GetList(p => p.CompanyId == companyId));
        }
        [CacheRemoveAspect("IBaBsReconciliationService.Get")]
        public IResult Update(BaBsReconciliation baBsReconciliation)
        {
            _baBsReconciliation.Update(baBsReconciliation);
            return new SuccessResult(Message.UpdatedBaBsReconciliation);
        }
    }
}
