using System.Threading.Tasks;

namespace InterviewProject.Services.Abstractions
{
    public interface IAdminService
    {
        Task AddTag(string name);
    }
}
