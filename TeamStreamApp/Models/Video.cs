using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamStreamApp.Models
{
    public class Video
    {
        public Guid Id { get; set; }
        public string VideoId { get; set; }
        public string VideoName { get; set; }
        public string VideoUrl { get; set; }
        public string VideoKeywords { get; set; }
        public string VideoThumbnail { get; set; }
        public DateTime VideoDate { get; set; }
    }
}