using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using LaundryServiceBot.Resources;
using SampleCustomerCare.Helper;
using Microsoft.Bot.Connector;
using SampleCustomerCare.Styles;

namespace LaundryServiceBot.Dialogs
{
    [Serializable]
    public class RateCardDialog : IDialog<string>
    {
        protected readonly PromptStyler HitStyler;
        public async Task StartAsync(IDialogContext context)
        {
            await this.PostPromptRateMessageAsync(context);
        }
        public RateCardDialog()
        {
            HitStyler = new FiberNetCustomServiceStyle();
        }
        private async Task PostPromptRateMessageAsync(IDialogContext context)
        {
            var message = context.MakeMessage();

            this.HitStyler.Apply(
                      ref message,
                      "RATE CARD");

            await context.PostAsync(message);

            PromptDialog.Confirm(context, this.ShouldContinueSearching,"Do You Want to Request for Laundry!" );
        }
        protected virtual async Task ShouldContinueSearching(IDialogContext context, IAwaitable<bool> input)
        {
            bool shouldContinue = await input;
            if (shouldContinue)
                context.Done<object>(null);
            else
                await context.PostAsync("T");
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            bool success = false;
            var message = await argument;            

        }
    }
}