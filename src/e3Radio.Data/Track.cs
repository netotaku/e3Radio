//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace e3Radio.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Track
    {
        public Track()
        {
            this.TrackLikes = new HashSet<TrackLike>();
        }
    
        public int TrackID { get; set; }
        public string Artist { get; set; }
        public string TrackName { get; set; }
        public string Album { get; set; }
        public string LastFmLink { get; set; }
        public string PictureSmall { get; set; }
        public string PictureMedium { get; set; }
        public string PictureLarge { get; set; }
        public string PictureExtraLarge { get; set; }
        public System.DateTime DateAdded { get; set; }
        public Nullable<System.DateTime> DateUpdated { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string SpotifyUri { get; set; }
        public Nullable<int> Length { get; set; }
        public int PlayCount { get; set; }
        public Nullable<System.DateTime> LastPlayed { get; set; }
        public Nullable<long> RequestUserID { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
    
        public virtual User User { get; set; }
        public virtual ICollection<TrackLike> TrackLikes { get; set; }
    }
}
