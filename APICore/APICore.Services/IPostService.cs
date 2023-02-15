using System.Linq;
using System.Threading.Tasks;
using APICore.Common.DTO.Request;
using APICore.Data.Entities;

namespace APICore.Services
{
    public interface IPostService
    {
        Task<IQueryable<Post>> GetPostAsync(int userId = 0);
        Task<IQueryable<Post>> GetPostByUserSerialAsync(string serialUser);
        Task AddPostAsync(AddPostRequest postRequest, int currentUser);
        Task<Post> UpdatePostAsync (UpdatePostRequest postRequest);
        Task DeletePostAsync(DeletePostRequest deletePostRequest);
    }
}