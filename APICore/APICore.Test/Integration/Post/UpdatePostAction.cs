using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using APICore.API.BasicResponses;
using APICore.API.Controllers;
using APICore.Common.DTO.Request;
using APICore.Data;
using APICore.Data.Entities;
using APICore.Data.Entities.Enums;
using APICore.Data.UoW;
using APICore.Services;
using APICore.Services.Impls;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace APICore.Tests.Integration.Post
{
    public class UpdatePostAction
    {
        private DbContextOptions<CoreDbContext> ContextOptions { get; }

        public UpdatePostAction()
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

        [Fact(DisplayName = "Successfuly Update Post Should Return Ok Status Code (200)")]
        public async void SuccessfullyUpdatePostShouldReturnOk()
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

            var fakePostRequest = new UpdatePostRequest();
            fakePostRequest.Id = 1;
            fakePostRequest.Text = "Text updated!";
            
            //ACT
            await postController.AddPostAsync(new AddPostRequest());
            var taskResult = (ObjectResult)postController.UpdatePostAsync(fakePostRequest).Result;
            
            //ASSERT
            Assert.Equal(200, taskResult.StatusCode);
        }
    }
}