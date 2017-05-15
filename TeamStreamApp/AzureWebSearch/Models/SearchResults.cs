using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamStreamApp.Search.Models
{
    public class KeywordSearchResults
    {
        public Guid Id { get; set; }
       // public Guid VideoId { get; set; }
        public string Name { get; set; }
        public string Keywords { get; set; }
        public string RawUrl { get; set; }
    }

    public class KeywordCaptionSearchResults : KeywordSearchResults
    {
        public string Text { get; set; }
        public int ThumbnailIndex { get; set; }
        public string ThumbnailUrl { get; set; }
    }

    public class KeywordCaptionTagSearchResults : KeywordSearchResults
    {
        public string ThumbnailUrl { get; set; }
        public string Tags { get; set; }
        public string Text { get; set; }
    }
}