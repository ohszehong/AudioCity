using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCity
{
    public class ConfigureAudioCityAzureBlob
    {
        //use this function to get blob container reference
        public static CloudBlobContainer GetBlobContainerInformation()
        {
            //1.1 link with the appsettings.json file to read content
            //need to use Microsoft.Extensions.Configuration library and System.IO library

            var Builder = new ConfigurationBuilder().SetBasePath(Directory
                .GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration Configure = Builder.Build();

            //1.2 get the access connection string so that your app is able to link to the correct storage
            CloudStorageAccount AccountDetails = CloudStorageAccount.Parse(Configure["ConnectionStrings:AudioCityAccountStorageConnection"]);

            //1.3 create client object to refer to the correct container 
            CloudBlobClient ClientAgent = AccountDetails.CreateCloudBlobClient();
            CloudBlobContainer Container = ClientAgent.GetContainerReference("gigblob"); //no capital letters are allowed
                                                                                         //^ this line is only to get the reference, havent create the container yet 

            return Container;
        }

        public static void CreateBlobContainer()
        {
           CloudBlobContainer Container =  GetBlobContainerInformation();
           Container.CreateIfNotExistsAsync();
        }

    }
}
