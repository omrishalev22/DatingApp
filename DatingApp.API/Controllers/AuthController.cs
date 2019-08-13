using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;

namespace DatingApp.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _repo = repo;
            _config = config;
            _mapper = mapper;
        }

        
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto){
        userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
        
        if(await this._repo.UserExists(userForRegisterDto.Username)){
           return BadRequest("user name already exists!");
        }
            var userToCreate = new User(){Username = userForRegisterDto.Username};
            var createUser = await this._repo.Register(userToCreate,userForRegisterDto.Password);
            return StatusCode(201);
        }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLoginDto){
        var userFromRepo = await this._repo.Login(userForLoginDto.Username, userForLoginDto.Password );    
        if(userFromRepo == null)
            return Unauthorized();
        var claims = new [] {
            new Claim(ClaimTypes.NameIdentifier ,userFromRepo.Id.ToString()),
            new Claim(ClaimTypes.Name ,userFromRepo.Username),
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
        var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds 
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var user = _mapper.Map<UserForListDto>(userFromRepo);
        return Ok(new {
            token = tokenHandler.WriteToken(token),
            user = user
        });


    }   

    }

    

}