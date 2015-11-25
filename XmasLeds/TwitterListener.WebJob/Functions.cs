using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs;
using Rumr.DurryLights.Domain.Models;
using Rumr.DurryLights.Domain.Services;
using Rumr.DurryLights.Domain.Utilities;
using Rumr.DurryLights.ServiceBus;
using Rumr.DurryLights.Sql;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Events.EventArguments;
using Stream = Tweetinvi.Stream;

namespace TwitterListener.WebJob
{
    public class Functions
    {
        private static ILightsService _lightsService;

        [NoAutomaticTrigger]
        public async static void ListenForTweetsAsync(TextWriter log)
        {
            var credentials = new TwitterCredentials(
                ConfigurationManager.AppSettings["TwitterConsumerKey"],
                ConfigurationManager.AppSettings["TwitterConsumerSecret"],
                ConfigurationManager.AppSettings["TwitterAccessToken"],
                ConfigurationManager.AppSettings["TwitterAccessTokenSecret"]);

            Auth.SetCredentials(credentials);

            var stream = Stream.CreateUserStream();
            stream.TweetCreatedByAnyoneButMe += OnTweetCreated;
            
            await stream.StartStreamAsync();
        }

        private async static void OnTweetCreated(object sender, TweetReceivedEventArgs e)
        {
            await Console.Out.WriteLineAsync("Received tweet: " + e.Tweet);

            var text = RemoveTwitterHandles(e.Tweet.Text);

            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Source", "twitter"),
                    new KeyValuePair<string, string>("Text", text),
                    new KeyValuePair<string, string>("From", e.Tweet.CreatedBy.Name)
                });

                await client.PostAsync(ConfigurationManager.AppSettings["DurryLightsApiEndpoint"], content);
            }

            Tweet.PublishTweetInReplyTo(string.Format("@{0} Thanks for your request!", e.Tweet.CreatedBy.ScreenName), e.Tweet);
        }

        private static string RemoveTwitterHandles(string text)
        {
            return Regex.Replace(text, "@\\w+", string.Empty);
        }
    }


}
