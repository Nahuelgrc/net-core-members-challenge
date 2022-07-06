using System.Collections.Generic;
using FizzWare.NBuilder;
using InterviewProject.Data.Models;
using InterviewProject.Services.Models;
using InterviewProject.Shared;

namespace InterviewProject.Tests.Shared
{
    public static class FakeData
    {
        public static Tag GetTag(int id, string name = "")
        {
            return Builder<Tag>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => !string.IsNullOrWhiteSpace(name) ? x.Name = name : x.Name = $"Tag{id}")
                .Build();
        }

        public static IList<BusinessTag> GetBusinessTagList(int[] ids = null)
        {
            if (ids == null)
            {
                return new List<BusinessTag>()
                {
                    new BusinessTag
                    {
                        Id = 1,
                    },
                    new BusinessTag
                    {
                        Id = 2,
                    },
                };
            }

            var businessTagList = new List<BusinessTag>();

            foreach (var id in ids)
            {
                businessTagList.Add(new BusinessTag
                {
                    Id = id,
                });
            }

            return businessTagList;
        }

        public static MembersTags GetMembersTags(int id, int memberId, int tagId)
        {
            return Builder<MembersTags>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.MemberId = memberId)
                .With(x => x.TagId = tagId)
                .Build();
        }

        public static Member GetMember(int id, JobType jobType, string name)
        {
            return Builder<Member>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.JobType = jobType)
                .With(x => x.Name = name)
                .Build();
        }

        public static Contractor GetContractor(int id, int contractDuration, Member member)
        {
            return Builder<Contractor>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.ContractDuration = contractDuration)
                .With(x => x.Member = member)
                .Build();
        }

        public static Employee GetEmployee(int id, EmployeeRoleType roleType, Member member)
        {
            return Builder<Employee>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.RoleType = roleType)
                .With(x => x.Member = member)
                .Build();
        }

        public static BusinessContractor GetBusinessContractor(IList<BusinessTag> businessTags, int id = 1, int contractDuration = 3, string name = "test")
        {
            return Builder<BusinessContractor>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.ContractDuration = contractDuration)
                .With(x => x.Name = name)
                .With(x => x.JobType = JobType.Contractor)
                .With(x => x.Tags = businessTags)
                .Build();
        }

        public static BusinessEmployee GetBusinessEmployee(IList<BusinessTag> businessTags, int id = 1, EmployeeRoleType roleType = EmployeeRoleType.DeliveryManager, string name = "test")
        {
            return Builder<BusinessEmployee>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.RoleType = roleType)
                .With(x => x.Name = name)
                .With(x => x.JobType = JobType.Employee)
                .With(x => x.Tags = businessTags)
                .Build();
        }
    }
}
