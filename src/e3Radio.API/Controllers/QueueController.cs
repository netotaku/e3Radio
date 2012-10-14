using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace e3Radio.API.Controllers
{
    public class QueueController : ApiController
    {
        public object Get()
        {
            using (var db = new e3Radio.Data.E3RadioEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                return (from q in db.Queues
                        where q.DatePlayed == null
                        orderby q.UpVotes - q.DownVotes descending, q.QueueID
                        let track = q.Track
                        select new
                        {
                            q.QueueID,
                            q.UpVotes,
                            q.DownVotes,
                            q.DateAdded,
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

        /// <summary>
        /// Handles queue actions.
        /// </summary>
        /// <param name="type">add, upvote, downvote, unvote</param>
        /// <param name="id">a queue ID as returned by the queue listing, or spotify uri for add requests</param>
        public void Get(string type, string id)
        {
            // fb user id of current user
            var userId = Facebook.Web.FacebookWebContext.Current.UserId;

            // set user ID to "autoplay" bot for testing purposes when not authed
            if (userId == 0) userId = 1;

            // call method to perform requested action
            if (type == "add")
            {
                AddTrackToQueue(id, userId);
            }
            else
            {
                VoteOnQueuedTrack(int.Parse(id), userId, type);
            }
        }

        private void VoteOnQueuedTrack(int queueId, long userId, string type)
        {
            using (var db = new e3Radio.Data.E3RadioEntities())
            {
                // check user exists in db
                var u = db.Users.SingleOrDefault(us => us.UserID == userId);
                if (u == null)
                {
                    u = new e3Radio.Data.User();

                    // get the dude's info from book of face
                    var fb = new Facebook.Web.FacebookWebClient();
                    dynamic me = fb.Get("/me");
                    u.UserID = userId;
                    u.Username = me.username ?? me.name;
                    u.Name = me.name;
                    u.FacebookLink = me.link;
                    u.DateCreated = DateTime.Now;

                    db.Users.Add(u);
                    db.SaveChanges();
                }

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
                if (type == "unvote")
                {
                    // delete the vote
                    db.QueueVotes.Remove(existingLike);
                }
                else
                {
                    // add or update
                    existingLike.IsVoteUp = (type == "voteup");
                    existingLike.DateVoted = DateTime.Now;
                }
                db.SaveChanges();
            }
        }

        private void AddTrackToQueue(string spotifyUri, long userId)
        {
            using (var db = new e3Radio.Data.E3RadioEntities())
            {
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

    }
}
