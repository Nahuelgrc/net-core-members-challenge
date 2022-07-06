using System.Collections.Generic;

namespace InterviewProject.Controllers.Models.Responses
{
    public class ApiGetAllEmployeesResponse
    {
        public int Count { get; set; }

        public IList<ApiEmployeeResponse> Employees { get; set; }
    }
}
