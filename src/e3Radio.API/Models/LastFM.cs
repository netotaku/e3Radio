using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace e3Radio.API.Models
{
    public class LastFM
    {
        public static void UpdateTrackFromLastFm(Track track)
        {
            try
            {
                // load recent tracks from last.fm
                XElement playedTrack = XElement.Load("http://ws.audioscrobbler.com/2.0/?method=track.getInfo&artist=" + System.Web.HttpUtility.UrlEncode(track.Artist) + "&track=" + System.Web.HttpUtility.UrlEncode(track.TrackName) + "&api_key=" + ConfigurationManager.AppSettings["e3RadioLastFmApiKey"]);

                // Update track info
                track.Album = (string)playedTrack.Element("album").Element("title");
                track.LastFmLink = (string)playedTrack.Element("url");
                foreach (var image in playedTrack.Elements("image"))
                {
                    switch ((string)image.Attribute("size"))
                    {
                        case "small": track.PictureSmall = (string)image; break;
                        case "medium": track.PictureMedium = (string)image; break;
                        case "large": track.PictureLarge = (string)image; break;
                        case "extralarge": track.PictureExtraLarge = (string)image; break;
                    }
                }
                track.DateUpdated = DateTime.Now;
            }
            catch
            {
                //e.g. track not found on last fm
            }
        }
    }
}
