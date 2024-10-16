using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Domain.Enums.Enums;

namespace HumanResources.Domain.Entities
{
    public class Employee:BaseEntity
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public Gender? Gender { get; set; }
        public float NetSalary { get; set; }//صافى المرتب
        public float? GrossSalary { get; set; }//اجمالى المرتب
        public MaritalStatus? MaritalStatus { get; set; }

        public DateOnly BirthOfDate { get; set; }

        public string? GraduationCertificateUrl { get; set; }
        public string? IdentityUrl { get; set; }
        public string? PersonalImageUrl { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }
        public DateOnly? DateOfAppointment { get; set; } // Date of Appointment
        public Governorate? Governorate { get; set; } // Date of Appointment
        public string? Address { get; set; } // Date of Appointment
        public string? Phone { get; set; } // Date of Appointment
        public JobPosition? JobPosition { get; set; } // Date of Appointment

        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public int DepartmentId { get; set; } // Foreign Key
        public Department? Department { get; set; } // Navigation Property
        public ICollection<Attendance> Attendances { get; set; }

    }
}
