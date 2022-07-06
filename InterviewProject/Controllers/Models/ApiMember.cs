using InterviewProject.Shared;
using System.Collections.Generic;

namespace InterviewProject.Controllers.Models
{
    public class ApiMember
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public JobType JobType { get; set; }

        public IList<int> Tags { get; set; }
    }
}