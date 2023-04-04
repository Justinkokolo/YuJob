using AutoMapper;
using backend.Core.Context;
using backend.Core.Dtos.Company;
using backend.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private ApplicationDbContext _context { get; }
        private IMapper _mapper { get; }

        public CompanyController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // CRUD 

        // Create
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreateDto dto)
        {
            Company newCompany = _mapper.Map<Company>(dto);
            await _context.Companies.AddAsync(newCompany);
            await _context.SaveChangesAsync();

            return Ok("Companty Created Successfully");
        }

        // Read
        [HttpGet]
        [Route("Get")]
        public async Task<ActionResult<IEnumerable<CompanyGetDto>>> GetCompanies()
        {
            var companies = await _context.Companies.OrderByDescending(q => q.CreatedAt).ToListAsync();
            var convertedCompanies = _mapper.Map<IEnumerable<CompanyGetDto>>(companies);

            return Ok(convertedCompanies);
        }

        // Read (Get Company By ID)
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CompanyGetDto>> GetCompanie([FromRoute] long id)
        {
            var company = await _context.Companies.FindAsync(id);
            var convertedCompany = _mapper.Map<CompanyGetDto>(company);


            if (convertedCompany is null)
            {
                return NotFound("Product Not Found");
            }

            return Ok(convertedCompany);
        }


        // Update 
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> UpdateCompany([FromRoute] long id, [FromBody] CompanyUpdateDto dto)

        {
            if (id != dto.ID)
            {
                return BadRequest();
            }
            var convertedCompany = _mapper.Map<Company>(dto);
            _context.Entry(convertedCompany).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Companies.Any(p => p.ID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();



        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<ActionResult<CompanyGetDto>> DeleteCompany(long id)
        {
            // var company = _context.Companies.FindAsync(id);
            var company = await _context.Companies.FindAsync(id);
         //   var convertedCompany = _mapper.Map<CompanyGetDto>(company);

            if (company == null)
            {
                return NotFound("Product Not Found");
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return Ok(company);
        }
    }
}
