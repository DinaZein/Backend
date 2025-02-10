using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementAPI.Models
{
  public class Document
  {
    [Key]
    public int Id { get; set; }

    [ForeignKey("Employee")]
    public int EmployeeId { get; set; }

    [Required]
    public string FilePath { get; set; }

    [Required]
    public string FileType { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Employee Employee { get; set; }
  }
}
