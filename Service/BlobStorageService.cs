using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Core;

namespace ReferralSystem.Service
{
    public class BlobStorageService
    {
        string accessKey = string.Empty;
        private readonly IConfiguration _configuration;
        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string UploadFileToBlob(string strFileName, IFormFile fileData, string fileMimeType)
        {
            try
            {

                var _task = Task.Run(() => this.UploadFileToBlobAsync(strFileName, fileData, fileMimeType));
                _task.Wait();
                string fileUrl = _task.Result;
                return fileUrl;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }



        private string GenerateFileName(string fileName)
        {
            string strFileName = string.Empty;
            string[] strName = fileName.Split('.');
            strFileName = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/" + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "." + strName[strName.Length - 1];
            return strFileName;
        }

        private async Task<string> UploadFileToBlobAsync(string strFileName, IFormFile fileData, string fileMimeType)
        {
            var filePath = Path.GetTempFileName();

            try
            {
                if (fileData.Length > 0)
                {

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await fileData.CopyToAsync(stream);
                    }
                }

                this.accessKey = _configuration.GetSection("AccessKey").Value;

                accessKey =
                    "DefaultEndpointsProtocol=https;AccountName=referraldocuments;AccountKey=ID4sHh6dof/G8x/Qq83WkvhG4H1hYOi9pI1vxpYasXNtXVERREEv2jcZBWOp0dXmv85wEB9lb6gS2hJrCwylqA==;EndpointSuffix=core.windows.net";
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

                string strContainerName = "uploads";
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
                string fileName = this.GenerateFileName(strFileName);


                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                //var blockBlob = cloudBlobContainer.GetBlockBlobReference("mikepic.png");
                //using (var fileStream = System.IO.File.OpenWrite(@"C:\Users\P10433832\Downloads\mikepic-backup.png"))
                //{
                //   await blockBlob.DownloadToStreamAsync(fileStream);
                //}
                fileMimeType = "application/pdf";

                if (fileName != null && fileData != null)
                {
                    var blockBlob = cloudBlobContainer.GetBlockBlobReference("latest.pdf");
                    using (var fileStream = System.IO.File.OpenRead(@filePath))
                    {
                        await blockBlob.UploadFromStreamAsync(fileStream);
                    }

                    //var abc  = blockBlob.Uri.AbsoluteUri;

                    //using (SyncMemoryStream stream1 = new SyncMemoryStream(fileData, 0, fileData.Length))
                    //{
                    //    await blockBlob.UploadFromStreamAsync(stream1);
                    //}


                    //Stream stream = new MemoryStream(fileData);

                    //CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference("bbbbb.pdf");
                    //cloudBlockBlob.Properties.ContentType = fileMimeType;
                    //await cloudBlockBlob.UploadFromStreamAsync(stream);
                    //return cloudBlockBlob.Uri.AbsoluteUri;
                }

                //var blockBlob = cloudBlobContainer.GetBlockBlobReference("Capita ONE Installation Instructions v1.0_Updated.docx");
                //var memStream = new MemoryStream();

                //await blockBlob.DownloadToStreamAsync(memStream);



                //using (var fileStream = System.IO.File.OpenWrite(@"C:\Users\P10433832\Downloads\Capita.docx"))
                //{
                //    await blockBlob.DownloadToStreamAsync(fileStream);
                //}

                return "";
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}

