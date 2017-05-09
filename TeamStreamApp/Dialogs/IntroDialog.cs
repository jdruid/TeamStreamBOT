using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace TeamStreamApp.Dialogs
{
    [Serializable]
    public class IntroDialog : IDialog<IMessageActivity>
    {   
        string name;
       
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(ConversationStartedAsync);
        }

        public async Task ConversationStartedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            IMessageActivity message = await argument;
            //await context.PostAsync(message.Text);

            PromptDialog.Text(
                context: context,
                resume: ResumeAndPromptDateRangeAsync,
                prompt: "Hi there! I am the TeamStream Bot. What type of videos are you looking for?",
                retry: "I didn't understand. Please try again.");
        }

        public async Task ResumeAndPromptDateRangeAsync(IDialogContext context, IAwaitable<string> argument)
        {
            name = await argument;

            PromptDialog.Choice(
                context: context,
                resume: ResumeAndHandlePromptDateRangeAsync,
                options: Enum.GetValues(typeof(DateRange)).Cast<DateRange>().ToArray(),
                prompt: $"Ok. Do you have a date range in mind?",
                retry: "I didn't understand. Please try again.");
        }
        
        public async Task ResumeAndHandlePromptDateRangeAsync(IDialogContext context, IAwaitable<DateRange> argument)
        {
            var daterange = await argument;

            if (daterange.Equals("None"))
                await context.PostAsync("Ok, but that might take a while. Please wait...");
            else
                await context.PostAsync("Ok. I will start looking.");

            context.Wait(ConversationStartedAsync);
        }

        public enum DateRange
        {
            Over1Year,
            WithinAYear,
            PastFewMonths,
            None
        }


    }
}