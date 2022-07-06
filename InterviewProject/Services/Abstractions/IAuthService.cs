using InterviewProject.Services.Models;
using InterviewProject.Shared;
using System.Threading.Tasks;

namespace InterviewProject.Services.Abstractions
{
    public interface IAuthService
    {
        Task<BusinessLogin> RegisterAndLogin(string username, string password);
        Task<BusinessLogin> Login(string username, string password);
    }
}
