using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailTemplatesController : ControllerBase
    {
        private readonly IMailTemplateService _mailtemplateService;

        public MailTemplatesController(IMailTemplateService mailtemplateService)
        {
            _mailtemplateService = mailtemplateService;
        }


        [HttpPost]

        public IActionResult Add(MailTemplate mailTemplate)
        {
            var result = _mailtemplateService.Add(mailTemplate);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
