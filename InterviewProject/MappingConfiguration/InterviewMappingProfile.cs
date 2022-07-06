using AutoMapper;
using InterviewProject.Controllers.Models;
using InterviewProject.Controllers.Models.Requests;
using InterviewProject.Controllers.Models.Responses;
using InterviewProject.Data.Models;
using InterviewProject.Services.Models;
using System.Linq;

namespace InterviewProject.MappingConfiguration
{
    public class InterviewMappingProfile : Profile
    {
        public InterviewMappingProfile()
        {
            // Api - Business
            this.CreateMap<int, BusinessTag>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(x => x));

            this.CreateMap<ApiMember, BusinessMember>()
                .ReverseMap()
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(x => x.Tags.Select(y => y.Id)));

            this.CreateMap<ApiContractorRequest, BusinessContractor>()
                .ReverseMap();

            this.CreateMap<ApiEmployeeRequest, BusinessEmployee>()
                .ReverseMap();

            this.CreateMap<BusinessContractor, ApiContractorResponse>()
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(x => x.Tags.Select(y => y.Id)))
                .ReverseMap();

            this.CreateMap<BusinessEmployee, ApiEmployeeResponse>()
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(x => x.Tags.Select(y => y.Id)))
                .ReverseMap();

            this.CreateMap<BusinessMember, ApiContractorResponse>()
                .ReverseMap();

            this.CreateMap<BusinessMember, ApiEmployeeResponse>()
                .ReverseMap();

            this.CreateMap<ApiContractorUpdateRequest, BusinessContractor>();

            this.CreateMap<ApiEmployeeUpdateRequest, BusinessEmployee>();

            // ---------------------------------------------------

            // Business - Data
            this.CreateMap<MembersTags, BusinessTag>()
               .ForMember(dto => dto.Id, opt => opt.MapFrom(x => x.TagId))
               .ReverseMap();

            this.CreateMap<BusinessMember, Member>()
                .ReverseMap();

            this.CreateMap<BusinessTag, Tag>()
                .ReverseMap();

            this.CreateMap<BusinessContractor, Contractor>()
                .ReverseMap()
                .ForMember(dto => dto.JobType, opt => opt.MapFrom(x => x.Member.JobType))
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(x => x.Member.MembersAndTags))
                .ForMember(dto => dto.Name, opt => opt.MapFrom(x => x.Member.Name))
                .ForMember(dto => dto.Id, opt => opt.MapFrom(x => x.Member.Id));

            this.CreateMap<BusinessEmployee, Employee>()
                .ReverseMap()
                .ForMember(dto => dto.JobType, opt => opt.MapFrom(x => x.Member.JobType))
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(x => x.Member.MembersAndTags))
                .ForMember(dto => dto.Name, opt => opt.MapFrom(x => x.Member.Name))
                .ForMember(dto => dto.Id, opt => opt.MapFrom(x => x.Member.Id));

        }
    }
}
