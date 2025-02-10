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
    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> GetEmployees([FromQuery] string? search, int page = 1, int pageSize = 5)
    {
      var query = _context.Employees
          .Where(e => string.IsNullOrEmpty(search) || e.Name.Contains(search))
          .Select(e => new EmployeeDTO
          {
            Id = e.Id,
            Name = e.Name,
            Email = e.Email,
            Phone = e.Phone,
            JobTitle = e.JobTitle,
            Department = e.Department,
            Salary = e.Salary,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            ProfilePicture = _context.Documents
                  .Where(d => d.EmployeeId == e.Id && d.FileType == "ProfilePicture")
                  .Select(d => d.FilePath)
                  .FirstOrDefault()
          });

      int totalRecords = query.Count();

      var employees = query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToList();

      return Ok(new
      {
        data = employees,
        page,
        pageSize,
        totalRecords,
        totalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
      });
    }


    // GET: api/employee/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
      var employee = await _context.Employees.FindAsync(id);
      if (employee == null)
        return NotFound();
      var profilePicture = await _context.Documents
          .Where(d => d.EmployeeId == id && d.FileType == "ProfilePicture")
          .Select(d => d.FilePath)
          .FirstOrDefaultAsync();

      return Ok(new
      {
        employee.Id,
        employee.Name,
        employee.Email,
        employee.Phone,
        employee.JobTitle,
        employee.Department,
        employee.Salary,
        employee.StartDate,
        employee.EndDate,
        ProfilePicture = profilePicture
      });
    }



    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromForm] Employee employee, IFormFile? profilePicture)
    {

      try
      {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        if (profilePicture != null)
        {
          var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/photos");
          Directory.CreateDirectory(uploadsFolder);

          var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profilePicture.FileName);
          var filePath = Path.Combine(uploadsFolder, uniqueFileName);

          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await profilePicture.CopyToAsync(stream);
          }


          var document = new Document
          {
            EmployeeId = employee.Id,
            FilePath = $"uploads/photos/{uniqueFileName}",
            FileType = "ProfilePicture"
          };

          _context.Documents.Add(document);
          await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error: {ex.Message}");
      }
    }


    // PUT: api/employee/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromForm] Employee employee, IFormFile? profilePicture)
    {
      if (id != employee.Id)
        return BadRequest("Employee ID mismatch.");

      try
      {

        var existingEmployee = await _context.Employees.FindAsync(id);
        if (existingEmployee == null)
          return NotFound("Employee not found.");

        existingEmployee.Name = employee.Name;
        existingEmployee.Email = employee.Email;
        existingEmployee.Phone = employee.Phone;
        existingEmployee.JobTitle = employee.JobTitle;
        existingEmployee.Department = employee.Department;
        existingEmployee.Salary = employee.Salary;
        existingEmployee.StartDate = employee.StartDate;
        existingEmployee.EndDate = employee.EndDate;

        if (profilePicture != null)
        {
          var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/photos");
          Directory.CreateDirectory(uploadsFolder);

          var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profilePicture.FileName);
          var filePath = Path.Combine(uploadsFolder, uniqueFileName);

          using (var stream = new FileStream(filePath, FileMode.Create))
          {
            await profilePicture.CopyToAsync(stream);
          }


          var existingDocument = await _context.Documents
              .FirstOrDefaultAsync(d => d.EmployeeId == id && d.FileType == "ProfilePicture");

          if (existingDocument != null)
          {
            existingDocument.FilePath = $"uploads/photos/{uniqueFileName}";
          }
          else
          {
            var document = new Document
            {
              EmployeeId = id,
              FilePath = $"uploads/photos/{uniqueFileName}",
              FileType = "ProfilePicture"
            };
            _context.Documents.Add(document);
          }
        }

        await _context.SaveChangesAsync();
        return NoContent();
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error: {ex.Message}");
      }
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
    [HttpPost("{id}/upload-photo")]
    public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
    {
      var employee = await _context.Employees.FindAsync(id);
      if (employee == null)
      {
        return NotFound("Employee not found.");
      }

      if (file == null || file.Length == 0)
      {
        return BadRequest("No file uploaded.");
      }

      var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/photos");
      if (!Directory.Exists(uploadsFolder))
      {
        Directory.CreateDirectory(uploadsFolder);
      }

      var filePath = Path.Combine(uploadsFolder, $"{id}_{file.FileName}");
      using (var stream = new FileStream(filePath, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }

      var existingPhoto = await _context.Documents
          .FirstOrDefaultAsync(d => d.EmployeeId == id && d.FileType == "Profile Picture");

      if (existingPhoto != null)
      {
        _context.Documents.Remove(existingPhoto);
      }

      var document = new Document
      {
        EmployeeId = id,
        FilePath = $"/uploads/photos/{id}_{file.FileName}",
        FileType = "Profile Picture"
      };

      _context.Documents.Add(document);
      await _context.SaveChangesAsync();

      return Ok(new { Message = "Profile Picture uploaded successfully.", DocumentPath = document.FilePath });
    }
    [HttpGet("{id}/photo")]
    public async Task<IActionResult> GetProfilePicture(int id)
    {
      var photo = await _context.Documents
          .Where(d => d.EmployeeId == id && d.FileType == "Profile Picture")
          .OrderByDescending(d => d.UploadedAt) // Get the latest picture
          .FirstOrDefaultAsync();

      if (photo == null)
      {
        return NotFound("No profile picture found.");
      }

      return Ok(new { PhotoPath = photo.FilePath });
    }
    [HttpPost("{id}/upload-document")]
    public async Task<IActionResult> UploadDocument(int id, IFormFile file, string fileType)
    {
      var employee = await _context.Employees.FindAsync(id);
      if (employee == null)
      {
        return NotFound("Employee not found.");
      }

      if (file == null || file.Length == 0)
      {
        return BadRequest("No file uploaded.");
      }

      var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/documents");
      if (!Directory.Exists(uploadsFolder))
      {
        Directory.CreateDirectory(uploadsFolder);
      }

      var filePath = Path.Combine(uploadsFolder, $"{id}_{file.FileName}");
      using (var stream = new FileStream(filePath, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }

      var document = new Document
      {
        EmployeeId = id,
        FilePath = $"/uploads/documents/{id}_{file.FileName}",
        FileType = fileType
      };

      _context.Documents.Add(document);
      await _context.SaveChangesAsync();

      return Ok(new { Message = "Document uploaded successfully.", DocumentPath = document.FilePath });
    }
    [HttpDelete("delete-document/{documentId}")]

    [HttpGet("{id}/documents")]
    public async Task<IActionResult> GetEmployeeDocuments(int id)
    {
      var documents = await _context.Documents
          .Where(d => d.EmployeeId == id)
          .Select(d => new { d.Id, d.FilePath, d.FileType, d.UploadedAt })
          .ToListAsync();

      return Ok(documents);
    }

    public async Task<IActionResult> DeleteDocument(int documentId)
    {
      var document = await _context.Documents.FindAsync(documentId);
      if (document == null)
      {
        return NotFound("Document not found.");
      }

      var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.FilePath.TrimStart('/'));

      if (System.IO.File.Exists(filePath))
      {
        System.IO.File.Delete(filePath);
      }

      _context.Documents.Remove(document);
      await _context.SaveChangesAsync();

      return Ok(new { Message = "Document deleted successfully." });
    }

  }
}
