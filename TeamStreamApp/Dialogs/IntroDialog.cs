using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Internals.Fibers;
using TeamStreamApp.BotComponents.Search.Contracts.Services;
using TeamStreamApp.BotComponents.Search.Contracts.Models;

namespace TeamStreamApp.Dialogs
{
    [Serializable]
    public class IntroDialog : IDialog<object>
    {
        private ISearchClient searchClient;

        public IntroDialog(ISearchClient searchClient)
        {
            SetField.NotNull(out this.searchClient, nameof(searchClient), searchClient);
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.StartSearchDialog);
            return Task.CompletedTask;
        }

        public Task StartSearchDialog(IDialogContext context, IAwaitable<IMessageActivity> input)
        {
            context.Call(new VideoSearchDialog(this.searchClient), this.Done);
            return Task.CompletedTask;
        }

        public async Task Done(IDialogContext context, IAwaitable<IList<SearchHit>> input)
        {
            var selection = await input;

            var message = context.MakeMessage();

            GetVideoCard(ref message, selection);

            await context.PostAsync(message);

            context.Done<object>(null);
        }

        private static void GetVideoCard(ref IMessageActivity message, IList<SearchHit> selection)
        {
            
            if (selection != null)
            {
                var cards = selection.Select(h => new VideoCard
                {
                    Title = h.Name,
                    Subtitle = h.Text,
                    Image = new ThumbnailUrl
                    {
                        Url = h.ThumbnailUrl
                    },
                    Media = new List<MediaUrl>
                    {
                        new MediaUrl()
                        {
                            Url = h.RawUrl
                        }
                    },
                    Buttons = new[] { new CardAction(ActionTypes.OpenUrl, "Learn More", value: "http://teamstreamproductions.com") },

                });

                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                message.Attachments = cards.Select(c => c.ToAttachment()).ToList();
                //message.Text = prompt;
               // message.Speak = speak;
            }
            
        }
        
    }
    
}
