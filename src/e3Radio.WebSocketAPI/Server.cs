using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fleck;
using System.Xml.Linq;

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
            var server = new WebSocketServer(System.Configuration.ConfigurationManager.AppSettings["listenOn"]);
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
                    HandleIncomingMessage(socket, message);
                };
            });

            // Don't exit the program until 'exit' is typed
            Console.WriteLine("Type 'exit' to stop server, or anything else to broadcast a message.");
            var input = Console.ReadLine();
            while (input != "exit")
            {
                BroadcastEvent("chat-message", input);
                input = Console.ReadLine();
            }
        }

        /// <summary>
        /// Gets the user's Facebook ID from their fb cookie.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private static long GetUserID(IWebSocketConnection socket)
        {
            try
            {
                // cookie key includes app id
                var fbCookieValue = socket.ConnectionInfo.Cookies["fbsr_" + System.Configuration.ConfigurationManager.AppSettings["fbAppId"]];
                Facebook.FacebookClient x = new Facebook.FacebookClient();

                // parse the cookie using the app secret
                dynamic y = x.ParseSignedRequest(System.Configuration.ConfigurationManager.AppSettings["fbAppSecret"], fbCookieValue);

                return long.Parse(y.user_id);
            }
            catch
            {
                // user id 1 = autoplay. this is temporary...
                return 1;
            }
        }

        /// <summary>
        /// Handle incoming messages based on spec in readme.md
        /// </summary>
        /// <param name="message">Incoming json string</param>
        private static void HandleIncomingMessage(IWebSocketConnection socket, string message)
        {
            // Convert incoming json into a readable format
            var json = Newtonsoft.Json.JsonConvert.DeserializeXNode("{root:" + message + "}").Element("root");

            // Get common params
            string eventName = (string)json.Element("event");
            long userId = GetUserID(socket);

            // Handle the event
            switch (eventName)
            {
                case "TX-playQueue":
                    // optional parameters, not yet used in interface
                    string chart = (string)json.Element("chart") ?? "playQueue";
                    int page = (int?)json.Element("page") ?? 1;
                    int pageSize = (int?)json.Element("pageSize") ?? 10;
                    object data = e3Radio.Data.TrackManager.GetTrackListing(chart, userId, page, pageSize);
                    SendEvent(socket, "RX-playQueue", data);
                    break;
                case "TX-track":
                    int tId = (int)json.Element("id");
                    var tk = e3Radio.Data.TrackManager.GetTrack(tId);
                    SendEvent(socket, "RX-track", tk);
                    break;
                //case "love":
                //case "hate":
                //case "unvote":
                //    int? trackId = (int?)json.Element("trackId");
                //    var tp = (e3Radio.Data.TrackManager.TrackVoteType)Enum.Parse(typeof(e3Radio.Data.TrackManager.TrackVoteType), eventName);
                //    e3Radio.Data.TrackManager.SaveTrackVote(userId.Value, trackId.Value, tp);
                //    break;
                case "TX-request":
                    string spotifyUri = (string)json.Element("data");
                    var track = e3Radio.Data.TrackManager.RequestTrack(spotifyUri, userId);
                    BroadcastEvent("RX-request", track);
                    break;
                case "TX-movePlayhead":
                    string spotifyUri2 = (string)json.Element("data");
                    var track2 = e3Radio.Data.TrackManager.UpdateNowPlayingTrack(spotifyUri2);
                    BroadcastEvent("RX-movePlayhead", track2);
                    break;
                case "chat-message":
                    allSockets.ToList().ForEach(s => s.Send(message));
                    break;
            }
        }

        /// <summary>
        /// Send message to all sockets to advance the playhead to this track.
        /// </summary>
        /// <param name="track"></param>
        private static void BroadcastEvent(string @event, object data)
        {
            // Broadcast message to users in JSON format
            var msg = new
            {
                @event = @event,
                data = data
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
            allSockets.ToList().ForEach(s => s.Send(json));
        }

        /// <summary>
        /// Send message to the socket.
        /// </summary>
        /// <param name="track"></param>
        private static void SendEvent(IWebSocketConnection socket, string @event, object data)
        {
            // Broadcast message to users in JSON format
            var msg = new
            {
                @event = @event,
                data = data
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
            socket.Send(json);
        }        
    }
}
