using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FavoriteYoutubeVideos
{
    public class YoutubeVideo
    {
        public string Title { get; }
        public string Description { get;}
        public string ThumbnailUrl { get;}
        public string UploadedBy { get;}
        public string Url { get;}
        public YoutubeVideo(string url)
        {
            string id = GetYouTubeId(url);
            var json = GetJsonFromApi(id);
            var vidInfo = json.Items[0].Snippet;

            this.Title = vidInfo.Title;
            this.Description = vidInfo.Description;
            this.ThumbnailUrl = vidInfo.Thumbnails.Default.Url;
            this.UploadedBy = vidInfo.ChannelTitle;
            this.Url = url;
        }


        private static YoutubeJson GetJsonFromApi(string vidId)
        {
            Uri requestUri = new Uri($@"https://www.googleapis.com/youtube/v3/videos?part=snippet&id={vidId}&key=AIzaSyDe9BTHGNPUokfch_MK1zykiBI2p_AipHA");

            WebRequest request = WebRequest.Create(requestUri);
            WebResponse resp = request.GetResponse();


            YoutubeJson result;
            using (Stream stream = resp.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = JsonConvert.DeserializeObject<YoutubeJson>(sr.ReadToEnd());
                    sr.Close();
                }
            }

            return result;
        }

        private static string GetYouTubeId(string url)
        {
            string id = Regex.Match(url,
                "https?:\\/\\/(?:[0-9A-Z-]+\\.)?(?:youtu\\.be\\/|youtube(?:-nocookie)?\\.com\\S*[^\\w\\s-])([\\w-]{11})(?=[^\\w-]|$)(?![?=&+%\\w.-]*(?:['\"][^<>]*>|<\\/a>))[?=&+%\\w.-]*",
                RegexOptions.IgnoreCase).Groups[1].Value;

            return id;
        }
    }

    public class PageInfo
    {
        public int TotalResults { get; set; }
        public int ResultsPerPage { get; set; }
    }

    public class Default
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Medium
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class High
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Standard
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Maxres
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Thumbnails
    {
        public Default Default { get; set; }
        public Medium Medium { get; set; }
        public High High { get; set; }
        public Standard Standard { get; set; }
        public Maxres Maxres { get; set; }
    }

    public class Localized
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class Snippet
    {
        public DateTime PublishedAt { get; set; }
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Thumbnails Thumbnails { get; set; }
        public string ChannelTitle { get; set; }
        public List<string> Tags { get; set; }
        public string CategoryId { get; set; }
        public string LiveBroadcastContent { get; set; }
        public Localized Localized { get; set; }
        public string DefaultAudioLanguage { get; set; }
    }

    public class Item
    {
        public Item(string kind)
        {
            this.Kind = kind;
        }

        public string Kind { get; set; }
        public string Etag { get; set; }
        public string Id { get; set; }
        public Snippet Snippet { get; set; }
    }

    public class YoutubeJson
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public PageInfo PageInfo { get; set; }
        public List<Item> Items { get; set; }
    }
}
