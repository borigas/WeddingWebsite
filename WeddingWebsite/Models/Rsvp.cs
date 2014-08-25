using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeddingWebsite.Models
{
    public class Rsvp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Attending { get; set; }
        public int TotalAdults { get; set; }
        public int TotalChildren { get; set; }
    }
}