using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamStreamApp.BotComponents.Search.Contracts.Models
{
    public class GenericFacet
    {
        public object Value { get; set; }

        public long Count { get; set; }
    }
}