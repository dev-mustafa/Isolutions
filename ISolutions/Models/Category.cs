using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISolutions.Portal.Models
{
    public class Category
    {
        public Category()
        {
            Courses = new List<Course>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string LargeImage { get; set; }      
        public bool IsReady { get; set; }
        [Column(TypeName = "ntext")]
        [UIHint("tinymce_full"), AllowHtml]
        public string Description { get; set; }

        public virtual List<Slide> Slides { get; set; }
        public virtual List<Course> Courses { get; set; }
    }
}