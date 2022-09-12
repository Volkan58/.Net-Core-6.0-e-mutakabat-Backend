using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountReconciliationDetailController : ControllerBase
    {
        private readonly IAccountReconciliationDetailService _accountReconciliationDetailService;

        public AccountReconciliationDetailController(IAccountReconciliationDetailService accountReconciliationDetailService)
        {
            _accountReconciliationDetailService = accountReconciliationDetailService;
        }

        [HttpPost("addDetail")]
        public IActionResult Add(AccountReconciliationDetail accountReconciliationDetail)
        {
           var result= _accountReconciliationDetailService.Add(accountReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);

        }
        [HttpPost("Update")]
        public IActionResult Update(AccountReconciliationDetail accountReconciliationDetail)
        {
            var result = _accountReconciliationDetailService.Update(accountReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("Delete")]
        public IActionResult Delete(AccountReconciliationDetail accountReconciliationDetail)
        {
            var result = _accountReconciliationDetailService.Delete(accountReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("GetById")]
        public IActionResult GetById(int Id)
        {
            var result = _accountReconciliationDetailService.GetById(Id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        [HttpGet("GetList")]
        public IActionResult GetList(int accountreconciliationId)
        {
            var result = _accountReconciliationDetailService.GetList(accountreconciliationId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        [HttpPost("addFromExcel")]
        public IActionResult AddFromExcel(IFormFile file, int accountreconciliationId)
        {
            if (file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + ".xlsx";
                var filePath = $"{Directory.GetCurrentDirectory()}/Content/{fileName}";
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                    stream.Flush();
                }

                var result = _accountReconciliationDetailService.AddToExcel(filePath, accountreconciliationId);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result.Message);
            }
            return BadRequest("Dosya seçimi yapmadınız");
        }
    }
}
