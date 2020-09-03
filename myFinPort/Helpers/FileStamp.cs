using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace myFinPort.Helpers
{
    public class FileStamp
    {
        public static string MakeUnique(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName);
            fileName = SlugMaker.MakeSlug(fileName);
            fileName = $"{fileName}{DateTime.Now.Ticks}{extension}";
            return fileName;
        }
    }
}