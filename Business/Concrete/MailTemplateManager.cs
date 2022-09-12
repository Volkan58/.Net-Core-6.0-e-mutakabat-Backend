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
    public class MailTemplateManager : IMailTemplateService
    {
        private readonly IMailTemplateDal _mailtemplatedal;

        public MailTemplateManager(IMailTemplateDal mailtemplatedal)
        {
            _mailtemplatedal = mailtemplatedal;
        }
        [CacheRemoveAspect("IMailTemplateService.Get")]
        public IResult Add(MailTemplate mailTemplate)
        {
            _mailtemplatedal.Add(mailTemplate);
            return new SuccessResult(Message.MailTemplateAdded);
        }
        [CacheRemoveAspect("IMailTemplateService.Get")]
        public IResult Delete(MailTemplate mailTemplate)
        {
            _mailtemplatedal.Delete(mailTemplate);
            return new SuccessResult(Message.MailTemplateDeleted);
        }
        [CacheAspect(60)]
        public IDataResult<MailTemplate> Get(int id)
        {
            return new SuccessDataResult<MailTemplate>(_mailtemplatedal.Get(m => m.Id == id));
        }
        [CacheAspect(60)]
        public IDataResult<List<MailTemplate>> GetAll(int companyId)
        {
            return new SuccessDataResult<List<MailTemplate>>(_mailtemplatedal.GetList(m => m.CompanyId == companyId));
        }

        public IDataResult<MailTemplate> GetByTemplateName(int companyId, string Name)
        {
            var result = _mailtemplatedal.Get(m => m.Type == Name && m.CompanyId == companyId);
            return new SuccessDataResult<MailTemplate>(result);
        }
        [CacheRemoveAspect("IMailTemplateService.Get")]
        public IResult Update(MailTemplate mailTemplate)
        {
            _mailtemplatedal.Update(mailTemplate);
            return new SuccessResult(Message.MailTemplateUpdated);
        }
    }
}
