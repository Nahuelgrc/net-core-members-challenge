using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewProject.Services.Abstractions
{
    public interface IMemberService<T>
        where T : class
    {
        Task<IList<T>> GetAll();

        Task<T> GetById(int id);

        Task<T> Create(T member);

        Task<T> Update(int id, T member);

        Task Delete(int id);

        Task<IList<T>> Filter(IList<int> flagIds);
    }
}
