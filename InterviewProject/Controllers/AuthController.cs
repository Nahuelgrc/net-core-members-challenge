using AutoMapper;
using InterviewProject.Controllers.Models.Requests;
using InterviewProject.Controllers.Models.Responses;
using InterviewProject.Services.Abstractions;
using InterviewProject.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InterviewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IMapper mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            this.authService = authService;
            this.mapper = mapper;
        }

        // POST api/auth/register
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<ApiLoginResponse>> RegisterAndLogin([FromBody] ApiLoginRequest request)
        {
            var response = await this.authService.RegisterAndLogin(request.Username, request.Password);
            return Ok(new ApiLoginResponse
            {
                Token = response.Token,
                Id = response.Id,
                RoleType = response.RoleType
            });
        }

        // POST api/auth/login
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] ApiLoginRequest request)
        {
            try
            {
                var response = await this.authService.Login(request.Username, request.Password);
                return Ok(new ApiLoginResponse
                {
                    Token = response.Token,
                    Id = response.Id,
                    RoleType = response.RoleType
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound();
            }
        }
    }
}
