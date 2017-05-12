using System;
using System.Collections.Generic;

namespace TeamStreamApp.BotComponents.Search.Contracts.Models
{
    [Serializable]
    public class SearchHit
    {
        public SearchHit()
        {
            this.PropertyBag = new Dictionary<string, object>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Keywords { get; set; }

        public string RawUrl { get; set; }

        public string Tags { get; set; }

        public string Text { get; set; }
        public string ThumbnailUrl { get; set; }

        public IDictionary<string, object> PropertyBag { get; set; }
    }
}