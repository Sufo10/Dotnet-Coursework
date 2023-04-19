using System;
using Microsoft.AspNetCore.Http;

namespace Coursework.Application.Common.Interface
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(IFormFile file);
    }
}

