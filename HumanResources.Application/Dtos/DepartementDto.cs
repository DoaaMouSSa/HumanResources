using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.Dtos
{
    public class DepartementDto
    {
        public class DepartementDtoForSelcet
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class DepartmentDtoForAdd
        {
            public string Name { get; set; }
            public string? Description { get; set; }
            public IFormFile? ImageFile { get; set; }
        }
        public class DepartmentDtoForUpdate
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public IFormFile? ImageFile { get; set; }
        }
    }
}
