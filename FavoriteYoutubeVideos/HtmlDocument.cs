using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FavoriteYoutubeVideos
{
    public static class HtmlDocument
    {
        public static string Create(List<string> videos)
        {
            string videosToString = string.Empty;
            foreach (string video in videos)
            {
                videosToString += video;
            }

            return videosToString;
        }
    }

    public static class HtmlVideo
    {
        public static string Create(string title, string channelName, string thumbUrl, string description)
        {
            return
            //$"<div class='card'>\n<h2>{title}</h2>\n<h5>{channelName}</h5>\n<div class='fakeimg' style='height: 200px;'>\n<img src='{thumbUrl}' />\n</div>\n<p>{description}</p>\n</div>";
                $"<div class='card'>\n<h2>{title}</h2>\n<h5>{channelName}</h5>\n<img src='{thumbUrl.Replace("default", "hqdefault")}'>\n<p class='description'>{description}</p>\n</div>";
        }
    }
}
