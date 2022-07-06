using System.Collections.Generic;

namespace InterviewProject.Controllers.Models.Responses
{
    public class ApiGetAllContractorsResponse
    {
        public int Count { get; set; }

        public IList<ApiContractorResponse> Contractors { get; set; }
    }
}
