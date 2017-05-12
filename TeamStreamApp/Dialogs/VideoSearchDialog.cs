using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamStreamApp.BotComponents.Search.Contracts.Services;
using TeamStreamApp.BotComponents.Search.Dialogs;

namespace TeamStreamApp.Dialogs
{
    [Serializable]
    public class VideoSearchDialog : SearchDialog
    {
        private static readonly string[] TopRefiners = { "keywords", "caption", "tags" };

        public VideoSearchDialog(ISearchClient searchClient) : base(searchClient, multipleSelection: true)
        {
        }

        protected override string[] GetTopRefiners()
        {
            return TopRefiners;
        }
    }
}