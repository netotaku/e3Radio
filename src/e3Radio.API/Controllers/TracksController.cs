using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace e3Radio.API.Controllers
{
    /// <summary>
    /// Handles Track listing and voting requests.
    /// </summary>
    public class TracksController : ApiController
    {
        /// <summary>
        /// Handles requests for track listings.
        /// GET api/tracks/{type}?page=X&size=Y
        /// </summary>
        /// <param name="type">One of the chart types (see GetTracksQueryable)</param>
        /// <param name="page">Page number starting from 1</param>
        /// <param name="size">Page size</param>
        /// <returns></returns>
        public dynamic Get(string type, int page = 1, int size = 10)
        {
            // fb user id to get my vote
            var userId = Facebook.Web.FacebookWebContext.Current.UserId;

            // get from cache if possible
            string cacheKey = GetCacheKey(type, userId, page, size);
            var results = System.Web.HttpContext.Current.Cache[cacheKey];
            if (results == null)
            {
                //todo:lock here by cache key
                using (var db = new e3Radio.Data.E3RadioEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    // filter and order by
                    var filteredQuery = GetTracksQueryable(type, userId, db);

                    // pagination
                    var pagedQuery = filteredQuery.Skip((page - 1) * size).Take(size);

                    // get the data we like in a list
                    results = e3Radio.Data.TrackManager.FormatTracks(pagedQuery);
                }
                System.Web.HttpContext.Current.Cache.Insert(cacheKey, results, null, DateTime.Now.AddSeconds(10), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return results;
        }

        /// <summary>
        /// Creates a unique key for this track request
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static string GetCacheKey(string type, long userId, int page, int size)
        {
            // create a unique key for this track request
            string key = "e3Radio-tracks-" + type + "/" + page + "/" + size;
            if (type.StartsWith("my-"))
            {
                // this type of feed is specific to the user so cache by user
                key += "/" + userId;
            }
            return key;
        }

        /// <summary>
        /// Gets an IQueryable which selects Tracks filtered and ordered
        /// based on the chart type requested.
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="userId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private static IQueryable<e3Radio.Data.Track> GetTracksQueryable(string chart, long userId, e3Radio.Data.E3RadioEntities db)
        {
            IQueryable<e3Radio.Data.Track> result;

            // filter and order 
            if (chart == "worst")
            {
                result = from track in db.Tracks
                         where track.Likes > 0 || track.Dislikes > 0
                         orderby track.Likes - track.Dislikes
                         select track;
            }
            else if (chart == "most-played")
            {
                result = from track in db.Tracks
                         orderby track.Queues.Count
                         select track;
            }
            else if (chart == "marmite")
            {
                // tracks which are both liked and hated, order by the most voted, weight the most marmite to the top
                result = from track in db.Tracks
                         where track.Likes > 0 && track.Dislikes > 0
                         let totalVotes = track.Likes + track.Dislikes
                         let voteDifference = Math.Abs(track.Likes - track.Dislikes)
                         orderby (totalVotes - voteDifference) descending
                         select track;
            }
            else if (chart == "my-loves")
            {
                // tracks you loved, most recent first
                result = from tl in db.TrackLikes
                         where tl.UserID == userId && tl.IsLike
                         orderby tl.DateLiked descending
                         select tl.Track;
            }
            else if (chart == "my-hates")
            {
                // tracks you hated, most recent first
                result = from tl in db.TrackLikes
                         where tl.UserID == userId && !tl.IsLike
                         orderby tl.DateLiked descending
                         select tl.Track;
            }
            else if (chart == "ultimate")
            {
                // ultimate (voted on tracks order by karma)
                result = from track in db.Tracks
                         where track.Likes > 0 || track.Dislikes > 0
                         orderby track.Likes - track.Dislikes descending
                         select track;
            }
            else if (chart == "upcoming")
            {
                // next queued tracks
                result = from q in db.Queues
                         where q.DatePlayed == null
                         orderby q.UpVotes - q.DownVotes descending, q.QueueID
                         select q.Track;
            }
            else if (chart == "recent")
            {
                // most recently played
                result = from q in db.Queues
                         where q.DatePlayed != null
                         orderby q.DatePlayed descending
                         select q.Track;
            }
            else
            {
                throw new Exception("invalid track type!");
            }
            return result;
        }

        /// <summary>
        /// Handles vote actions.
        /// </summary>
        /// <param name="type">love, hate or unlovehate</param>
        /// <param name="id">a track ID as returned by the track listing</param>
        public void Get(string type, int id)
        {
            // fb user id of current user
            var userId = Facebook.Web.FacebookWebContext.Current.UserId;

            // set user ID to "autoplay" bot for testing purposes when not authed
            if (userId == 0) userId = 1;

            // Save.
            var tp = (e3Radio.Data.TrackManager.TrackVoteType)Enum.Parse(typeof(e3Radio.Data.TrackManager.TrackVoteType), type);
            e3Radio.Data.TrackManager.SaveTrackVote(userId, id, tp);
        }

    }
}
