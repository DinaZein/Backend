using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TimesheetController : ControllerBase
  {
    private readonly AppDbContext _context;

    public TimesheetController(AppDbContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Timesheet>>> GetTimesheets()
    {
      return await _context.Timesheets.Include(t => t.Employee).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Timesheet>> GetTimesheet(int id)
    {
      var timesheet = await _context.Timesheets.Include(t => t.Employee).FirstOrDefaultAsync(t => t.Id == id);

      if (timesheet == null)
        return NotFound();

      return timesheet;
    }

    [HttpPost]
    public async Task<ActionResult<Timesheet>> CreateTimesheet(Timesheet timesheet)
    {
      try
      {
        _context.Timesheets.Add(timesheet);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTimesheet), new { id = timesheet.Id }, timesheet);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error creating timesheet: {ex.Message}");
        return StatusCode(500, "An error occurred while creating the timesheet.");
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTimesheet(int id, Timesheet timesheet)
    {
      if (id != timesheet.Id)
        return BadRequest();

      _context.Entry(timesheet).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
        return NoContent();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error updating timesheet: {ex.Message}");
        return StatusCode(500, "An error occurred while updating the timesheet.");
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTimesheet(int id)
    {
      var timesheet = await _context.Timesheets.FindAsync(id);
      if (timesheet == null)
        return NotFound();

      _context.Timesheets.Remove(timesheet);
      await _context.SaveChangesAsync();
      return NoContent();
    }
  }
}
