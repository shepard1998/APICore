using System;
using System.Linq;
using System.Threading.Tasks;
using APICore.Common.DTO.Request;
using APICore.Data.Entities;
using APICore.Data.UoW;
using APICore.Services.Exceptions;
using Microsoft.Extensions.Localization;

namespace APICore.Services.Impls
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _uow;

        private readonly IStringLocalizer<IAccountService> _localizer;

        public PostService(IUnitOfWork uow, IStringLocalizer<IAccountService> localizer)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<IQueryable<Post>> GetPostAsync(int userId=0)
        {
            var result = _uow.PostRepository.GetAll();

            if (userId != 0)
            {
                result = result.Where(l => l.UserId == userId);
            }

            return await Task.FromResult(result);
        }

        public async Task<IQueryable<Post>> GetPostByUserSerialAsync(string serialUser)
        {
            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Identity == serialUser);

            if (user == null)
            {
                throw new UserNotFoundException(_localizer);
            }

            var result = GetPostAsync(user.Id);

            return await result;
        }

        public async Task AddPostAsync(AddPostRequest postRequest, int currentUser)
        {
            if (postRequest.Text == "")
            {
                throw new EmptyPostTextBadRequestException(_localizer);
            }
            
            var post = new Post
            {
                Text = postRequest.Text,
                CreatedAt = DateTime.Now,
                UserId = currentUser
            };

            await _uow.PostRepository.AddAsync(post);
            await _uow.CommitAsync();
        }

        public async Task<Post> UpdatePostAsync(UpdatePostRequest postRequest)
        {
            var post = await _uow.PostRepository.FirstOrDefaultAsync(u => u.Id == postRequest.Id);
            Console.WriteLine("here");
            if (post == null)
            {
                throw new PostNotFoundException(_localizer);
            }

            post.Text = postRequest.Text;

            _uow.PostRepository.Update(post);
            await _uow.CommitAsync();

            return post;
        }

        public async Task DeletePostAsync(DeletePostRequest deletePostRequest)
        {
            var post = await _uow.PostRepository.FirstOrDefaultAsync(u => u.Id == deletePostRequest.Id);

            if (post == null)
            {
                throw new PostNotFoundException(_localizer);
            }
            
            _uow.PostRepository.Delete(post);
            await _uow.CommitAsync();
        }
    }
}