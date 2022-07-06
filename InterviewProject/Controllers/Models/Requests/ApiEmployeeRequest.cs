using InterviewProject.Shared;

namespace InterviewProject.Controllers.Models.Requests
{
    public class ApiEmployeeRequest : ApiMember
    {
        public EmployeeRoleType RoleType { get; set; }
    }
}
