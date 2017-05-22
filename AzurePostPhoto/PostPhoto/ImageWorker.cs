using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace PostPhoto
{
    public class ImageWorker //класс для получения параметров изображения
    {
        protected Bitmap image;
        public ImageWorker(Stream file)
        {
            image = new Bitmap(file);
        }

        public Size GetSize() // получить размер
        {
            return image.Size;
        }
        public string Type() // получить тип
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == image.RawFormat.Guid)
                    return codec.MimeType;
            }
            return "image/unknown";
        }
    }
}