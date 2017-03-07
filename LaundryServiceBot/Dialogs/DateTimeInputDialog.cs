using LaundryServiceBot.Dialogs;
using LaundryServiceBot.Resources;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SampleCustomerCare.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace LaundryServiceBot.Dialogs
{
    [Serializable]
    public class DateTimeInputDialog : IDialog<string>
    {
        /// <summary>
        /// If it is null, we're looking for DateTimeFrom
        /// Else, we're looking for DateTimeTo
        /// </summary>
     
        string selectedDateString, selectedTimeString;
        string MobileNum, EmailId,Address;
        public ICustomInputValidator Validator { get; private set; }
        public DateTimeInputDialog()
        {
               
        }

        public async Task StartAsync(IDialogContext context)
        {
            await this.PostPromptDateInputMessageAsync(context);            
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            bool success = false;
            var message = await argument;
            if (message.Text.ToLower().Equals("today")) //"today" as input
            {
                success = true;
                var date = DateTime.UtcNow.Date.ToString("dd/MM/yyy");
                selectedDateString = date;
            }
            else if (message.Text.ToLower().Equals("tomorrow"))//"tomorrow" as input
            {
                success = true;
                var date = DateTime.UtcNow.Date.AddDays(1).ToString("dd/MM/yyy");
                selectedDateString = date;               
            }
            else if (IsValidDateTimeTest(message.Text))
            {
                var selectedDate = DateTime.Parse(message.Text); //(, null);
                success = true;
                selectedDateString = message.Text;
              
            }
            else //no valid input
            {
                success = false;
                await context.PostAsync("Worng Date Input..Please enter DD-MM-YYYY:HH:MM for EG 12-01-2016:12:20");
            }

            if (success)
            {
                string msg = Resource.TimeHeading;
                await context.PostAsync(msg);
                context.Wait(MobileNumber);
            }
            else
            {

                await PostPromptDateInputMessageAsync(context);
                context.Wait(MessageReceivedAsync);
            }

        }

        public bool IsValidDateTimeTest(string dateTime)
        {
            string[] formats = { "dd/MM/yyyy" };
            DateTime parsedDateTime;
            return DateTime.TryParseExact(dateTime, formats, new CultureInfo("en-US"),
                                           DateTimeStyles.None, out parsedDateTime);
        }
        public virtual async Task TimeSelected(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            //if (IsValidDateTimeTest(message.Text))
            {
              
                //await context.PostAsync(message);
               // context.Wait(this.MobileNumber);
              
             }
            
        }

      
     

        private async Task PostPromptTimeInputMessageAsync(IDialogContext context)
        {
            string msg = Resource.TimeHeading;
            await context.PostAsync(msg);
            context.Wait(TimeSelected);
        }

        private async Task PostPromptDateInputMessageAsync(IDialogContext context)
        {
            string msg = Resource.DateSelction;

            var reply = context.MakeMessage();

            var options = new[]
           {
                Resource.TxtToday,
                Resource.TxtTommrow
            };
            reply.AddHeroCard(
                Resource.DateHeading,
                Resource.DateSelction,
                options,
                new[] { "http://www.wassupondemand.com/assets/images/png/laundry.png" });

            await context.PostAsync(reply);         
            context.Wait(this.MessageReceivedAsync);
        }

   
        public virtual async Task MobileNumber(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
           {
                selectedTimeString = message.Text;
                selectedDateString = "Selected date for PickUp " + selectedDateString + ":" + selectedTimeString;
                message.Text = selectedDateString;
                Validator = new PhoneCustomInputValidator();
                await context.PostAsync("Please Enter MobileNumber");
                context.Wait(InputGiven);
              

            }

        }

        public async Task InputGiven(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            string msg = message.Text.Trim();
            if (!Validator.IsValid(msg))
            {
                await context.PostAsync("The MobileNumber Entered is worng!");
                context.Wait(InputGiven);
            }
            else
            {
                MobileNum = message.Text.Trim();
                Validator = new EmailCustomInputValidator();
                await context.PostAsync("Please Enter Email ID");
                context.Wait(Email);
            }
        }

        private async Task Email(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {

            var message = await argument;
            string msg = message.Text.Trim();
            if (!Validator.IsValid(msg))
            {
                await context.PostAsync("The MobileNumber Entered is worng!");
                context.Wait(InputGiven);
            }
            else
            {
                EmailId = message.Text.Trim();
                string Text = "Please Enter the Complete Address along with Pin";
              
                await context.PostAsync(Text);
                context.Wait(AddressPopup);

            }

        }


        private async Task AddressPopup(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {

            var message = await argument;
            string msg = message.Text.Trim();
            Address = message.Text.Trim();

            string Text = "Please find the Details \n\n" + Address +" " + MobileNum + " " + EmailId + " " + selectedDateString + " " + "\n\n"
                + "Please Press Yes If You Want to Raise the Request for Laundury?";
                                                                                                                                                            // "2. Image Link: ![duck](http://aka.ms/Fo983c)";
            PromptDialog.Confirm(context, this.ShouldContinueSearching, Text);

         }

        protected virtual async Task ShouldContinueSearching(IDialogContext context, IAwaitable<bool> input)
        {
            try
            {
                bool shouldContinue = await input;
                if (shouldContinue)
                {
                    string Text = "Your Request has been Rasied \n\n" +
                        "Your PickUp has been Scheduled"+
                        "Please note Your Request ID LAU20022017CUMERID \n\n" +
                         "Image Link: ![WASSUP](http://www.wassupondemand.com/) \n\n"; // +
                                                                                                                                                 // "2. Image Link: ![duck](http://aka.ms/Fo983c)";
                    await context.PostAsync(Text);
                }

                context.Done<object>(null);
            }
            catch (TooManyAttemptsException)
            {
                context.Done<object>(null);
            }
        }

    }

}