using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeddingWebsite.Models
{
    public class Rsvp
    {
        public int Id { get; set; }

        [HiddenInput]
        public string UserId { get; set; }

        //[Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Please provide your full name")]
        [DisplayName("Full Name")]
        public string Name { get; set; }

        [Required]
        public bool Attending { get; set; }

        [Required]
        [Range(0, 5)]
        [DisplayName("Total Adults")]
        public int TotalAdults { get; set; }

        [Required]
        [Range(0, 10)]
        [DisplayName("Total Children")]
        public int TotalChildren { get; set; }
    }
}