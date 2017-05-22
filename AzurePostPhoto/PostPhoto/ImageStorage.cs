using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PostPhoto
{
    static class ImageStorage
    {
        private struct Option
        {
            public Stream file;
            public string fileName;
        }
        public static void Record(Stream file, string fileName) // сохранение file в cloud storage с именем fileName
        {
            Option option = new Option();
            option.file = file;
            option.fileName = fileName;
            new Thread(RecordThread).Start(option);
        }
        private static void RecordThread(object option) // сохранение файла в cloud storage. Тип option - Option
        {            
            try
            {
                if (CloudContainer.Container == null)
                    CloudContainer.connectToStorage();
                CloudBlockBlob blockBlob = CloudContainer.Container.GetBlockBlobReference(((Option)option).fileName);
                blockBlob.UploadFromStream(((Option)option).file);
            }
            catch (Exception exception)
            {
                Trace.TraceError("Can not write to image store. " + exception);
            }
        }
    }
}