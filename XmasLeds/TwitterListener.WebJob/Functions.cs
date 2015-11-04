using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using Rumr.DurryLights.Domain;
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
        private static IBusPublisher _busPublisher;
        private static LightDisplayParser _lightDisplayParser;

        [NoAutomaticTrigger]
        public async static void ListenForTweetsAsync(TextWriter log)
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            _busPublisher = new BusPublisher(connectionString);
            _lightDisplayParser = new LightDisplayParser(new ColourRepository());
            
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

            var lightDisplay = await _lightDisplayParser.ParseAsync(text);

            if (lightDisplay != null)
            {
                await _busPublisher.PublishAsync(lightDisplay);

                Tweet.PublishTweetInReplyTo(string.Format("@{0} Thanks for your request!", e.Tweet.CreatedBy.ScreenName), e.Tweet);
            }
        }

        private static string RemoveTwitterHandles(string text)
        {
            return Regex.Replace(text, "@\\w+", string.Empty);
        }
    }


}
