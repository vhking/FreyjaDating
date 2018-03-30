using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FreyjaDating.API.Data;
using FreyjaDating.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreyjaDating.API.Controllers
{
    [Authorize]
    [Route("api/[controller")]
    public class UsersController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;

        }

        [HttpPost]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDTO>>(users);
            return Ok(users);
        }
        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDTO>(user);
            return Ok(user);
        }
    }
}