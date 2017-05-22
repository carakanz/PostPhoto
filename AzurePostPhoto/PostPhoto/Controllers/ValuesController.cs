using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Swashbuckle.Swagger.Annotations;
using System.Drawing;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;

namespace PostPhoto.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [SwaggerOperation("GetAll")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [HttpPost()]
        public async Task<HttpResponseMessage> Post()
        {
            // Ошибка если не MimeMultipartContent
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            MultipartMemoryStreamProvider streamProvider = new MultipartMemoryStreamProvider();
            try
            {
                DateTime time = DateTime.Now;
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                Random random = new Random();
                ImageWorker image;
                Size size;
                string type;
                // Читаем содержимое MIME multipart с помощью созданного нами провайдера потоков.
                await Request.Content.ReadAsMultipartAsync(streamProvider);
                List<List<int>> sizeImage = new List<List<int>>();
                // Выполняем для каждого элемента в streamProvider
                foreach (HttpContent file in streamProvider.Contents)
                {
                    // Получаем размеры изображения. Если это не изображение возвращаем ошибку;
                    try
                    {
                        image = new ImageWorker(file.ReadAsStreamAsync().Result);
                        size = image.GetSize();
                        type = image.Type().Split('/')[1];
                        file.ReadAsStreamAsync().Result.Position = 0;
                        List<int> tempList = new List<int>();
                        tempList.Add(size.Width);
                        tempList.Add(size.Height);
                        sizeImage.Add(tempList);
                    }
                    catch (Exception e)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, e);
                    }
                    // Обнуляем полицию указателя в Stream
                    file.ReadAsStreamAsync().Result.Position = 0;
                    // Генерируем имя файла
                    string fileName = string.Format("{0}.{1}.{2}:{3}.{4}:{5}.{6}-{7}.{8}",
                        time.Year, time.Month.ToString("D2"), time.Day.ToString("D2"), time.Hour.ToString("D2"), time.Minute.ToString("D2"), time.Second.ToString("D2"), time.Millisecond.ToString("D3")
                        , new string(Enumerable.Repeat(chars, 20)
                      .Select(s => s[random.Next(s.Length)]).ToArray()), type);
                    // Записываем результат в таблицу и сохраняем изображение
                    TableRecord.Record(fileName, size.Width, size.Height, time);
                    ImageStorage.Record(file.ReadAsStreamAsync().Result, fileName);
                }
                // Send OK Response along with saved file names to the client. 
                Trace.TraceError("Can not write to image store. palach");
                return Request.CreateResponse(HttpStatusCode.OK, sizeImage);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // PUT api/values/5
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Delete(int id)
        {
        }
    }
}
