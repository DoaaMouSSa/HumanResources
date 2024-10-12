using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HumanResources.Application.FileServices
{

    public class FileService:IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public string UploadImage(IFormFile file, string FolderName)
        {
            try
            {
                if (file != null)
                {
                    if (!Directory.Exists(_webHostEnvironment.WebRootPath + "\\images\\" + FolderName))
                    {
                        Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "\\images\\" + FolderName);
                    }
                    string path = _webHostEnvironment.WebRootPath + "\\images\\" + FolderName + "\\";
                    string imagePath = path + file.FileName;
                    using (FileStream fileStream = System.IO.File.Create(imagePath))
                    {
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                    }

                    return file.FileName;
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

    }
}