using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleCustomerCare.Styles
{
    [Serializable]
    public class BBCustomerServiceStyle: PromptStyler
    {
        public override void Apply(ref IMessageActivity message, string prompt)
        {

            base.Apply(ref message, prompt);
        }
       
    }

    [Serializable]
    public class FiberNetCustomServiceStyle : PromptStyler
    {
        public override void Apply(ref IMessageActivity message, string prompt)
        {
            ReceiptCard receiptCardMen = new ReceiptCard
            {
                Title = "Rate card For Men",
                //    Facts = new List<Fact> { new Fact("Order Number", "1234"), new Fact("Payment Method", "VISA 5555-****") },
                Items = new List<ReceiptItem>
                        {
                            new ReceiptItem("Shirt", price: "Rs 38.45", quantity: "1"),
                            new ReceiptItem("T-Shirt", price: "Rs 45.00", quantity: "1"),
                            new ReceiptItem("Trouser", price: "Rs 38.45", quantity: "1"),
                            new ReceiptItem("Jeans", price: "Rs 60.00", quantity: "1"),
                            new ReceiptItem("Blazer-Only DryCleaning", price: "Rs 190.45", quantity: "1"),
                            new ReceiptItem("Safari Suit", price: "Rs 200.00", quantity: "1"),
                        },
              
            };

            var receiptWomenCard = new ReceiptCard
            {
                Title = "Rate card For Women",
                //    Facts = new List<Fact> { new Fact("Order Number", "1234"), new Fact("Payment Method", "VISA 5555-****") },
                Items = new List<ReceiptItem>
                        {
                            new ReceiptItem("Top Shirt/Kurta", price: "Rs 38.45", quantity: "1"),
                            new ReceiptItem("Slacks/Pants", price: "Rs 45.00", quantity: "1"),
                            new ReceiptItem("Jeans", price: "Rs 38.45", quantity: "1"),
                            new ReceiptItem("Shorts", price: "Rs 60.00", quantity: "1"),
                            new ReceiptItem("Saree (Cotton with Starch)", price: "Rs 190.45", quantity: "1"),
                            new ReceiptItem("Dhoti/Lungi", price: "Rs 200.00", quantity: "1"),
                        },
                
            };


            List<Attachment> mainPage = new List<Attachment>();
            mainPage.Add(receiptCardMen.ToAttachment());
            mainPage.Add(receiptWomenCard.ToAttachment());          
            message.AttachmentLayout = AttachmentLayoutTypes.List;
            message.Attachments = mainPage;
            message.Text = prompt;
            base.Apply(ref message, prompt);
        }

    }
}