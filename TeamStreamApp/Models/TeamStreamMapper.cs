using System.Linq;
using Microsoft.Azure.Search.Models;
using System;
using TeamStreamApp.BotComponents.Search.Azure.Services;
using TeamStreamApp.BotComponents.Search.Contracts.Models;

namespace TeamStreamApp
{
    public class TeamStreamMapper : IMapper<DocumentSearchResult, GenericSearchResults>
    {
        public GenericSearchResults Map(DocumentSearchResult item)
        {
            var searchResult = new GenericSearchResults();

            searchResult.Results = item.Results.Select(r => ToSearchHit(r)).ToList();
            searchResult.Facets = item.Facets?.ToDictionary(kv => kv.Key, kv => kv.Value.Select(f => ToFacet(f)));

            return searchResult;
        }

        private static GenericFacet ToFacet(FacetResult facetResult)
        {
            return new GenericFacet
            {
                Value = facetResult.Value,
                Count = facetResult.Count.Value
            };
        }

        private static SearchHit ToSearchHit(SearchResult searchResult)
        {
            var searchHit = new SearchHit
            {
                Id = (string)searchResult.Document["Id"],
                Name = (string)searchResult.Document["Name"],
                ThumbnailUrl = (string)searchResult.Document["thumbnailUrl"],
                Text = ((string)searchResult.Document["Text"]),
                Keywords = ((string)searchResult.Document["Keywords"]),
                Tags = ((string)searchResult.Document["tags"]),
                RawUrl = ((string)searchResult.Document["RawUrl"])
            };
            
            return searchHit;
        }
        
    }
}