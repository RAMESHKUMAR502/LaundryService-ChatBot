using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LaundryServiceBot.Dialogs
{
    public sealed class CustomTextInputDialog : IDialog<string>
    {
        private CustomTextInputDialog()
        { }

        public string InputPrompt { get; private set; }
        public string WrongInputPrompt { get; private set; }

        public ICustomInputValidator Validator { get; private set; }

        public static CustomTextInputDialog CreateCustomTextInputDialog
            (string inputPrompt, string wrongInputPrompt, ICustomInputValidator validator)
        {
            return new CustomTextInputDialog()
            { InputPrompt = inputPrompt, WrongInputPrompt = wrongInputPrompt, Validator = validator };
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(InputPrompt);
            context.Wait(InputGiven);
        }

        public async Task InputGiven(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            string msg = message.Text.Trim();
            if (!Validator.IsValid(msg))
            {
                await context.PostAsync(WrongInputPrompt);
                context.Wait(InputGiven);
            }
            else
                context.Done(msg);
        }
    }


    public interface ICustomInputValidator
    {
        bool IsValid(string input);
    }

    [Serializable()]
    public class PhoneCustomInputValidator : ICustomInputValidator
    {
        public bool IsValid(string input)
        {
            input = input.Replace(" ", "");
            input = input.Replace("+", "");
            input = input.Replace("(", "");
            input = input.Replace(")", "");
            if (input.Length > 9)
            {
                long number1 = 0;
                bool canConvert = long.TryParse(input, out number1);
                return canConvert;
            }
            return false;
        }
    }

    [Serializable()]
    public class EmailCustomInputValidator : ICustomInputValidator
    {
        public bool IsValid(string input)
        {          
            return isValidEmail(input);
        }

        public bool isValidEmail(string inputEmail)
        {
            try
            {
                string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                      @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                      @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex re = new Regex(strRegex);
                if (re.IsMatch(inputEmail))
                    return (true);
                else
                    return (false);
            }
            catch (Exception ex)
            {
                return (false);
            }
        }
    }

  

    [Serializable()]
    public class TextCustomInputValidator : ICustomInputValidator
    {

        private int MinLength, MaxLength;
        public TextCustomInputValidator(int minLength, int maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }
        public bool IsValid(string input)
        {
            return input.Length >= MinLength && input.Length <= MaxLength;
        }
    }
}