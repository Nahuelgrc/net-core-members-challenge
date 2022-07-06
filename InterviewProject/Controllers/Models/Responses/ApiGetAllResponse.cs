using System.Collections.Generic;

namespace InterviewProject.Controllers.Models.Responses
{
    public class ApiGetAllResponse
    {
        public int Count { get; set; }

        public IList<ApiContractorResponse> Contractors { get; set; }

        public IList<ApiEmployeeResponse> Employees { get; set; }
    }
}
