using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources.Application.FileServices
{
    public interface IFileService
    {
        public string UploadImage(IFormFile file, string FolderName);

    }
}