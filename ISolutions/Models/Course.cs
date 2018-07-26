using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISolutions.Portal.Models
{
    public class Course
    {
        public Course()
        {
            Events = new List<TrainingEvent>();
        }
        public int Id { get; set; }
        public int Code { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual List<TrainingEvent> Events { get; set; }
    }
}