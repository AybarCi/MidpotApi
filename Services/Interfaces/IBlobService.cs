using System;
using System.IO;
using System.Threading.Tasks;

namespace DatingWeb.Services.Interfaces
{
    public interface IBlobService
    {
        Task<Uri> UploadFileBlobAsync(string blobContainerName, Stream content, string contentType, string fileName);
    }
}
