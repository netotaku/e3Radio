using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace e3Radio.API.Models
{
    public class Spotify
    {

        public static Track GetOrCreateTrackBySpotifyUri(e3RadioEntities db, string reqUri)
        {
            // find existing track, if it has been played before
            var track = db.Tracks.FirstOrDefault(t => t.SpotifyUri == reqUri);
            if (track == null)
            {
                track = new Track()
                {
                    SpotifyUri = reqUri,
                    Likes = 0,
                    Dislikes = 0,
                    DateAdded = DateTime.Now
                };

                // look up track info from spotify or die
                if (!LookupTrackInfo(track)) return null;

                db.Tracks.Add(track);
            }

            // update additional track info from last.fm
            LastFM.UpdateTrackFromLastFm(track);

            return track;
        }

        /// <summary>
        /// Attempts to load track details from spotify uri
        /// </summary>
        /// <param name="track"></param>
        public static bool LookupTrackInfo(Track track)
        {
            try
            {
                // search for track using Spotify metadata API and get spotify URI
                System.Xml.Linq.XNamespace ns = "http://www.spotify.com/ns/music/1";
                XElement data = XElement.Load("http://ws.spotify.com/lookup/1/?uri=" + System.Web.HttpUtility.UrlEncode(track.SpotifyUri));
                track.TrackName = (string)data.Element(ns + "name");
                track.Artist = (string)data.Element(ns + "artist").Element(ns + "name");
                if (data.Element(ns + "album") != null)
                {
                    track.Album = (string)data.Element(ns + "album").Element(ns + "name");
                }
                track.Length = (double?)data.Element(ns + "length");
                track.Popularity = (double?)data.Element(ns + "popularity");
                return true;
            }
            catch
            {
                // prolly rate limited
                return false;
            }
        }
    }
}
