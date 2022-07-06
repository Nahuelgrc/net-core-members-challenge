using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewProject.Controllers.Models.Requests
{
    public class ApiAuthRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
