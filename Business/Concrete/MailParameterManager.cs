using Business.Abstract;
using Business.Constans;
using Core.Aspects.Caching;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class MailParameterManager: IMailParameterService
    {
        private readonly IMailParameterDal _mailParameter;

        public MailParameterManager(IMailParameterDal mailParameter)
        {
            _mailParameter = mailParameter;
        }

        [CacheAspect(60)]
        public IDataResult<MailParameter> Get(int companyId)
        {
            return new SuccessDataResult<MailParameter>(_mailParameter.Get(m => m.CompanyId == companyId));
        }
        [CacheRemoveAspect("IMailParameterService.Get")]
        public IResult Update(MailParameter mailParameter)
        {
            var result = Get(mailParameter.CompanyId);
            if (result.Data == null)
            {
                _mailParameter.Add(mailParameter);
            }
            else
            {
                result.Data.SMTP = mailParameter.SMTP;
                result.Data.Port = mailParameter.Port;
                result.Data.SSL=mailParameter.SSL;
                result.Data.Email= mailParameter.Email;
                result.Data.Password= mailParameter.Password;
                _mailParameter.Update(result.Data);
            }
            return new SuccessResult(Message.MailParameterUpdated);
        }
    }
}
