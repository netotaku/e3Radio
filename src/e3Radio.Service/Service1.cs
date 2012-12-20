using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Fleck;

namespace e3Radio.Service
{
    public partial class e3RadioService : ServiceBase
    {
        public e3RadioService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// List of connected clients
        /// </summary>
        static List<IWebSocketConnection> allSockets;

        /// <summary>
        /// The server instance.
        /// </summary>
        static WebSocketServer server;

        protected override void OnStart(string[] args)
        {
            // redirect console output to a log file this is where Fleck logs to
            var logFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            var logStream = new System.IO.StreamWriter(logFile);
            logStream.AutoFlush = true;
            Console.SetOut(logStream);
            FleckLog.Level = LogLevel.Debug;

            // ensure empty list of sockets is ready
            allSockets = new List<IWebSocketConnection>();

            // initialise the server
            server = new WebSocketServer(System.Configuration.ConfigurationManager.AppSettings["listenOn"]);
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    // when a client connects
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    // when a client disconnects
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    // when a client sends us a message, do something
                    HandleIncomingMessage(socket, message);
                };
            });
        }

        protected override void OnStop()
        {
            // close and remove references to all sockets
            if (allSockets != null)
            {
                foreach (var sock in allSockets)
                {
                    sock.Close();
                }
                allSockets.RemoveRange(0, allSockets.Count);
                allSockets = null;
            }

            // kill the server
            if (server != null)
            {
                server.Dispose();
                server = null;
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
        /// Sends data to all sockets in JSON format.
        /// </summary>
        private static void BroadcastEvent(string @event, object data)
        {
            var msg = new
            {
                @event = @event,
                data = data
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
            allSockets.ToList().ForEach(s => s.Send(json));
        }

        /// <summary>
        /// Sends data to the socket in JSON format.
        /// </summary>
        private static void SendEvent(IWebSocketConnection socket, string @event, object data)
        {
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
