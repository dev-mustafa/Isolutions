using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ISolutions.Portal.Models
{
    public class TrainingEvent
    {
        public int Id { get; set; }
        public int Duration { get; set; }
        public DateTime Date { get; set; }
        public bool IsReady { get; set; }
        [Display(Name = "City")]
        public int CityId { get; set; }
        [Display(Name = "Course")]
        public int CourseId { get; set; }
        public virtual City City { get; set; }
        public virtual Course Course { get; set; }
    }
}