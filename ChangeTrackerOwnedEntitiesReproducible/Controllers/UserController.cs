using ChangeTrackerOwnedEntitiesReproducible.Domain;
using ChangeTrackerOwnedEntitiesReproducible.Domain.ValueObject;
using ChangeTrackerOwnedEntitiesReproducible.Infrastructure;
using ChangeTrackerOwnedEntitiesReproducible.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChangeTrackerOwnedEntitiesReproducible.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(ApplicationDbContext context, UnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] UserDto userDto)
        {
            var user = Domain.User.Create(
                userDto.Title,
                Name.Create(userDto.Forename),
                Name.Create(userDto.Surname),
                userDto.Email != null ? Email.Create(userDto.Email) : null,
                userDto.MobilePhoneNumber != null ? PhoneNumber.Create(userDto.MobilePhoneNumber) : null
            );

            _context.Users.Add(user);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Update(
                userDto.Title,
                Name.Create(userDto.Forename),
                Name.Create(userDto.Surname),
                userDto.Email != null ? Email.Create(userDto.Email) : null,
                userDto.MobilePhoneNumber != null ? PhoneNumber.Create(userDto.MobilePhoneNumber) : null
            );

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }

    public class UserDto
    {
        public string? Title { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string? Email { get; set; }
        public string? MobilePhoneNumber { get; set; }
    }
}