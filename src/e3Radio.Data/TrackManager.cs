using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace e3Radio.Data
{
    public class TrackManager
    {
        public enum TrackVoteType { love, hate, unvote };

        public static void SaveTrackVote(long userId, int trackId, TrackVoteType type)
        {
            using (var db = new e3Radio.Data.E3RadioEntities())
            {
                // check user exists in db
                UserManager.GetOrCreateUser(db, userId);

                // add, remove or update track like
                var existingLike = db.TrackLikes.SingleOrDefault(tl => tl.TrackID == trackId && tl.UserID == userId);
                if (existingLike == null)
                {
                    // add new
                    existingLike = new e3Radio.Data.TrackLike();
                    existingLike.TrackID = trackId;
                    existingLike.UserID = userId;
                    db.TrackLikes.Add(existingLike);
                }
                if (type == TrackVoteType.unvote)
                {
                    // delete the love/hate entry
                    db.TrackLikes.Remove(existingLike);
                }
                else
                {
                    // add or update
                    existingLike.IsLike = (type == TrackVoteType.love);
                    existingLike.DateLiked = DateTime.Now;
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Convert a queryable list of Tracks into a List of anonymous
        /// types suitable to be converted into json and returned.
        /// </summary>
        /// <param name="queues"></param>
        /// <returns></returns>
        public static object FormatTracks(IQueryable<Track> tracks)
        {
            return (from track in tracks
                    //let iLike = (bool?)track.TrackLikes.FirstOrDefault(tl => tl.UserID == userId).IsLike
                    let TrackLikes = track.TrackLikes.Select(l => new { l.User.Name, l.User.UserID, l.IsLike, l.DateLiked })
                    select new
                    {
                        track.TrackID,
                        track.Artist,
                        track.TrackName,
                        track.Album,
                        track.SpotifyUri,
                        track.LastFmLink,
                        track.Length,
                        track.PictureSmall,
                        track.PictureMedium,
                        track.PictureLarge,
                        track.PictureExtraLarge,
                        track.DateAdded,
                        track.Likes,
                        track.Dislikes,
                        track.PlayCount,
                        track.LastPlayed,
                        track.RequestDate,
                        track.RequestUserID,
                        //track.RequestUser, // causes linq problem
                        //iLike,  // not needed now we have TrackLikes
                        TrackLikes
                    }).ToList();
        }

        public static object RequestTrack(string spotifyUri, long userId)
        {
            using (var db = new e3Radio.Data.E3RadioEntities())
            {
                // check user exists in db
                UserManager.GetOrCreateUser(db, userId);

                // find existing track, if it has been played before, or add
                Track track = e3Radio.Data.Spotify.GetOrCreateTrackBySpotifyUri(db, spotifyUri);
                if (track == null)
                {
                    throw new Exception("Failed to find Spotify Track");
                }

                // mark it as requested by this user and now, this makes it play sooner
                track.RequestDate = DateTime.Now;
                track.RequestUserID = userId;

                db.SaveChanges();
                
                // Convert track into IQueryable so we can format it
                var tracks = new List<Data.Track>() { track }.AsQueryable();
                return FormatTracks(tracks);
            }
        }

        public static object UpdateNowPlayingTrack(string spotifyUri)
        {
            using (var db = new e3Radio.Data.E3RadioEntities())
            {
                Track nowPlaying = db.Tracks.FirstOrDefault(r => r.SpotifyUri == spotifyUri);
                if (nowPlaying != null)
                {
                    // update now playing track (replacement for last fm scrobbler)
                    nowPlaying.LastPlayed = DateTime.Now;

                    // Wipe any request date to make sure it goes to back of queue
                    nowPlaying.RequestDate = null;

                    // increment play counter
                    nowPlaying.PlayCount++;
                
                    db.SaveChanges();

                    // Convert track into IQueryable so we can format it
                    var tracks = new List<Data.Track>() { nowPlaying }.AsQueryable();
                    return FormatTracks(tracks);
                }
                else
                {
                    throw new NotSupportedException("The track was not in the play queue");
                }
            }
        }

        #region Track Listings


        /// <summary>
        /// Handles requests for track listings.
        /// </summary>
        /// <param name="type">One of the chart types (see GetTracksQueryable)</param>
        /// <param name="page">Page number starting from 1</param>
        /// <param name="size">Page size</param>
        /// <returns></returns>
        public static object GetTrackListing(string type, long userId, int page = 1, int size = 10)
        {
            // get from cache if possible
            string cacheKey = GetCacheKey(type, userId, page, size);
            var results = System.Runtime.Caching.MemoryCache.Default[cacheKey];
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
                // Disabled caching because wrong tracks can be returned
                //System.Runtime.Caching.MemoryCache.Default.Add(cacheKey, results, DateTime.Now.AddSeconds(10));
            }
            return results;
        }

        /// <summary>
        /// Creates a unique key for this track request
        /// </summary>
        private static string GetCacheKey(string type, long userId, int page, int size)
        {
            // create a unique key for this track request
            string key = "e3Radio-tracks-" + type + "/" + page + "/" + size;
            if (type.StartsWith("my"))
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
            else if (chart == "mostPlayed")
            {
                result = from track in db.Tracks
                         orderby track.PlayCount descending
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
            else if (chart == "myLikes")
            {
                // tracks you loved, most recent first
                result = from tl in db.TrackLikes
                         where tl.UserID == userId && tl.IsLike
                         orderby tl.DateLiked descending
                         select tl.Track;
            }
            else if (chart == "myDislikes")
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
            else if (chart == "playQueue")
            {
                // next queued tracks
                result = from t in db.Tracks
                         where t.Likes >= t.Dislikes || t.RequestDate != null
                         orderby t.RequestDate ?? DateTime.MaxValue, t.LastPlayed
                         select t;
            }
            else if (chart == "recent")
            {
                // most recently played
                result = from t in db.Tracks
                         where t.LastPlayed != null
                         orderby t.LastPlayed descending
                         select t;
            }
            else
            {
                throw new Exception("invalid track type!");
            }
            return result;
        }

        #endregion

    }
}
