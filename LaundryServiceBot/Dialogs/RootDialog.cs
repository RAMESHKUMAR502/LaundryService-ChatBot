using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using LaundryServiceBot.Resources;
using SampleCustomerCare.Helper;

namespace LaundryServiceBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private ResumptionCookie resumptionCookie;
        protected readonly PromptStyler HitStyler;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public RootDialog()
        {

        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (this.resumptionCookie == null)
            {
                this.resumptionCookie = new ResumptionCookie(message);
            }

            await this.PostPromptInputMessageAsync(context);
        }

        private async Task PostPromptInputMessageAsync(IDialogContext context)
        {

            var reply = context.MakeMessage();

            var options = new[]
            {
                Resource.BtnBookService,
                Resource.BtnRatecard
            };
            reply.AddHeroCard(
                Resource.HeadingTitle,
                Resource.SubTitle,
                options,
                new[] { "http://www.wassupondemand.com/assets/images/png/laundry.png" });

            await context.PostAsync(reply);

            context.Wait(this.ActOnSearchResults);
        }

        private async Task ActOnSearchResults(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if(message.Text == Resources.Resource.BtnBookService)
            {
                context.Call(new DateTimeInputDialog(), this.AfterDeliveryAddress);
            }
            else if(message.Text == Resources.Resource.BtnRatecard)
            {
                context.Call(new RateCardDialog(), this.AfterDeliveryAddress);
            }       
            
        }


        private async Task AfterDeliveryAddress(IDialogContext context, IAwaitable<string> result)
        {
            await this.PostPromptInputMessageAsync(context);
        }
    }
}