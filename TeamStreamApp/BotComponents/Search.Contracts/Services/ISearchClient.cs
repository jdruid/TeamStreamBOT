using System.Threading.Tasks;
using TeamStreamApp.BotComponents.Search.Contracts.Models;

namespace TeamStreamApp.BotComponents.Search.Contracts.Services
{
    public interface ISearchClient
    {
        Task<GenericSearchResults> SearchAsync(SearchQueryBuilder queryBuilder, string refiner = null);
    }
}