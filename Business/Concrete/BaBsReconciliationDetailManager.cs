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
    public class BaBsReconciliationDetailManager : IBaBsReconciliationDetailService
    {
        private readonly IBaBsReconciliationDetailDal _baBsReconciliationDetail;

        public BaBsReconciliationDetailManager(IBaBsReconciliationDetailDal baBsReconciliationDetail)
        {
            _baBsReconciliationDetail = baBsReconciliationDetail;
        }
        [CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
        public IResult Add(BaBsReconciliationDetail baBsReconciliationDetail)
        {
            _baBsReconciliationDetail.Add(baBsReconciliationDetail);
            return new SuccessResult(Message.AddedBaBsReconciliationDetail);
        }
        [CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
        [TransactionScopeAspect]
        public IResult AddToExcel(string filePath, int baBsReconciliationId)
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
                            double amount = reader.GetDouble(2);
                            BaBsReconciliationDetail baBsReconciliationDetail = new BaBsReconciliationDetail()
                            {
                                BaBsReconciliationId = baBsReconciliationId,
                                Date = date,
                                Description = description,
                                Amount = Convert.ToDecimal(amount)
                            };
                            _baBsReconciliationDetail.Add(baBsReconciliationDetail);
                        }


                    }
                }
            }
            File.Delete(filePath);
            return new SuccessResult(Message.AddedBaBsReconciliation);
        }
        [CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
        public IResult Delete(BaBsReconciliationDetail baBsReconciliationDetail)
        {
            _baBsReconciliationDetail.Delete(baBsReconciliationDetail);
            return new SuccessResult(Message.DeletedBaBsReconciliationDetail);
        }
        [CacheAspect(60)]
        public IDataResult<BaBsReconciliationDetail> GetById(int Id)
        {
            return new SuccessDataResult<BaBsReconciliationDetail>(_baBsReconciliationDetail.Get(p => p.Id == Id));
        }

        public IDataResult<List<BaBsReconciliationDetail>> GetList(int baBsReconciliationId)
        {
            return new SuccessDataResult<List<BaBsReconciliationDetail>>(_baBsReconciliationDetail.GetList(p => p.BaBsReconciliationId == baBsReconciliationId));
        }
        [CacheRemoveAspect("IBaBsReconciliationDetailService.Get")]
        public IResult Update(BaBsReconciliationDetail baBsReconciliationDetail)
        {
            _baBsReconciliationDetail.Update(baBsReconciliationDetail);
            return new SuccessResult(Message.UpdatedBaBsReconciliationDetail);
        }
    }
}
