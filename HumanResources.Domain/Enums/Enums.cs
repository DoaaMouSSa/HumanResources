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
            اسبوعى = 1,       //30
            شهرى_30 = 2,       //30
            شهرى_26 = 3,   
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
        public enum DeductionType
        {
            نقدى,
            ساعات,
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
