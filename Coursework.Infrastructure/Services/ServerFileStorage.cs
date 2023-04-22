using System;
using Coursework.Application.Common.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
namespace Coursework.Infrastructure.Services
{
	public class ServerFileStorage:IFileStorage
	{
        private readonly IFileStorage _fileStorage;

        public ServerFileStorage()
        {
        }
        public async Task<string> SaveFileAsync(IFormFile file)
        {
            string startupPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(),"images");
            var fileName = string.Concat(
                Path.GetFileNameWithoutExtension(file.FileName),
                DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Path.GetExtension(file.FileName)
                );
            var filePath = Path.Combine(startupPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}

