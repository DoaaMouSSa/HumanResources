﻿using HumanResources.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HumanResources.Domain.Enums.Enums;
using HumanResources.Application.Validations;

namespace HumanResources.Application.Dtos
{
    public class EmployeeDto
    {
        public class EmployeeDtoForAdd
        {
            [Required(ErrorMessage = "الكود مطلوب.")]
            [UniqueIntCode] 
            public int Code { get; set; }
            [Required(ErrorMessage = "هذا الحقل مطلوب")]

            public string Name { get; set; }
            [Required(ErrorMessage = "هذا الحقل مطلوب")]

            public string Address { get; set; } // Date of Appointment
            [Required(ErrorMessage = "هذا الحقل مطلوب")]

            public string Phone { get; set; } // Date of Appointment
            [Required(ErrorMessage = "هذا الحقل مطلوب")]

            public float GrossSalary { get; set; }//اجمالى المرتب
            [Required(ErrorMessage = "هذا الحقل مطلوب")]

            public DateOnly BirthOfDate { get; set; }
            [Required(ErrorMessage = "هذا الحقل مطلوب")]

            public TimeSpan CheckInTime { get; set; }
            [Required(ErrorMessage = "هذا الحقل مطلوب")]

            public TimeSpan CheckOutTime { get; set; }
            public IFormFile? GraduationCertificateFile { get; set; }
            public IFormFile? IdentityFile { get; set; }
            public IFormFile? PersonalImageFile { get; set; }

            public SalaryFormula SalaryFormula { get; set; }//حسبة المرتب

            public ExperienceLevel ExperienceLevel { get; set; }
            public Gender Gender { get; set; }
            public MaritalStatus MaritalStatus { get; set; }
            [Required(ErrorMessage = "هذا الحقل مطلوب")]

            public DateOnly DateOfAppointment { get; set; } // Date of Appointment
            public Governorate Governorate { get; set; } // Date of Appointment

            public JobPosition JobPosition { get; set; } // Date of Appointment


            public int DepartmentId { get; set; } // Foreign Key
        }
        public class EmployeeDtoForShow
        {
            public int Code { get; set; }
            public string Name { get; set; }
            public string Address { get; set; } // Date of Appointment
            public string Phone { get; set; } // Date of Appointment
            public float GrossSalary { get; set; }//اجمالى المرتب
            public string Age { get; set; }
            public string CheckInTime { get; set; }
            public string CheckOutTime { get; set; }
            public string GraduationCertificateUrl { get; set; }
            public string IdentityUrl { get; set; }
            public string PersonalImageUrl { get; set; }

            public SalaryFormula SalaryFormula { get; set; }//حسبة المرتب

            public ExperienceLevel ExperienceLevel { get; set; }
            public Gender Gender { get; set; }
            public MaritalStatus MaritalStatus { get; set; }
            public string DateOfAppointment { get; set; } // Date of Appointment
            public Governorate Governorate { get; set; } // Date of Appointment

            public JobPosition JobPosition { get; set; } // Date of Appointment


            public string DepartmentName { get; set; } // Foreign Key
        }
    }
}
