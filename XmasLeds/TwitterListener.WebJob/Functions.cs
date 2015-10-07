using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Events.EventArguments;
using Stream = Tweetinvi.Stream;

namespace TwitterListener.WebJob
{
    public class Functions
    {
        private static MessagingFactory _factory;

        [NoAutomaticTrigger]
        public async static void ListenForTweetsAsync(TextWriter log)
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            _factory = MessagingFactory.CreateFromConnectionString(connectionString);
            
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

            var matches = Regex.Match(e.Tweet.Text, "(\\d{1,3}),(\\d{1,3}),(\\d{1,3})", RegexOptions.Compiled);

            if (matches.Success)
            {
                var colourRequest = new ColourRequest
                {
                    Red = int.Parse(matches.Groups[1].Value),
                    Green = int.Parse(matches.Groups[2].Value),
                    Blue = int.Parse(matches.Groups[3].Value)
                };

                var topicClient = _factory.CreateTopicClient("Commands");

                var json = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(colourRequest)));
                var brokeredMessage = new BrokeredMessage(json);

                topicClient.Send(brokeredMessage);

                Tweet.PublishTweetInReplyTo(string.Format("@{0} Thanks for your request!", e.Tweet.CreatedBy.ScreenName), e.Tweet);
            }
        }
    }

    public class ColourRequest
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
    }
}
