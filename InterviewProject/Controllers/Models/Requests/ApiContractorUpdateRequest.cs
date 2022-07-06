using System.Collections.Generic;

namespace InterviewProject.Controllers.Models.Requests
{
    public class ApiContractorUpdateRequest
    {
        public string Name { get; set; }

        public int ContractDuration { get; set; }

        public IList<int> Tags { get; set; }
    }
}
