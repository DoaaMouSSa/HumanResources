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
            Male = 1,       
            Female = 2, 
        }
        public enum SalaryFormula
        {
            full = 1,       //30
            traditional = 2,   
        }
        public enum ExperienceLevel
        {
            Beginner = 1,       // 1 year or less
            Intermediate = 2,   // 1-3 years
            Advanced = 3,       // 3-5 years
            Expert = 4          // 5+ years
        }
        public enum MaritalStatus
        {
            Single,
            Married,
            Separated,
        }
        public enum Governorate
        {
            Cairo,
            Alexandria,
            Giza,
            Qalyubia,
            Gharbia,
            Sharqia,
            Dakahlia,
            Minya,
            Asyut,
            Sohag,
            Qena,
            Luxor,
            Aswan,
            RedSea,
            Matrouh,
            NorthSinai,
            SouthSinai,
            Suez,
            PortSaid,
            Ismailia,
            BeniSuef,
            NewValley,
            Damietta
        }
        public enum JobPosition
        {
            Intern,
            TeamLead,
            ProjectManager,
            ProductOwner,
            QualityAssurance,
            BusinessAnalyst,
            DevOpsEngineer,
            SystemsArchitect,
            DatabaseAdministrator,
            NetworkAdministrator,
            HumanResources,
            MarketingSpecialist,
            SalesRepresentative,
            CustomerSupport
        }
    }
    }
