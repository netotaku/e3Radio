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

        public static void RequestTrack(string spotifyUri, long userId)
        {
            using (var db = new e3Radio.Data.E3RadioEntities())
            {
                // check user exists in db
                UserManager.GetOrCreateUser(db, userId);

                // find existing track, if it has been played before, or add
                var track = e3Radio.Data.Spotify.GetOrCreateTrackBySpotifyUri(db, spotifyUri);
                if (track == null)
                {
                    throw new Exception("Failed to find Spotify Track");
                }

                // mark it as requested by this user and now, this makes it play sooner
                track.RequestDate = DateTime.Now;
                track.RequestUserID = userId;

                db.SaveChanges();
            }
        }

        public static void UpdateNowPlayingTrack(string spotifyUri)
        {
            using (var db = new e3Radio.Data.E3RadioEntities())
            {
                var nowPlayingRow = db.Tracks.FirstOrDefault(r => r.SpotifyUri == spotifyUri);
                if (nowPlayingRow != null)
                {
                    // update now playing track (replacement for last fm scrobbler)
                    nowPlayingRow.LastPlayed = DateTime.Now;

                    // Wipe any request date to make sure it goes to back of queue
                    nowPlayingRow.RequestDate = null;

                    // increment play counter
                    nowPlayingRow.PlayCount++;
                }
                else
                {
                    //TODO: They played something not in the queue.
                }

                db.SaveChanges();
            }
        }
    }
}
