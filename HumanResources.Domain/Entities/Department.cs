using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class Department:BaseEntity
    {

        [Display(Name ="اسم القسم")]
        [Required(ErrorMessage = "اسم القسم مطلوب")]
        [StringLength(100, ErrorMessage = "لا يزيد عن 100 حرف")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
