using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fleck;

namespace e3Radio.WebSocketAPI
{
    class Server
    {
        /// <summary>
        /// List of connected clients
        /// </summary>
        static List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();

        static void Main()
        {
            FleckLog.Level = LogLevel.Debug;
            var server = new WebSocketServer("ws://localhost:8181");
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    // when a client connects
                    allSockets.Add(socket);
                    Console.WriteLine("User connected! There are " + allSockets.Count + " connected users");
                };
                socket.OnClose = () =>
                {
                    // when a client disconnects
                    allSockets.Remove(socket);
                    Console.WriteLine("User disconnected! There are " + allSockets.Count + " connected users");
                };
                socket.OnMessage = message =>
                {
                    // when a client sends us a message, forward it to other clients
                    Console.WriteLine(message);
                    allSockets.ToList().ForEach(s => s.Send(message));  
                };
            });

            // Look for changes in radio data regularly and send to connected clients
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 5000; // 5 seconds
            aTimer.Enabled = true;

            // Don't exit the program until 'exit' is typed
            Console.WriteLine("Type 'exit' to stop server, or anything else to broadcast a message.");
            var input = Console.ReadLine();
            while (input != "exit")
            {
                // Broadcast message to users in JSON format
                var msg = new object[]{new
                {
                    type = "chat",
                    message = input
                }};
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(json);
                }
                input = Console.ReadLine();
            }
        }

        private static void HandleIncomingMessage(string message)
        {

        }

        // Specify what you want to happen when the Elapsed event is raised.
        private static bool running;
        private static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            if (!running)
            {
                running = true;
                using (var db = new e3Radio.Data.E3RadioEntities())
                {
                    LookForTrackChanges(db);
                    LookForRecentLikes(db);
                    LookForRecentVotes(db);
                }
                running = false;
            }
        }

        static DateTime lastLikeTime = DateTime.Now;
        private static void LookForRecentLikes(Data.E3RadioEntities db)
        {
            var result = (from likes in db.TrackLikes
                        where likes.DateLiked > lastLikeTime
                        orderby likes.DateLiked
						let track = likes.Track
						let user = likes.User
						select new
						{
                            type = "trackLike",
							user.Name,
							user.UserID,
							track.Artist,
							track.TrackName,
							likes.IsLike,
							likes.DateLiked
						}).ToList();

            lastLikeTime = DateTime.Now;
            if (result.Count > 0)
            {
                // broadcast recent likes
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(json);
                }
            }
        }

        static DateTime lastVoteTime = DateTime.Now;
        private static void LookForRecentVotes(Data.E3RadioEntities db)
        {
            var result = (from likes in db.QueueVotes
                          where likes.DateVoted > lastVoteTime
                          orderby likes.DateVoted
                          let track = likes.Queue.Track
                          let user = likes.User
                          select new
                          {
                              type = "queueVote",
                              user.Name,
                              user.UserID,
                              track.Artist,
                              track.TrackName,
                              likes.IsVoteUp,
                              likes.DateVoted
                          }).ToList();

            lastVoteTime = DateTime.Now;
            if (result.Count > 0)
            {
                // broadcast recent likes
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(json);
                }
            }
        }

        static int currentTrackPlayedID;
        private static void LookForTrackChanges(e3Radio.Data.E3RadioEntities db)
        {
            // Get current playing track
            var result =
                (from trackPlays in db.TrackPlayeds
                 orderby trackPlays.TrackPlayedID descending
                 select trackPlays).First();

            if (result.TrackPlayedID != currentTrackPlayedID)
            {
                currentTrackPlayedID = result.TrackPlayedID;
                // Broadcast track changed message
                var msg = new
                {
                    type = "trackChanged",
                    artist = result.Track.Artist,
                    trackName = result.Track.TrackName,
                    album = result.Track.Album,
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(json);
                }
            }
        }
    }
}
