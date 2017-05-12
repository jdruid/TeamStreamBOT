using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using TeamStreamApp.BotComponents.Search.Contracts.Services;
using TeamStreamApp.BotComponents.Search.Contracts.Models;

namespace TeamStreamApp.BotComponents.Search.Azure.Services
{
    public class AzureSearchClient : ISearchClient
    {
        private readonly ISearchIndexClient searchClient;
        private readonly IMapper<DocumentSearchResult, GenericSearchResults> mapper;

        public AzureSearchClient(IMapper<DocumentSearchResult, GenericSearchResults> mapper)
        {
            this.mapper = mapper;
            SearchServiceClient client = new SearchServiceClient(
                ConfigurationManager.AppSettings["SearchServiceName"],
                new SearchCredentials(ConfigurationManager.AppSettings["SearchServiceApiKey"]));

            this.searchClient = client.Indexes.GetClient("tags");
        }

        public async Task<GenericSearchResults> SearchAsync(SearchQueryBuilder queryBuilder, string refiner)
        {
            var documentSearchResult = await this.searchClient.Documents.SearchAsync(queryBuilder.SearchText, BuildParameters(queryBuilder, refiner));

            return this.mapper.Map(documentSearchResult);
        }

        private static SearchParameters BuildParameters(SearchQueryBuilder queryBuilder, string facet)
        {
            SearchParameters parameters = new SearchParameters
            {
                Top = queryBuilder.HitsPerPage,
                Skip = queryBuilder.PageNumber * queryBuilder.HitsPerPage,
                SearchMode = SearchMode.All
            };

            if (facet != null)
            {
                parameters.Facets = new List<string> { facet };
            }

            if (queryBuilder.Refinements.Count > 0)
            {
                StringBuilder filter = new StringBuilder();
                string separator = string.Empty;

                foreach (var entry in queryBuilder.Refinements)
                {
                    foreach (string value in entry.Value)
                    {
                        filter.Append(separator);
                        filter.Append($"{entry.Key} eq '{EscapeFilterString(value)}'");
                        separator = " and ";
                    }
                }

                parameters.Filter = filter.ToString();
            }

            return parameters;
        }

        private static string EscapeFilterString(string s)
        {
            return s.Replace("'", "''");
        }
    }
}