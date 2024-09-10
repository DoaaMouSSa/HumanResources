using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class Employee:BaseEntity
    {
        public string Name { get; set; }
        public int DepartmentId { get; set; } // Foreign Key
        public Department Department { get; set; } // Navigation Property
    }
}
