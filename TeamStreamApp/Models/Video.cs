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
        public string requestId { get; set; }
        public Metadata metadata { get; set; }
        public List<Description> description { get; set; }
    }

    public class Description
    {
        public List<string> tags { get; set; }
        public List<Caption> captions { get; set; }
    }

    public class Caption
    {
        public string text { get; set; }
        public double confidence { get; set; }
    }   

    public class Metadata
    {
        public int width { get; set; }
        public int height { get; set; }
        public string format { get; set; }
    }

    
}