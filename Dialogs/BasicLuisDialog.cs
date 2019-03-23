using System;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Text;
using Newtonsoft.Json;

using HttpUtils;
using Microsoft.Bot.Connector;

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

        [LuisIntent("HomeAutomation.Status")]
        public async Task HomeAutomationStatus(IDialogContext context, LuisResult result)
        {
            var client = new RestClient();
            client.EndPoint = @"https://api.particle.io/v1/devices/2c0026000f47363336383437/doorStatus?access_token=139a6bbeb6a6463a35a5a06c472d7f32ab8dc9bb";
            client.Method = HttpVerb.GET;
            client.ContentType = "application/x-www-form-urlencoded";
            string status = client.MakeRequest();
            dynamic j = JsonConvert.DeserializeObject(status);
            int doorStatus = j.result;

            if (doorStatus == 0)
            {
                // The door is currently closed
                IMessageActivity response = context.MakeMessage();
                response.Text = "The garage door is currently closed";
                response.Speak = "The garage door is currently closed";
                response.InputHint = InputHints.IgnoringInput;
                await context.PostAsync(response);
            }
            else
            {
                // The door is currently open
                IMessageActivity response = context.MakeMessage();
                response.Text = "The garage door is currently open";
                response.Speak = "The garage door is currently open";
                response.InputHint = InputHints.IgnoringInput;
                await context.PostAsync(response);
            }

            await context.PostAsync("Open Intent Finished: " + status);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Greeting" with the name of your newly created intent in the following handler
        [LuisIntent("HomeAutomation.TurnOn")]
        public async Task HomeAutomationTurnOnIntent(IDialogContext context, LuisResult result)
        {
            //await this.ShowLuisResult(context, result);

            var client = new RestClient();
            client.EndPoint = @"https://api.particle.io/v1/devices/2c0026000f47363336383437/doorStatus?access_token=139a6bbeb6a6463a35a5a06c472d7f32ab8dc9bb";
            client.Method = HttpVerb.GET;
            client.ContentType = "application/x-www-form-urlencoded";
            string status = client.MakeRequest();
            dynamic j = JsonConvert.DeserializeObject(status);
            int doorStatus = j.result;

            if(doorStatus == 0)
            {
                client.EndPoint = @"https://api.particle.io/v1/devices/2c0026000f47363336383437/led?access_token=139a6bbeb6a6463a35a5a06c472d7f32ab8dc9bb";
                client.Method = HttpVerb.POST;
                client.PostData = "&arg=on";
                client.ContentType = "application/x-www-form-urlencoded";
                status = client.MakeRequest();
                if (status.Contains("failed"))
                {
                    // Failed open door request
                    IMessageActivity response = context.MakeMessage();
                    response.Text = "Failed to open the garage door!";
                    response.Speak = "No can do senior";
                    response.InputHint = InputHints.IgnoringInput;
                    await context.PostAsync(response);
                }
                else
                {
                    // Success, Opening the garage door.
                    IMessageActivity response = context.MakeMessage();
                    response.Text = "Opening the garage door!";
                    response.Speak = "Open Sesame";
                    response.InputHint = InputHints.IgnoringInput;
                    await context.PostAsync(response);
                }
            }
            else
            {
                // The door is already open
                IMessageActivity response = context.MakeMessage();
                response.Text = "The garage door is already open";
                response.Speak = "Silly human, the door is already open";
                response.InputHint = InputHints.IgnoringInput;
                await context.PostAsync(response);
            }

            await context.PostAsync("Open Intent Finished: " + status);
        }

        [LuisIntent("HomeAutomation.TurnOff")]
        public async Task HomeAutomationTurnOffIntent(IDialogContext context, LuisResult result)
        {
            //await this.ShowLuisResult(context, result);

            var client = new RestClient();
            client.EndPoint = @"https://api.particle.io/v1/devices/2c0026000f47363336383437/doorStatus?access_token=139a6bbeb6a6463a35a5a06c472d7f32ab8dc9bb";
            client.Method = HttpVerb.GET;
            client.ContentType = "application/x-www-form-urlencoded";
            string status = client.MakeRequest();
            dynamic j = JsonConvert.DeserializeObject(status);
            int doorStatus = j.result;

            if (doorStatus == 100)
            {
                client.EndPoint = @"https://api.particle.io/v1/devices/2c0026000f47363336383437/led?access_token=139a6bbeb6a6463a35a5a06c472d7f32ab8dc9bb";
                client.Method = HttpVerb.POST;
                client.PostData = "&arg=off";
                client.ContentType = "application/x-www-form-urlencoded";
                status = client.MakeRequest();

                if (status.Contains("failed"))
                {
                    // Failed request to close the garage door
                    IMessageActivity response = context.MakeMessage();
                    response.Text = "Failed to close the garage door!";
                    response.Speak = "No can do senior";
                    response.InputHint = InputHints.IgnoringInput;
                    await context.PostAsync(response);
                }
                else
                {
                    // Success - Closing the garage door.
                    var response = context.MakeMessage();
                    response.Text = "Closing the garage door!";
                    response.Speak = "Your wish is my command";
                    response.InputHint = InputHints.IgnoringInput;
                    await context.PostAsync(response);
                }
            }
            else
            {
                // The door is already open
                IMessageActivity response = context.MakeMessage();
                response.Text = "The garage door is already closed";
                response.Speak = "Silly human, the door is already closed";
                response.InputHint = InputHints.IgnoringInput;
                await context.PostAsync(response);
            }

            await context.PostAsync("Close Intent Finished: " + status);
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                //this.count = 1;
                await context.PostAsync("Opening the Garage Door");
                var client = new RestClient();
                client.EndPoint = @"https://api.particle.io/v1/devices/2c0026000f47363336383437/led?access_token=139a6bbeb6a6463a35a5a06c472d7f32ab8dc9bb";
                client.Method = HttpVerb.POST;
                client.PostData = "&arg=on";
                client.ContentType = "application/x-www-form-urlencoded";
                var json = client.MakeRequest();
            }
            else
            {
                await context.PostAsync("Not opening the garage door.");
            }
            //context.Wait(HomeAutomationTurnOnIntent);
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