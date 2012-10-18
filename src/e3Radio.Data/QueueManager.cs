using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace e3Radio.Data
{
    public class QueueManager
    {
        public enum QueueVoteType { votedown, voteup, unvote };

        public static void SaveQueueVote(long userId, int queueId, QueueVoteType type)
        {
            using (var db = new e3Radio.Data.E3RadioEntities())
            {
                // check user exists in db
                UserManager.GetOrCreateUser(db, userId);

                // add or update queue vote
                var existingLike = db.QueueVotes.SingleOrDefault(tl => tl.QueueID == queueId && tl.UserID == userId);
                if (existingLike == null)
                {
                    // add new
                    existingLike = new e3Radio.Data.QueueVote();
                    existingLike.QueueID = queueId;
                    existingLike.UserID = userId;
                    db.QueueVotes.Add(existingLike);
                }
                if (type == QueueVoteType.unvote)
                {
                    // delete the vote
                    db.QueueVotes.Remove(existingLike);
                }
                else
                {
                    // add or update
                    existingLike.IsVoteUp = (type == QueueVoteType.voteup);
                    existingLike.DateVoted = DateTime.Now;
                }
                db.SaveChanges();
            }
        }

        public static void AddTrackToQueue(string spotifyUri, long userId)
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

                // add to the play queue
                var queue = new e3Radio.Data.Queue()
                {
                    DateAdded = DateTime.Now,
                    UserID = userId,
                    Track = track,
                    UpVotes = 0,
                    DownVotes = 0
                };
                db.Queues.Add(queue);

                // start with 1 vote if added by a human
                if (userId > 1)
                {
                    queue.UpVotes = 1;
                    queue.QueueVotes.Add(new e3Radio.Data.QueueVote()
                    {
                        DateVoted = DateTime.Now,
                        IsVoteUp = true,
                        UserID = userId
                    });
                }

                db.SaveChanges();
            }
        }

        public static void UpdateNowPlayingTrack(string spotifyUri)
        {
            using (var db = new e3Radio.Data.E3RadioEntities())
            {
                var nowPlayingRow = db.Queues.FirstOrDefault(r => r.Track.SpotifyUri == spotifyUri && r.DatePlayed == null);
                if (nowPlayingRow != null)
                {
                    // update now playing track (replacement for last fm scrobbler)
                    nowPlayingRow.DatePlayed = DateTime.Now;
                }
                else
                {
                    //TODO: They played something not in the queue.
                }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Convert a queryable list of Queues into a List of anonymous
        /// types suitable to be converted into json and returned.
        /// </summary>
        /// <param name="queues"></param>
        /// <returns></returns>
        public static object FormatQueues(IQueryable<Queue> queues)
        {
            return (from q in queues
                    let track = q.Track
                    let user = q.User
                    select new
                    {
                        q.QueueID,
                        q.UpVotes,
                        q.DownVotes,
                        q.DateAdded,
                        user.Name,
                        user.UserID,
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
                    }).ToList();
        }
    }
}
