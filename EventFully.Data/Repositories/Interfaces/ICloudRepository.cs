using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Repositories.Interfaces
{
    public interface ICloudRepository
    {
        //Task<CloudBlobContainer> GetContainer(string containerName);
        //Task<string> UploadBlob(IFormFile file, CloudBlobContainer container, string blobName);
        //Task<string> UploadBlob(System.IO.MemoryStream file, string contentType, CloudBlobContainer container, string blobName);
        //Task<string> UploadBlob(string base64ImageString, string contentType, CloudBlobContainer container, string blobName);
        //Task<string> DeleteBlob(CloudBlobContainer container, string blobName);
        Task<string> UploadFile(string slimString, string fileName);
    }
}
