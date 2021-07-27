using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity.Controllers
{
    public class DownloadContentController : Controller
    {
        public async Task<IActionResult> DownloadFile(string ContentFilePath)
        {
            //download file from blob storage
            CloudBlobContainer Container = ConfigureAudioCityAzureBlob.GetBlobContainerInformation();
            CloudBlockBlob Content = Container.GetBlockBlobReference(ContentFilePath);

            try
            {
                await using (MemoryStream memory = new MemoryStream())
                {
                    await Content.DownloadToStreamAsync(memory);
                }
                Stream output = Content.OpenReadAsync().Result;
                return File(output, Content.Properties.ContentType, Content.Name);

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occured when downloading content from blob storage: ", Ex.ToString());
            }

            //return to previous page
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
