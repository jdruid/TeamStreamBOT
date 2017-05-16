using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamStreamFunctions.Models
{
    //Classes
    public class Caption
    {
        public string text { get; set; }
        public double confidence { get; set; }
    }

    public class Description
    {
        public List<string> tags { get; set; }
        public List<Caption> captions { get; set; }
    }

    public class Metadata
    {
        public int width { get; set; }
        public int height { get; set; }
        public string format { get; set; }
    }

    public class VisionAnalysis
    {
        public Guid id { get; set; }
        public Guid videoId { get; set; }
        public string requestId { get; set; }
        public string thumbnailUrl { get; set; }
        public int thumbnailIndex { get; set; }
        public int thumbnailCount { get; set; }
        public Description description { get; set; }
        public Metadata metadata { get; set; }
    }
}