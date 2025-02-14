using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
  public class Employee
  {
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Phone { get; set; }

    [Required]
    public string JobTitle { get; set; }

    [Required]
    public string Department { get; set; }

    [Required]
    public decimal Salary { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
  }
  public class EmployeeDTO
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string JobTitle { get; set; }
    public string Department { get; set; }
    public decimal Salary { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? ProfilePicture { get; set; }
  }

}
