using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FreyjaDating.API.Data;
using FreyjaDating.API.DTOs;
using FreyjaDating.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FreyjaDating.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        
        public AuthController(
            IAuthRepository repo,
            IConfiguration config,
            IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
            _repo = repo;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDTO userForRegisterDTO)
        {
            if (!string.IsNullOrEmpty(userForRegisterDTO.Username))
            {
                // Stores the user name as lowercase. 
                userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();
            }

            // checks if  user exits
            if (await _repo.UserExists(userForRegisterDTO.Username))
            {
                ModelState.AddModelError("Username", "Username already exist");
            }

            // validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // sets user name 
            var userToCreate = _mapper.Map<User>(userForRegisterDTO);

            // register user
            var createdUser = await _repo.Register(userToCreate, userForRegisterDTO.Password);

            var userToReturn = _mapper.Map<UserForDetailedDTO>(createdUser);

            return CreatedAtRoute("GetUser", new {controller = "Users", id = createdUser.Id},userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDTO userForLoginDTO)
        {
            var userFromRepo = await _repo.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);
            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            // generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userForLoginDTO.Username)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescription);
            var tokenString = tokenHandler.WriteToken(token);
            var user = _mapper.Map<UserForListDTO>(userFromRepo);

            return Ok(new { tokenString, user });
        }


    }
}