using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FavoriteYoutubeVideos
{
    public class VideoPoco
    {
        public int VideoId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ChannelName { get; set; }
        public bool Deleted { get; set; }
    }
}
