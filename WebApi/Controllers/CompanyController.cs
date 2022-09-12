using Business.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyservice;

        public CompanyController(ICompanyService companyservice)
        {
            _companyservice = companyservice;
        }

        [HttpGet("getcompanylist")]
        public IActionResult GetCompanyList()
        {
            var result = _companyservice.GetList();
            if (result.Success)
            {
                return Ok(result);
            }
           return BadRequest(result.Message);
        }
        [HttpGet("getbyıd")]
        public IActionResult GetById(int Id)
        {
            var result = _companyservice.GetById(Id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost ("addcompanyandusercompany")]
        public IActionResult AddCompanyAndUserCompany(CompanyDto companyDto)
        {
            var result = _companyservice.AddCompanyAndUserCompany(companyDto);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
        [HttpPost("updateCompany")]
        public IActionResult UpdateCompany(Company company)
        {
            var result = _companyservice.Update(company);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
