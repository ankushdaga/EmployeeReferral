using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ReferralSystem.Models;
using ReferralSystem.Repository;
using ReferralSystem.Service;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ReferralSystem.Controllers
{
  // [Route("api/[controller]")]
//   [ApiController]
    public class BooksController : Controller
    {
         private readonly IMongoRepository<Books> _bookRepo;

        private readonly IConfiguration _configuration;

        public IActionResult Index()
        {
            return View();
        }


        public BooksController(IMongoRepository<Books> bookRepository , IConfiguration configuration)
        {
            _bookRepo = bookRepository;
            _configuration = configuration;
        }


        [HttpGet]
        public ActionResult<List<Books>> Get()
        {
            var booklist = _bookRepo.Get();
            return booklist;
        }

       

        [HttpPost]
        //[HttpPost("registerPerson")]
        public async Task<IActionResult> AddPerson(Books book)
        {

            //code for uploading blob data to azure
            BlobStorageService objBlobService = new BlobStorageService(_configuration);
            byte[] fileData = new byte[book.File.Length];
            string mimeType1 = book.File.ContentType;

          
          
            book.Author = objBlobService.UploadFileToBlob("ankush.pdf", book.File, mimeType1);



           




            string accessKey =
                "DefaultEndpointsProtocol=https;AccountName=referraldocuments;AccountKey=ID4sHh6dof/G8x/Qq83WkvhG4H1hYOi9pI1vxpYasXNtXVERREEv2jcZBWOp0dXmv85wEB9lb6gS2hJrCwylqA==;EndpointSuffix=core.windows.net";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("uploads");
            var blockBlob = cloudBlobContainer.GetBlobReference("ankush.pdf");
            string fileName = "ankush.pdf";
            MemoryStream memStream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(memStream);


            HttpResponse response = HttpContext.Response;
            //response.ContentType = blockBlob.Properties.ContentType;
            response.ContentType = blockBlob.Properties.ContentType;
            response.Headers.Add("Content-Disposition", "Attachment; filename=" + fileName);
            response.Headers.Add("Content-Length", blockBlob.Properties.Length.ToString());
            return File(memStream.ToArray(), blockBlob.Properties.ContentType, "ankush.pdf");


            //CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference("ankush.pdf");
            //var exists = blob.ExistsAsync(); // to verify if file exist
            //await blob.FetchAttributesAsync();
            //byte[] dataBytes1;
            //var outputStream = new MemoryStream();

            //using (StreamReader blobfilestream = new StreamReader(await blob.OpenReadAsync()))
            //{
            //    dataBytes1 = blobfilestream.CurrentEncoding.GetBytes(blobfilestream.ReadToEnd());
            //    await blob.DownloadToStreamAsync(outputStream);
            //}

            //Byte[] value = BitConverter.GetBytes(dataBytes1.Length - 1);
            //string mimeType = "application/pdf";
            //Response.Headers.Add("Content-Disposition", "inline; filename=" + "filename.pdf");

            //return File(value, mimeType,"abc.pdf");

            //code for triggering mail using azure grid
            //var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
            //var client = new SendGridClient(apiKey);
            //var from = new EmailAddress("ankush.daga@capita.co.uk", "Example User 1");
            //List<EmailAddress> tos = new List<EmailAddress>
            //{
            //    new EmailAddress("pankaj.pawar@capita.co.uk", "Example User 2")
            //};

            //var subject = "Hello world email from Sendgrid ";
            //var htmlContent = "<strong>Hello world with HTML content</strong>";
            //var displayRecipients = false; // set this to true if you want recipients to see each others mail id 
            //var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, "", htmlContent, false);
            //var response = await client.SendEmailAsync(msg);



            //decimal d = 1.23M;
            //var bookdata = new Books()
            //{
            //    BookName = "John",
            //    Price = d
            //};

            //await _bookRepo.InsertOneAsync(bookdata);
        }

        [HttpGet("getPeopleData")]
        public IEnumerable<string> GetPeopleData()
        {
            var people = _bookRepo.FilterBy(
                filter => filter.BookName != "test",
                projection => projection.BookName
            );
            return people;
        }
    }
}
