using System;
using System.Configuration;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using TeamStreamApp.Search.Models;

namespace TeamStreamApp.Search
{
    public class TeamStreamSearch
    {
        private static ISearchServiceClient _searchClient;
        private static ISearchIndexClient _indexClient;

       
        public static string errorMessage;

        public TeamStreamSearch(string index)
        {
            //index names
            //captions
            //keywords
            //tags

            try
            {
                string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
                string apiKey = ConfigurationManager.AppSettings["SearchServiceApiKey"];

                // Create an HTTP reference to the catalog index
                _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
                _indexClient = _searchClient.Indexes.GetClient(index);
            }
            catch (Exception e)
            {
                errorMessage = e.Message.ToString();
            }
        }
        
        public DocumentSearchResult Search(string searchText)
        {
            // Execute search based on query string
            try
            {
                SearchParameters sp = new SearchParameters() { SearchMode = SearchMode.All };
                return _indexClient.Documents.Search(searchText, sp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }
            return null;
        }

        public List<KeywordCaptionTagSearchResults> SearchDocumentsKeywordsCaptionTags(string searchText)
        {
            List<KeywordCaptionTagSearchResults> results = new List<KeywordCaptionTagSearchResults>();
            try
            {
                SearchParameters sp = new SearchParameters() { SearchMode = SearchMode.All, Top = 1000 };
                DocumentSearchResult<KeywordCaptionTagSearchResults> searchResults = _indexClient.Documents.Search<KeywordCaptionTagSearchResults>(searchText, sp);

                foreach (SearchResult<KeywordCaptionTagSearchResults> result in searchResults.Results)
                {
                    results.Add(new KeywordCaptionTagSearchResults
                    {
                        Id = result.Document.Id,
                        Name = result.Document.Name,
                        Keywords = result.Document.Keywords,
                        RawUrl = result.Document.RawUrl,
                        Tags = result.Document.Tags,
                        ThumbnailUrl = result.Document.ThumbnailUrl,
                        Text = result.Document.Text
                    });
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }

            return null;

        }

        public List<KeywordCaptionSearchResults> SearchDocumentsKeywordsCaption(string searchText)
        {
            List<KeywordCaptionSearchResults> results = new List<KeywordCaptionSearchResults>();
            try
            {
                SearchParameters sp = new SearchParameters() { SearchMode = SearchMode.All, Top = 1000 };
                DocumentSearchResult<KeywordCaptionSearchResults> searchResults = _indexClient.Documents.Search<KeywordCaptionSearchResults>(searchText, sp);

                foreach (SearchResult<KeywordCaptionSearchResults> result in searchResults.Results)
                {
                    results.Add(new KeywordCaptionSearchResults
                    {
                        Id = result.Document.Id,
                        Name = result.Document.Name,
                        Keywords = result.Document.Keywords,
                        RawUrl = result.Document.RawUrl,
                        ThumbnailUrl = result.Document.ThumbnailUrl,
                        ThumbnailIndex = result.Document.ThumbnailIndex,
                        Text = result.Document.Text
                    });
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }

            return null;

        }

        public List<KeywordSearchResults> SearchDocumentsKeywords(string searchText)
        {
            List<KeywordSearchResults> results = new List<KeywordSearchResults>();
            try
            {
                SearchParameters sp = new SearchParameters() { SearchMode = SearchMode.All, Top = 1000 };
                DocumentSearchResult<KeywordSearchResults> searchResults = _indexClient.Documents.Search<KeywordSearchResults>(searchText, sp);

                foreach (SearchResult<KeywordSearchResults> result in searchResults.Results)
                {
                    results.Add(new KeywordSearchResults
                    {
                        Id = result.Document.Id,
                        Name = result.Document.Name,
                        Keywords = result.Document.Keywords,
                        RawUrl = result.Document.RawUrl
                    });
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error querying index: {0}\r\n", ex.Message.ToString());
            }

            return null;

        }
        
    }
}
