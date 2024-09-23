using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Domain.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateOnly? UpdatedAt { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
