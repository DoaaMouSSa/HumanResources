using HumanResources.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.Dtos
{
    public class WeekDto
    {
        public int Id { get; set; }
        public string Date { get; set; }
        //public List<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
