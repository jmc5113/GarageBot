using System;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Text;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Greeting" with the name of your newly created intent in the following handler
        [LuisIntent("HomeAutomation.TurnOn")]
        public async Task HomeAutomationTurnOnIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("HomeAutomation.TurnOff")]
        public async Task HomeAutomationTurnOffIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Help")]
        public async Task HelpIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        // Entities found in result
        private string BotEntityRecognition(LuisResult result)
        {
            StringBuilder entityResults = new StringBuilder();

            if (result.Entities.Count > 0)
            {
                foreach (EntityRecommendation item in result.Entities)
                {
                    // Query: Turn on the [light]
                    // item.Type = "HomeAutomation.Device"
                    // item.Entity = "light"
                    entityResults.Append(item.Type + "=" + item.Entity + ",");
                }
                // remove last comma
                entityResults.Remove(entityResults.Length - 1, 1);
            }

            return entityResults.ToString();
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        {
            // get recognized entities
            string entities = this.BotEntityRecognition(result);

            // round number
            string roundedScore = result.Intents[0].Score != null ? (Math.Round(result.Intents[0].Score.Value, 2).ToString()) : "0";

            await context.PostAsync($"**Query**: {result.Query}, **Intent**: {result.Intents[0].Intent}, **Score**: {roundedScore}. **Entities**: {entities}");
            context.Wait(MessageReceived);
        }
    }
}