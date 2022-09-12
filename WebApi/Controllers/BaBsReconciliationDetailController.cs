using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaBsReconciliationDetailController : ControllerBase
    {
        private readonly IBaBsReconciliationDetailService _baBsReconciliationDetailService;

        public BaBsReconciliationDetailController(IBaBsReconciliationDetailService baBsReconciliationDetailService)
        {
            _baBsReconciliationDetailService = baBsReconciliationDetailService;
        }

        [HttpPost("add")]
        public IActionResult Add(BaBsReconciliationDetail baBsReconciliationDetail)
        {
            var result = _baBsReconciliationDetailService.Add(baBsReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        [HttpPost("Update")]
        public IActionResult Update(BaBsReconciliationDetail baBsReconciliationDetail)
        {
            var result = _baBsReconciliationDetailService.Update(baBsReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        [HttpPost("Delete")]
        public IActionResult Delete(BaBsReconciliationDetail baBsReconciliationDetail)
        {
            var result = _baBsReconciliationDetailService.Delete(baBsReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        [HttpGet("GetById")]
        public IActionResult GetById(int Id)
        {
            var result = _baBsReconciliationDetailService.GetById(Id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        [HttpGet("GetList")]
        public IActionResult GetList(int baBsReconciliationId)
        {
            var result = _baBsReconciliationDetailService.GetList(baBsReconciliationId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("addFromExcel")]
        public IActionResult AddFromExcel(IFormFile file, int companyId)
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

                var result = _baBsReconciliationDetailService.AddToExcel(filePath, companyId);
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
