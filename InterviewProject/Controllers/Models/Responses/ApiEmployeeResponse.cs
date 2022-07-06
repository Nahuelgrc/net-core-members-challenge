using InterviewProject.Shared;

namespace InterviewProject.Controllers.Models.Responses
{
    public class ApiEmployeeResponse : ApiMember
    {
        public EmployeeRoleType RoleType { get; set; }
    }
}