using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EmployeeController : ControllerBase
  {
    private readonly AppDbContext _context;

    public EmployeeController(AppDbContext context)
    {
      _context = context;
    }

    // GET: api/employee
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(
      [FromQuery] string? search = "",
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 5)
    {
      if (page <= 0) page = 1;
      if (pageSize <= 0) pageSize = 5;

      var query = _context.Employees.AsQueryable();

      // **Filter by search term (name)**
      if (!string.IsNullOrEmpty(search))
      {
        query = query.Where(e => e.Name.Contains(search));
      }

      // **Pagination**
      var totalRecords = await query.CountAsync();
      var employees = await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

      // **Return with pagination metadata**
      return Ok(new
      {
        Data = employees,
        Page = page,
        PageSize = pageSize,
        TotalRecords = totalRecords,
        TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
      });
    }

    // GET: api/employee/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
      var employee = await _context.Employees.FindAsync(id);
      if (employee == null)
        return NotFound();

      return employee;
    }

    // POST: api/employee
    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
    {
      _context.Employees.Add(employee);
      await _context.SaveChangesAsync();
      return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
    }

    // PUT: api/employee/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
    {
      if (id != employee.Id)
        return BadRequest();

      _context.Entry(employee).State = EntityState.Modified;
      await _context.SaveChangesAsync();
      return NoContent();
    }

    // DELETE: api/employee/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
      var employee = await _context.Employees.FindAsync(id);
      if (employee == null)
        return NotFound();

      _context.Employees.Remove(employee);
      await _context.SaveChangesAsync();
      return NoContent();
    }
  }
}
