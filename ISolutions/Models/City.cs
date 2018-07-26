using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISolutions.Portal.Models
{
    public class City
    {
        public City()
        {
            Events = new List<TrainingEvent>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsReady { get; set; }
        public virtual List<TrainingEvent> Events { get; set; }
    }
}