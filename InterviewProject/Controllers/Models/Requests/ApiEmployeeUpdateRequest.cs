using InterviewProject.Shared;
using System.Collections.Generic;

namespace InterviewProject.Controllers.Models.Requests
{
    public class ApiEmployeeUpdateRequest
    {
        public string Name { get; set; }

        public EmployeeRoleType RoleType { get; set; }

        public IList<int> Tags { get; set; }
    }
}
