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

                var res = from q in db.Queues
                          where q.DatePlayed == null
                          orderby q.UpVotes - q.DownVotes descending, q.QueueID
                          select q;
                        
                return e3Radio.Data.QueueManager.FormatQueues(res);
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
                e3Radio.Data.QueueManager.AddTrackToQueue(id, userId);
            }
            else
            {
                var qtp = (e3Radio.Data.QueueManager.QueueVoteType)Enum.Parse(typeof(e3Radio.Data.QueueManager.QueueVoteType), type);
                e3Radio.Data.QueueManager.SaveQueueVote(userId, int.Parse(id), qtp);
            }
        }
    }
}
