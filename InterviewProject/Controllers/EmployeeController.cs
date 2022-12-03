using AutoMapper;
using InterviewProject.Controllers.Models.Requests;
using InterviewProject.Controllers.Models.Responses;
using InterviewProject.Services.Abstractions;
using InterviewProject.Services.Models;
using InterviewProject.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IMemberService<BusinessEmployee> employeeService;
        private readonly IMapper mapper;

        public EmployeeController(IMemberService<BusinessEmployee> employeeService, IMapper mapper)
        {
            this.employeeService = employeeService;
            this.mapper = mapper;
        }

        // GET: api/employee
        [HttpGet]
        public async Task<ActionResult<ApiGetAllEmployeesResponse>> GetAll()
        {
            var response = await this.employeeService.GetAll();
            return this.Ok(new Models.Responses.ApiGetAllEmployeesResponse
            {
                Count = response.Count,
                Employees = this.mapper.Map<IList<ApiEmployeeResponse>>(response),
            });
        }

        // GET api/employee/5
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ApiEmployeeResponse>> GetById([FromRoute] int id)
        {
            try
            {
                var response = await this.employeeService.GetById(id);
                return this.Ok(this.mapper.Map<ApiEmployeeResponse>(response));
            }
            catch (NotFoundException)
            {
                return this.NotFound();
            }
        }


        // POST api/employee
        [HttpPost]
        public async Task<ActionResult<ApiEmployeeResponse>> Create([FromBody] ApiEmployeeRequest request)
        {
            var businessEmployeeRequest = this.mapper.Map<BusinessEmployee>(request);
            var businessEmployeeResponse = await this.employeeService.Create(businessEmployeeRequest);
            return this.Ok(this.mapper.Map<ApiEmployeeResponse>(businessEmployeeResponse));
        }

        // PUT api/employee/1
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<ApiEmployeeResponse>> Update([FromRoute] int id, [FromBody] ApiEmployeeUpdateRequest request)
        {
            try
            {
                var businessEmployeeRequest = this.mapper.Map<BusinessEmployee>(request);
                var businessEmployeeResponse = await this.employeeService.Update(id, businessEmployeeRequest);
                return Ok(this.mapper.Map<ApiEmployeeResponse>(businessEmployeeResponse));
            }
            catch (NotFoundException)
            {
                return this.NotFound();
            }
        }

        // DELETE api/employee/1
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<ApiEmployeeResponse>> Delete([FromRoute] int id)
        {
            try
            {
                await this.employeeService.Delete(id);
                return this.NoContent();
            }
            catch (NotFoundException)
            {
                return this.NotFound();
            }
        }
    }
}
