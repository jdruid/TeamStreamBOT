﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamStreamApp.Models
{
    
    public class Video
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string RawUrl { get; set; }
        public string Keywords { get; set; }
        public DateTime Date { get; set; }        
        //public List<VisionAnalysis> VisionAnalysis { get; set; }
    }

    public class Caption
    {
        public Guid videoId { get; set; }
        public int thumbnailIndex { get; set; }
        public string text { get; set; }
        public double confidence { get; set; }        
    }

    public class Description
    {
        public Guid videoId { get; set; }
        public int thumbnailIndex { get; set; }
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