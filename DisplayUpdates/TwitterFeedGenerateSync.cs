using System;
using System.Collections.Generic;
using System.Linq;
using TweetSharp;

namespace DisplayUpdates
{
    public class TwitterFeedGenerateSync : TwitterFeedBase
    {
        public TwitterFeedGenerateSync(Tuple<string, string> authKeys)
            : base(authKeys)
        {
            IEnumerable<TwitterStatus> tweets = 
                service.ListTweetsOnHomeTimeline();

            var state = 
                new Tuple<TwitterService, long, IEnumerable<TwitterStatus>>(
                        service, 
                        GetMaxId(tweets, 0), 
                        tweets);

            IObservable<TwitterStatus> futureTweets =
                Observable.GenerateWithTime(
                 state,         //initialState
                 _ => true,     //condition
                 st =>          //iterate
                 {            
                     var sinceId = st.Item2;
                     var newtweets = 
                         service.ListTweetsOnHomeTimelineSince(sinceId);
                     sinceId = GetMaxId(newtweets, sinceId);
                     return new Tuple<TwitterService, long, IEnumerable<TwitterStatus>>(
                                        service, sinceId, newtweets);
                 },
                st => st.Item3.ToObservable(),          //resultSelector
                st => GetSleepTime(st.Item1, sched),    //timeSelector
                sched)                                  //scheduler
                .SelectMany(a => a); // need to flatten IO<IO<T> to just IO<T>

            Tweets = tweets.ToObservable()
                           .Concat(futureTweets)
                           .ReplayLastByKey(tws => tws.User)
                           .Publish()
                           .RefCount();
        }
    }
}