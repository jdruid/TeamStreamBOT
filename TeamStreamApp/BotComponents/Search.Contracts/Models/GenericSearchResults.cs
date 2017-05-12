using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamStreamApp.BotComponents.Search.Contracts.Models
{
    public class GenericSearchResults
    {
        public IEnumerable<SearchHit> Results { get; set; }

        public IDictionary<string, IEnumerable<GenericFacet>> Facets { get; set; }
    }
}