using AutoMapper;
using InterviewProject.Controllers.Models.Responses;
using InterviewProject.Services.Abstractions;
using InterviewProject.Services.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InterviewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonMemberController : ControllerBase
    {
        private readonly IMemberService<BusinessContractor> contractorService;
        private readonly IMemberService<BusinessEmployee> employeeService;
        private readonly IMapper mapper;

        public CommonMemberController(
            IMemberService<BusinessContractor> contractorService,
            IMemberService<BusinessEmployee> employeeService,
            IMapper mapper)
        {
            this.contractorService = contractorService;
            this.employeeService = employeeService;
            this.mapper = mapper;
        }

        // GET: api/commonmember
        [HttpGet]
        public async Task<ActionResult<ApiGetAllResponse>> GetAll()
        {
            var contractorsResponse = await contractorService.GetAll();
            var employeesResponse = await employeeService.GetAll();

            return Ok(new ApiGetAllResponse
            {
                Count = contractorsResponse.Count + employeesResponse.Count,
                Contractors = this.mapper.Map<IList<ApiContractorResponse>>(contractorsResponse),
                Employees = this.mapper.Map<IList<ApiEmployeeResponse>>(employeesResponse)
            });
        }

        // GET: api/commonmember/search?tags=[1,2]
        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<ApiGetAllResponse>> Search([FromQuery] List<int> tags)
        {
            var contractorsResponse = await contractorService.Filter(tags);
            var employeesResponse = await employeeService.Filter(tags);

            return Ok(new ApiGetAllResponse
            {
                Count = contractorsResponse.Count + employeesResponse.Count,
                Contractors = this.mapper.Map<IList<ApiContractorResponse>>(contractorsResponse),
                Employees = this.mapper.Map<IList<ApiEmployeeResponse>>(employeesResponse)
            });
        }
    }
}
