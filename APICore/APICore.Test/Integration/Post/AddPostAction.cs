using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using APICore.API.Controllers;
using APICore.Common.DTO.Request;
using APICore.Data;
using APICore.Data.Entities;
using APICore.Data.Entities.Enums;
using APICore.Data.UoW;
using APICore.Services;
using APICore.Services.Exceptions;
using APICore.Services.Impls;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace APICore.Tests.Integration.Post
{
    public class AddPostAction
    {
        private DbContextOptions<CoreDbContext> ContextOptions { get; }

        public AddPostAction()
        {
            ContextOptions = new DbContextOptionsBuilder<CoreDbContext>()
                .UseInMemoryDatabase("TestPostDatabase")
                .Options;

            SeedAsync().Wait();
        }

        private async Task SeedAsync()
        {
            await using var context = new CoreDbContext(ContextOptions);

            if (await context.Users.AnyAsync() == false)
            {
                await context.Users.AddAsync(new User
                {
                    Id = 3,
                    Email = "pepe@itguy.com",
                    FullName = "Pepe Delgado",
                    Gender = 0,
                    Phone = "+53 12345678",
                    Password = @"gM3vIavHvte3fimrk2uVIIoAB//f2TmRuTy4IWwNWp0=",
                    Status = StatusEnum.ACTIVE
                });

                await context.SaveChangesAsync();
            }
        }

        [Fact(DisplayName = "Successfully Add Post Should Return Created Status Code (201)")]
        public void SuccessfullyAddPostShouldReturnCreated()
        {
            //ARRANGE
            var fakeClaims = new List<Claim>()
            {
                new(ClaimTypes.UserData, "4")
            };
            var identity = new ClaimsIdentity(fakeClaims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(claimsPrincipal)
            };
            httpContext.Request.Headers.Add("Authorization", @"Bearer s0m34cc$3$$T0k3n");
            
            using var context = new CoreDbContext(ContextOptions);
            
            var postService = new PostService(new UnitOfWork(context), new Mock<IStringLocalizer<IAccountService>>().Object);
            var postController = new PostController(postService, new Mock<AutoMapper.IMapper>().Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                }
            };
            
            var fakePostRequest = new AddPostRequest
            {
                Text = "New Post Test!"
            };

            //ACT
            var taskResult = (ObjectResult)postController.AddPostAsync(fakePostRequest).Result;

            //ASSERT
            Assert.Equal(201, taskResult.StatusCode);
        }

        [Fact(DisplayName = "Empty Text In Post Should Return Bad Request Exception")]
        public void EmptyTextInPostShouldReturnBadRequestException()
        {
            //ARRANGE
            var fakeClaims = new List<Claim>()
            {
                new(ClaimTypes.UserData, "4")
            };
            var identity = new ClaimsIdentity(fakeClaims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(claimsPrincipal)
            };
            httpContext.Request.Headers.Add("Authorization", @"Bearer s0m34cc$3$$T0k3n");
            
            using var context = new CoreDbContext(ContextOptions);
            
            var postService = new PostService(new UnitOfWork(context), new Mock<IStringLocalizer<IAccountService>>().Object);
            var postController = new PostController(postService, new Mock<AutoMapper.IMapper>().Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                }
            };
            
            var fakePostRequest = new AddPostRequest
            {
                Text = ""
            };

            //ACT
            var aggregateException = postController.AddPostAsync(fakePostRequest).Exception;
            var taskResult = (BaseBadRequestException)aggregateException?.InnerException;

            //ASSERT
            if (taskResult != null) Assert.Equal(400, taskResult.HttpCode);
        }
    }


}