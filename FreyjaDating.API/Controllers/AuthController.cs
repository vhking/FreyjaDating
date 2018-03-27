using System.Threading.Tasks;
using FreyjaDating.API.Data;
using FreyjaDating.API.DTOs;
using FreyjaDating.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace FreyjaDating.API.Controllers
{
    [Route("api/[Controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDTO userForRegisterDTO)
        {
           
            // Stores the user name as lowercase. 
            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();
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
            var userToCreate = new User
            {
                Username = userForRegisterDTO.Username
            };

            // register user
            var createdUser = await _repo.Register(userToCreate, userForRegisterDTO.Password);

            return StatusCode(281);
        }

    }
}