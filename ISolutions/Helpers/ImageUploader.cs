using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace ISolutions.Portal.Helpers
{
    public class ImageUploader
    {
        public void Upload(HttpPostedFileBase file, string Path)
        {
            if (System.IO.File.Exists(Path))
                System.IO.File.Delete(Path);

            file.SaveAs(Path);
        }
    }
}