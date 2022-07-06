using InterviewProject.Shared;
using System.Collections.Generic;

namespace InterviewProject.Services.Models
{
    public class BusinessMember
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public JobType JobType { get; set; }

        public IList<BusinessTag> Tags { get; set; }
    }
}