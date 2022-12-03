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
    public class ContractorController : ControllerBase
    {
        private readonly IMemberService<BusinessContractor> contractorService;
        private readonly IMapper mapper;

        public ContractorController(IMemberService<BusinessContractor> contractorService, IMapper mapper)
        {
            this.contractorService = contractorService;
            this.mapper = mapper;
        }

        // GET: api/contractor
        [HttpGet]
        public async Task<ActionResult<ApiGetAllContractorsResponse>> GetAll()
        {
            var response = await this.contractorService.GetAll();
            return this.Ok(new Models.Responses.ApiGetAllContractorsResponse
            {
                Count = response.Count,
                Contractors = this.mapper.Map<IList<ApiContractorResponse>>(response),
            });
        }

        // GET api/contractor/5
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ApiContractorResponse>> GetById([FromRoute] int id)
        {
            try
            {
                var response = await this.contractorService.GetById(id);
                return this.Ok(this.mapper.Map<ApiContractorResponse>(response));
            }
            catch (NotFoundException)
            {
                return this.NotFound();
            }
        }


        // POST api/contractor
        [HttpPost]
        public async Task<ActionResult<ApiContractorResponse>> Create([FromBody] ApiContractorRequest request)
        {
            var businessContractorRequest = this.mapper.Map<BusinessContractor>(request);
            var businessContractorResponse = await this.contractorService.Create(businessContractorRequest);
            return this.Ok(this.mapper.Map<ApiContractorResponse>(businessContractorResponse));
        }

        // PUT api/contractor/1
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<ApiContractorResponse>> Update([FromRoute] int id, [FromBody] ApiContractorUpdateRequest request)
        {
            try
            {
                var businessContractorRequest = this.mapper.Map<BusinessContractor>(request);
                var businessContractorResponse = await this.contractorService.Update(id, businessContractorRequest);
                return Ok(this.mapper.Map<ApiContractorResponse>(businessContractorResponse));
            }
            catch (NotFoundException)
            {
                return this.NotFound();
            }
        }

        // DELETE api/contractor/1
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<ApiContractorResponse>> Delete([FromRoute] int id)
        {
            try
            {
                await this.contractorService.Delete(id);
                return this.NoContent();
            }
            catch (NotFoundException)
            {
                return this.NotFound();
            }
        }
    }
}
