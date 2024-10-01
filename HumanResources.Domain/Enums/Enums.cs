using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Enums
{
    public class Enums
    {
        public enum Gender
        {
            ذكر = 1,       
            أنثى = 2, 
        }
        public enum SalaryFormula
        {
            كامل = 1,       //30
            تقليدى = 2,   
        }
        public enum ExperienceLevel
        {
            مبتدئ = 1,       // 1 year or less
            متوسط  = 2,   // 1-3 years
            متقدم = 3,       // 3-5 years
            خبير = 4          // 5+ years
        }
        public enum MaritalStatus
        {
            أعزب,
            متزوج,
        }
        public enum Governorate
        {
            القاهرة,
            الاسكندرية,
            الجيزة,
            القليوبيه,
            
        }
        public enum JobPosition
        {
            موظف,
            مدير,

        }
    }
    }
