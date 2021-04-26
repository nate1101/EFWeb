using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventFully.Models;
using EventFully.Repositories.Interfaces;
using System.Net;
using System.IO;
using Microsoft.Extensions.Primitives;

namespace EventFully.Repositories
{
    public class CloudRepository : ICloudRepository
    {
        //private readonly MascotMediaCommonContext _dbContext;
        //private readonly CloudSettings _cloudSettings;
        public CloudRepository(IOptions<CloudSettings> cloudSettings)
        {
            //_cloudSettings = cloudSettings.Value;
        }

        //public async Task<CloudBlobContainer> GetContainer(string containerName)
        //{
        //    try
        //    {
        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_cloudSettings.AzureStorageConnectionString);
        //        // Create a blob client.
        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //        // Get a reference to the container
        //        CloudBlobContainer container = blobClient.GetContainerReference(containerName);
        //        // if container doesn’t exist, create it.
        //        await container.CreateIfNotExistsAsync();
        //        // set permissions to public
        //        await container.SetPermissionsAsync(new BlobContainerPermissions
        //        {
        //            PublicAccess = BlobContainerPublicAccessType.Blob
        //        });

        //        return container;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task<string> UploadBlob(IFormFile file, CloudBlobContainer container, string blobName)
        //{
        //    try
        //    {
        //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
                
        //        blockBlob.Properties.ContentType = file.ContentType;
        //        // delete the file if it already exists
        //        await blockBlob.DeleteIfExistsAsync();
        //        using (var fileStream = file.OpenReadStream())
        //        {
        //            await blockBlob.UploadFromStreamAsync(fileStream);
        //        }
        //        return blockBlob.Uri.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task<string> UploadBlob(System.IO.MemoryStream file, string contentType, CloudBlobContainer container, string blobName)
        //{
        //    try
        //    {
        //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
        //        blockBlob.Properties.ContentType = contentType;
        //        // delete the file if it already exists
        //        await blockBlob.DeleteIfExistsAsync();
                
        //        await blockBlob.UploadFromStreamAsync(file);
                
        //        return blockBlob.Uri.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task<string> UploadBlob(string base64ImageString, string contentType, CloudBlobContainer container, string blobName)
        //{
        //    try
        //    {
        //        base64ImageString = base64ImageString.Substring(base64ImageString.IndexOf(",") + 1);
        //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
        //        blockBlob.Properties.ContentType = contentType;
        //        // delete the file if it already exists
        //        await blockBlob.DeleteIfExistsAsync();

        //        byte[] imageBytes = Convert.FromBase64String(base64ImageString);

        //        await blockBlob.UploadFromByteArrayAsync(imageBytes, 0, imageBytes.Length);

        //        return blockBlob.Uri.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task<string> DeleteBlob(CloudBlobContainer container, string blobName)
        //{
        //    try
        //    {
        //        CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
        //        // delete the file if it already exists
        //        await blockBlob.DeleteIfExistsAsync();
                
        //        return blobName;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return "";
        //    }
        //}

        public async Task<string> UploadFile(string slimString, string fileName)
        {
            slimString = slimString.Substring(slimString.IndexOf(",") + 1);
            //string imageFileName = String.Format("ftp://ftp.site4now.net/assets/{0}", fileName);
            Uri imageFile = new Uri(String.Format("ftp://208.118.63.229/assets/{0}", fileName));
            var imageBytes = Convert.FromBase64String(slimString);

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(imageFile);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                request.ContentLength = imageBytes.Length;
                request.UsePassive = true;
                request.EnableSsl = false;
                request.Credentials = new NetworkCredential("eventbxadmin-001", "K@j@g00g00##2018");
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(imageBytes, 0, imageBytes.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                    return $"https://assets.eventbx.com/{fileName}";
                }

                //WebClient webClient = new WebClient();
                //webClient.Credentials = new NetworkCredential("eventbxadmin-001", "K@j@g00g00##2018");
                //try
                //{
                //    webClient.UploadDataAsync(imageFile, imageBytes);
                //    //webClient.Dispose();

                //    return $"https://assets.eventbx.com/{fileName}";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
