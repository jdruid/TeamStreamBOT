using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using TeamStreamApp.BotComponents.Search.Contracts.Models;

namespace TeamStreamApp.BotComponents.Search.Dialogs
{
    [Serializable]
    public class SearchHitStyler : PromptStyler
    {
        public override void Apply<T>(ref IMessageActivity message, string prompt, IReadOnlyList<T> options, IReadOnlyList<string> descriptions = null, string speak = null)
        {
            var hits = options as IList<SearchHit>;
            if (hits != null)
            {
                var cards = hits.Select(h => new ThumbnailCard
                {
                    Title = h.Name,
                    Images = new[] { new CardImage(h.ThumbnailUrl) },
                    Buttons = new[] { new CardAction(ActionTypes.ImBack, "Pick this one", value: h.Id) },
                    Text = h.Text
                });

                message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                message.Attachments = cards.Select(c => c.ToAttachment()).ToList();
                message.Text = prompt;
                message.Speak = speak;
            }
            else
            {
                base.Apply<T>(ref message, prompt, options, descriptions, speak);
            }
        }
    }
}
