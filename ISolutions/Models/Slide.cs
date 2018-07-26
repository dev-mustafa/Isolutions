using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISolutions.Portal.Models
{
    public class Slide
    {
        public int Id { get; set; }
        [AllowHtml]
        public string Title { get; set; }
        public string TitleColor { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public string ContentColor { get; set; }
        public string Image { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}