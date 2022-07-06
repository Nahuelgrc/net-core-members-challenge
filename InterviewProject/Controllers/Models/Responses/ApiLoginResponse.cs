using InterviewProject.Shared;

namespace InterviewProject.Controllers.Models.Responses
{
    public class ApiLoginResponse
    {
        public string Token { get; set; }
        public int Id { get; set; }
        public string RoleType { get; set; }
    }
}
