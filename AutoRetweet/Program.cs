using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dimebrain.TweetSharp;
using Dimebrain.TweetSharp.Fluent;
using Dimebrain.TweetSharp.Model;
using Dimebrain.TweetSharp.Extensions;

namespace AutoRetweet
{
    class Program
    {
        private static TwitterClientInfo _clientInfo = new TwitterClientInfo
        {
            ClientName = "AutoRetweet",
            ClientUrl = "http://www.themichaelvickproject.com",
            ClientVersion = "0.1",
            ConsumerKey = "iTZ54JWpeBW6z9p23i3lA",
            ConsumerSecret = "6ABSwbKNBPJDFpqwjPf91sXUc1UqArrATi1fLdVOAg"
        };

        static void Main(string[] args)
        {
            long lastStatusId = GetLastStatusId();

            if (lastStatusId == long.MinValue)
                return;

            TwitterSearchResult searchResults = GetLatestSearchResults(lastStatusId);

            if (searchResults == null)
                return;

            //Cap it to 10 updates
            int statusesToUpdate = searchResults.Statuses.Count > 10 ? 10 : searchResults.Statuses.Count;

            for(int i=0; i< statusesToUpdate; i++)
            {
                if (searchResults.Statuses[i].FromUserScreenName.Contains("michaelvickproj") ||
                    searchResults.Statuses[i].FromUserScreenName.Contains("JameyBearX") ||
                    searchResults.Statuses[i].FromUserScreenName.Contains("w33z") ||
                    searchResults.Statuses[i].FromUserScreenName.Contains("photoshopgrl"))
                    i--;
                else
                    UpdateStatus(FormatStatus(searchResults.Statuses[i].Text, searchResults.Statuses[i].FromUserScreenName));
            }

        }

        private static string FormatStatus(string sourceStatus, string sourceUserScreenName)
        {
            string retweetMsg = String.Format("RT @{0} {1}", sourceUserScreenName, sourceStatus);

            if (retweetMsg.Length < 116)
                return retweetMsg + " More: http://is.gd/2knIr";
            else
                return retweetMsg.Substring(0, 115) + " More: http://is.gd/2knIr";
        }

        private static void UpdateStatus(string newStatus)
        {
            var request = FluentTwitter.CreateRequest(_clientInfo);
            var twitter = FluentTwitter.CreateRequest().AuthenticateAs("michaelvickproj", "d0gs@rec00l").Statuses().Update(newStatus).AsJson();            

            var response = twitter.Request();

            var responseError = response.AsError();
        }

        private static TwitterSearchResult GetLatestSearchResults(long sinceStatusId)
        {
            var request = FluentTwitter.CreateRequest(_clientInfo);
            var twitter = FluentTwitter.CreateRequest().Search().Query().Containing("Michael Vick").Since(sinceStatusId).AsJson();



            // Sequential call for data  
            var response = twitter.Request();

            var searchResults = response.AsSearchResult();

            return searchResults;
        }

        private static long GetLastStatusId()
        {
            var request = FluentTwitter.CreateRequest(_clientInfo);
            var twitter = FluentTwitter.CreateRequest().Statuses().OnUserTimeline().For("michaelvickproj").AsJson();
            
            // Sequential call for data  
            var response = twitter.Request();

            // Convert response to data classes  
            var statuses = response.AsStatuses();

            if (statuses == null)
                return long.MinValue;
            else
                return statuses.First().Id;
        }
    }
}
