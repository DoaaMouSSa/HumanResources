using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class Week
    {
        public int? Id { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public string? Date { get; set; }
        public bool IsDeleted { get; set; }
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
