using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DatingApp.API.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(UserActivityLog))]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var users = await _repo.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            HttpContext.Response.AddPagination(new PaginationHeader(userParams.PageNumber,userParams.PageSize,users.TotalCount,users.TotalPage));
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id){
            var user = await this._repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailsDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser(int id, UserForEditDto userForEdit){
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
                
            var userfromRepo = await this._repo.GetUser(id);
            _mapper.Map(userForEdit,userfromRepo);
            if( await _repo.SaveAll()){
                return NoContent();
            }
            throw new Exception($"Updating new user {id} failed on save");
        }

        [HttpPost("like/{likedUserId}")]
        public async Task<IActionResult> LikeUser(int likedUserId){
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            if(await _repo.GetLike(id,likedUserId) == null) {
                    var liker = await _repo.GetUser(id); // person who likes
                    var likee = await _repo.GetUser(likedUserId); // person's being liked
                    var entity = new Like(){Likee = likee , Liker = liker, LikerId = id, LikeeId = likedUserId};
                    likee.Likees.Add(entity);
                    liker.Likers.Add(entity);
                    _repo.Add(entity);

                    if(await _repo.SaveAll()){
                        return NoContent();
                    }

                    throw new Exception($"Couldn't like the user");
                }

                throw new Exception($"User already likes this user");
        }
    }
}