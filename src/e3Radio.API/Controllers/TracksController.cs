using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace e3Radio.API.Controllers
{
    public class TracksController : ApiController
    {
        // GET api/tracks/{id}?page=X&size=Y
        public dynamic Get(string id, int page = 1, int size = 10)
        {
            // fb user id to get my vote
            var userId = Facebook.Web.FacebookWebContext.Current.UserId;

            // get from cache if possible
            string cacheKey = GetCacheKey(id, userId, page, size);
            var results = System.Web.HttpContext.Current.Cache[cacheKey];
            if (results == null)
            {
                //todo:lock here by cache key
                using (var db = new e3Radio.API.Models.e3RadioEntities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    // filter and order by
                    var filteredQuery = GetTracksQueryable(id, userId, db);

                    // pagination
                    var pagedQuery = filteredQuery.Skip((page - 1) * size).Take(size);

                    // select what we want
                    var resultQ = from track in pagedQuery
                                  //let iLike = (bool?)track.TrackLikes.FirstOrDefault(tl => tl.UserID == userId).IsLike
                                  let TrackLikes = track.TrackLikes.Select(l => new { l.User.Name, l.User.UserID, l.IsLike })
                                  select new
                                  {
                                      track.TrackID,
                                      track.Artist,
                                      track.TrackName,
                                      track.Album,
                                      track.SpotifyUri,
                                      track.LastFmLink,
                                      track.Length,
                                      track.Popularity,
                                      track.PictureSmall,
                                      track.PictureMedium,
                                      track.PictureLarge,
                                      track.PictureExtraLarge,
                                      track.DateAdded,
                                      track.Likes,
                                      track.Dislikes,
                                      //iLike,  // not needed now we have TrackLikes
                                      TrackLikes
                                  };

                    results = resultQ.ToList();
                }
                System.Web.HttpContext.Current.Cache.Insert(cacheKey, results, null, DateTime.Now.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return results;
        }

        private static string GetCacheKey(string id, long userId, int page, int size)
        {
            // create a unique key for this track request
            string key = "e3Radio-tracks-" + id + "/" + page + "/" + size;
            if (id.StartsWith("my-"))
            {
                // this type of feed is specific to the user so cache by user
                key += "/" + userId;
            }
            return key;
        }

        private static IQueryable<Models.Track> GetTracksQueryable(string chart, long userId, Models.e3RadioEntities db)
        {
            IQueryable<Models.Track> result;

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
                         orderby track.TrackPlayeds.Count
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
                         orderby q.UpVotes - q.DownVotes descending, q.QueueID
                         select q.Track;
            }
            else if (chart == "recent")
            {
                // most recently played
                result = from trackPlays in db.TrackPlayeds
                         orderby trackPlays.TrackPlayedID descending
                         select trackPlays.Track;
            }
            else
            {
                throw new Exception("invalid track type!");
            }
            return result;
        }


    }
}
