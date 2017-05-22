using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using System.Diagnostics;

namespace PostPhoto
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.
            return base.OnStart();
        }
    }

    // Соединение с clous storage 
    public static class CloudContainer
    {
        public static CloudBlobClient BlobClient;
        public static CloudBlobContainer Container;
        public static CloudTableClient CloudTable;
        public static CloudTable Table;
        private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=carakanphoto;AccountKey=uIeJUiutoiZT0qCOk+QAaYsFzpVw96Tb1Y3Ig6BWtcanTM/DmgazjAMpXh7QmHLU4XrJHSuHMa2L8oTIaxADKQ==;EndpointSuffix=core.windows.net";
        public static void connectToStorage() // Соединение с хранилищем фотографий
        {
            Trace.TraceInformation("Connect to storage");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            // Create a blob client.
            BlobClient = storageAccount.CreateCloudBlobClient();
            Container = BlobClient.GetContainerReference("photo");
            Container.CreateIfNotExists();
        }

        public static void connectToTable() // Соединение с таблицей результатов
        {
            Trace.TraceInformation("Connect to table");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTable = storageAccount.CreateCloudTableClient();
            Table = CloudTable.GetTableReference("SizePhoto");
            Table.CreateIfNotExists();
        }
    }
}
