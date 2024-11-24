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
        public DateTime? Code { get; set; }
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
