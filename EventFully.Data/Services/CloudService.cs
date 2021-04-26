using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Http;
using EventFully.Services.Interfaces;
using EventFully.Repositories.Interfaces;

namespace EventFully.Services
{
    public class CloudService : ICloudService
    {
        private readonly ICloudRepository _cloudRepository;
        public CloudService(ICloudRepository cloudRepository)
        {
            _cloudRepository = cloudRepository;
        }

        //public async Task<CloudBlobContainer> GetContainer(string containerName)
        //{
        //    return await _cloudRepository.GetContainer(containerName);
        //}

        //public async Task<string> UploadBlob(IFormFile file, CloudBlobContainer container, string blobName)
        //{
        //    return await _cloudRepository.UploadBlob(file, container, blobName);
        //}
        //public async Task<string> UploadBlob(System.IO.MemoryStream file, string contentType, CloudBlobContainer container, string blobName)
        //{
        //    return await _cloudRepository.UploadBlob(file, contentType, container, blobName);
        //}

        //public async Task<string> DeleteBlob(CloudBlobContainer container, string blobName)
        //{
        //    return await _cloudRepository.DeleteBlob(container, blobName);
        //}

        //public async Task<string> UploadBlob(string base64ImageString, string contentType, CloudBlobContainer container, string blobName)
        //{
        //    return await _cloudRepository.UploadBlob(base64ImageString, contentType, container, blobName);
        //}

        public async Task<string> UploadFile(string slimString, string fileName)
        {
            return await _cloudRepository.UploadFile(slimString, fileName);
        }
    }
}
