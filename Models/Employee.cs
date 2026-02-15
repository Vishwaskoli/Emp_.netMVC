using System.ComponentModel.DataAnnotations;

namespace ADO.Models
{
    public class Employee
    {
        public int EmpId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public DateOnly DOJ {  get; set; }
        [Required]
        public int DeptID { get; set; }
        [Required]
        public int DesgID { get; set; }
        public byte[]? ProfileImg { get; set; } 
    }
}
