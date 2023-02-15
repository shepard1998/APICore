using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using APICore.API.BasicResponses;
using APICore.Common.DTO.Request;
using APICore.Common.DTO.Response;
using APICore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APICore.API.Controllers
{
    [Route("api/post")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public PostController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        /// <summary>
        /// Add new post. Requires authentication
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddPostAsync([FromBody] AddPostRequest postRequest)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var currentUser = claimsIdentity.FindFirst(ClaimTypes.UserData)?.Value;
            
            await _postService.AddPostAsync(postRequest, int.Parse(currentUser));
            return Created("", true);
        }

        /// <summary>
        /// Update post. Requires Authentication
        /// </summary>
        [HttpPost("update-post")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdatePostAsync([FromBody] UpdatePostRequest postRequest)
        {
            var result = await _postService.UpdatePostAsync(postRequest);
            var post = _mapper.Map<PostResponse>(result);
            
            return Ok(new ApiOkResponse(post));
        }

        /// <summary>
        /// Delete post. Requires Authentication
        /// </summary>
        [HttpDelete("delete-post")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeletePostAsync([FromBody] DeletePostRequest postRequest)
        {
            await _postService.DeletePostAsync(postRequest);
            return Ok(new ApiOkResponse(postRequest));
        }

        /// <summary>
        /// List all post. Requires authentication
        /// </summary>
        [HttpGet()]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAllPosts()
        {
            var result = await _postService.GetPostAsync();
            var postList = _mapper.Map<IEnumerable<PostResponse>>(result);
            return Ok(new ApiOkResponse(postList));
        }

        /// <summary>
        ///List of current user's logs. Requires authentication
        /// </summary>
        [HttpGet("current-user-posts")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetCurrentUserPosts()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.UserData)?.Value;
            
            var result = await _postService.GetPostAsync(int.Parse(userId));

            var postList = _mapper.Map<IEnumerable<PostResponse>>(result);

            return Ok(new ApiOkResponse(postList));
        }
    }
}