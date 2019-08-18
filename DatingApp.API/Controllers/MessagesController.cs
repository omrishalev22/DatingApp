using System;
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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDatingRepository _repo;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;

        }

        [HttpGet("{id}", Name="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId , int id)
        {   
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) ) {
                    Unauthorized();
            }
            var message =  await _repo.GetMessage(id);
            var messageToReturn = _mapper.Map<MessageForCreationDto>(message);
            if(messageToReturn != null)
                return Ok(messageToReturn);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId,MessageForCreationDto messageForCreation)
        {
                if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) )
                    Unauthorized();
                
                messageForCreation.SenderId = userId;
                var recipient = await _repo.GetUser(messageForCreation.RecipientId);

                if(recipient == null)
                    BadRequest("no such user");

                var message = _mapper.Map<Message>(messageForCreation);
                _repo.Add(message);

                if(await _repo.SaveAll())
                {
                    var messageForReturn = _mapper.Map<MessageForCreationDto>(message);
                    return CreatedAtRoute("GetMessage", new {id = message.Id}, messageForReturn);
                }
                
                
                throw new Exception("Creating a new message failed");
        }

    }
}