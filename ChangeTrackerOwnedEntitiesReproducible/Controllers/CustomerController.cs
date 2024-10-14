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
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer([FromBody] CustomerDto customerDto)
        {
            var customer = Customer.Create(
                customerDto.Title,
                Name.Create(customerDto.Forename),
                Name.Create(customerDto.Surname),
                customerDto.CorrespondenceAddress,
                customerDto.DeliveryAddress,
                customerDto.DeliveryInstructions,
                customerDto.Email != null ? Email.Create(customerDto.Email) : null,
                customerDto.MobilePhoneNumber != null ? PhoneNumber.Create(customerDto.MobilePhoneNumber) : null,
                customerDto.AltTelephoneNumber != null ? PhoneNumber.Create(customerDto.AltTelephoneNumber) : null,
                customerDto.MarketingOptIn,
                customerDto.WelcomeCallComplete,
                customerDto.SmsMarketing
            );

            _context.Customers.Add(customer);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] CustomerDto customerDto)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            customer.Update(
                customerDto.Title,
                Name.Create(customerDto.Forename),
                Name.Create(customerDto.Surname),
                customerDto.CorrespondenceAddress,
                customerDto.DeliveryAddress,
                customerDto.DeliveryInstructions,
                customerDto.Email != null ? Email.Create(customerDto.Email) : null,
                customerDto.MobilePhoneNumber != null ? PhoneNumber.Create(customerDto.MobilePhoneNumber) : null,
                customerDto.AltTelephoneNumber != null ? PhoneNumber.Create(customerDto.AltTelephoneNumber) : null,
                customerDto.MarketingOptIn,
                customerDto.WelcomeCallComplete,
                customerDto.SmsMarketing
            );

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }
    }

    public class CustomerDto
    {
        public string? Title { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public CustomerAddress CorrespondenceAddress { get; set; }
        public CustomerAddress? DeliveryAddress { get; set; }
        public string? DeliveryInstructions { get; set; }
        public string? Email { get; set; }
        public string? MobilePhoneNumber { get; set; }
        public string? AltTelephoneNumber { get; set; }
        public bool MarketingOptIn { get; set; }
        public bool WelcomeCallComplete { get; set; }
        public bool SmsMarketing { get; set; }
    }
}