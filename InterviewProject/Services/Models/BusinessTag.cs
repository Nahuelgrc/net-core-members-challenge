using System.Collections.Generic;

namespace InterviewProject.Services.Models
{
    public class BusinessTag
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<BusinessMember> Members { get; set; }
    }
}
