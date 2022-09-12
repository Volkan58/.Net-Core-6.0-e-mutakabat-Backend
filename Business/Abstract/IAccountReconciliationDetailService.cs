using Core.Utilities.Result.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAccountReconciliationDetailService
    {
        IResult Add(AccountReconciliationDetail accountReconciliationDetail);
        IResult AddToExcel(string filePath, int accountreconciliationId);
        IResult Update(AccountReconciliationDetail accountReconciliationDetail);
        IResult Delete(AccountReconciliationDetail accountReconciliationDetail);
        IDataResult<AccountReconciliationDetail> GetById(int Id);

        IDataResult<List<AccountReconciliationDetail>> GetList(int accountreconciliationId);
    }
}
