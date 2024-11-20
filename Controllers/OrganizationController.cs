using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TaskOne.Data;
using TaskOne.Models;

namespace TaskOne.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly OrganizationDbContext _context;

    public OrganizationController(OrganizationDbContext context)
    {
        _context = context;
    }

    [HttpGet("GetList")]
    public async Task<IActionResult> GetList(int pageNumber = 1, int pageSize = 100)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var data = await _context.Organizations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalRecords = await _context.Organizations.CountAsync();

            stopwatch.Stop();

            return Ok(new { 
                Data = data, 
                TotalRecords = totalRecords,
                TimeTaken = stopwatch.Elapsed.TotalSeconds + " Second"
            });
        }   
        catch (Exception ex)
        {
            return StatusCode(500, $"Ichki server xatosi: {ex.Message}");
        }
    }

    [HttpGet("Search")]
    public async Task<IActionResult> Search(string query, int pageNumber = 1, int pageSize = 100)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("So'rov parametri bo'sh bo'lishi mumkin emas.");
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();

            var data = _context.Organizations.Where(o =>
                EF.Functions.Like(o.Name, $"%{query}%") ||
                EF.Functions.Like(o.Address, $"%{query}%") ||
                EF.Functions.Like(o.PhoneNumber, $"%{query}%") ||
                EF.Functions.Like(o.Email, $"%{query}%") ||
                EF.Functions.Like(o.Website, $"%{query}%") ||
                EF.Functions.Like(o.ContactPerson, $"%{query}%") ||
                EF.Functions.Like(o.ContactPersonPhone, $"%{query}%") ||
                EF.Functions.Like(o.ContactPersonEmail, $"%{query}%")
            );

            var result = await data
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalRecords = await data.CountAsync();

            stopwatch.Stop();

            return Ok(new
            {
                Data = result,
                TotalRecords = totalRecords,
                TimeTaken = stopwatch.Elapsed.TotalSeconds + " Second"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ichki server xatosi: {ex.Message}");
        }
    }

    [HttpPost("CreateRandomData")]
    public async Task<IActionResult> CreateRandomData(int count = 1000)
    {
        var stopwatch = Stopwatch.StartNew(); // Vaqtni boshlash

        // Random ma'lumotlar yaratish uchun Bogus Faker
        var faker = new Bogus.Faker<Organization>()
            .RuleFor(o => o.Name, f => f.Company.CompanyName())
            .RuleFor(o => o.Address, f => f.Address.StreetAddress())
            .RuleFor(o => o.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(o => o.Email, f => f.Internet.Email())
            .RuleFor(o => o.Website, f => f.Internet.DomainName())
            .RuleFor(o => o.ContactPerson, f => f.Name.FullName())
            .RuleFor(o => o.ContactPersonPhone, f => f.Phone.PhoneNumber())
            .RuleFor(o => o.ContactPersonEmail, f => f.Internet.Email());

        // Random ma'lumotlar ro'yxati yaratish
        var organizations = faker.Generate(count);

        try
        {
            // Ma'lumotlarni ma'lumotlar bazasiga qo'shish
            await _context.Organizations.AddRangeAsync(organizations);
            await _context.SaveChangesAsync();

            stopwatch.Stop(); // Vaqtni to'xtatish

            return Ok(new
            {
                Message = $"{count} ta ma'lumotlar muvaffaqiyatli qo'shildi.",
                Duration = $"{stopwatch.Elapsed.TotalSeconds} Second"
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop(); // Vaqtni to'xtatish
            return StatusCode(500, new
            {
                Message = $"Ichki server xatosi: {ex.Message}",
                Duration = $"{stopwatch.Elapsed.TotalSeconds} Second"

            });
        }
    }

}
