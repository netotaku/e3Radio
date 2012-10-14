using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace e3Radio.API.Models
{
    public class Spotify
    {
        /// <summary>
        /// Find a Track from it's Spotify URI.
        /// If it isn't in the DB we will load it's details and create it.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="reqUri"></param>
        /// <returns></returns>
        public static Track GetOrCreateTrackBySpotifyUri(e3RadioEntities db, string reqUri)
        {
            // find existing track, if it has been played before
            var track = db.Tracks.FirstOrDefault(t => t.SpotifyUri == reqUri);
            if (track == null)
            {
                // Track is not in the database, load info from spotify/last.fm
                track = new Track()
                {
                    SpotifyUri = reqUri,
                    Likes = 0,
                    Dislikes = 0,
                    DateAdded = DateTime.Now
                };

                // look up track info from spotify or die
                if (!LookupTrackInfo(track)) return null;

                // update additional track info (cover art) from last.fm
                LastFM.UpdateTrackFromLastFm(track);

                db.Tracks.Add(track);
            }
            else // Track is already in the database.
            {
                // Update fields we recently added from Spotify
                if (!track.Length.HasValue)
                    LookupTrackInfo(track);

                // Try to get cover art if not set
                if (track.PictureSmall == null)
                    LastFM.UpdateTrackFromLastFm(track);
            }

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
