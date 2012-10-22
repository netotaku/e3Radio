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
                    HandleIncomingMessage(message);
                };
            });

            // Don't exit the program until 'exit' is typed
            Console.WriteLine("Type 'exit' to stop server, or anything else to broadcast a message.");
            var input = Console.ReadLine();
            while (input != "exit")
            {
                BroadcastChatMessage(input);
                input = Console.ReadLine();
            }
        }

        private static void BroadcastChatMessage(string input)
        {
            // Broadcast message to users in JSON format
            var msg = new
                {
                    type = "chat",
                    message = input
                };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
            allSockets.ToList().ForEach(s => s.Send(json));
        }

        private static void HandleIncomingMessage(string message)
        {
            // Convert incoming json into a readable format
            var json = Newtonsoft.Json.JsonConvert.DeserializeXNode("{root:" + message + "}").Element("root");

            // Get common params
            string type = (string)json.Element("type");
            long? userId = (long?)json.Element("userId");
            int? trackId = (int?)json.Element("trackId");

            // Update DB if necessary depending on the type
            switch (type)
            {
                case "chat":
                    //todo: log chat messages?
                    break;
                case "love":
                case "hate":
                case "unvote":
                    var tp = (e3Radio.Data.TrackManager.TrackVoteType)Enum.Parse(typeof(e3Radio.Data.TrackManager.TrackVoteType), type);
                    e3Radio.Data.TrackManager.SaveTrackVote(userId.Value, trackId.Value, tp);
                    break;
                case "addtoqueue":
                    string spotifyUri = (string)json.Element("SpotifyUri");
                    e3Radio.Data.TrackManager.RequestTrack(spotifyUri, userId.Value);
                    break;
                case "nowplaying":
                    string spotifyUri2 = (string)json.Element("SpotifyUri");
                    e3Radio.Data.TrackManager.UpdateNowPlayingTrack(spotifyUri2);
                    break;
            }

            // broadcast this message to all clients so they can show notification
            allSockets.ToList().ForEach(s => s.Send(message));
        }


    }
}
