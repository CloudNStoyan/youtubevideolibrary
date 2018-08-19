using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Npgsql;

namespace FavoriteYoutubeVideos
{
    public static class Server
    {
        public static void Run(string url)
        {
            try
            {
                var listener = new HttpListener();
                listener.Prefixes.Add(url);
                listener.Start();
                while (true)
                {
                    Console.WriteLine("Waiting for connection...");
                    var context = listener.GetContext();

                    string requestBody;

                    using (var inputStream = context.Request.InputStream)
                    using (var reader = new StreamReader(inputStream, Encoding.UTF8))
                    {
                        requestBody = reader.ReadToEnd();
                    }
                    
                    string responseBody = MakeResponseBody(context,requestBody);

                    context.Response.StatusCode = (int)HttpStatusCode.OK;

                    var buffer = Encoding.UTF8.GetBytes(responseBody);

                    context.Response.ContentLength64 = buffer.Length;

                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    context.Response.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string MakeResponseBody(HttpListenerContext context,string requestBody)
        {
            string page = context.Request.RawUrl;
            if (page == "/post")
            {
                return SaveUrl(new YoutubeVideo(requestBody));

            } else if (page == "/videos")   
            {
                return GetAllVideosInHtml();
            }


            return "No info found";
        }

        private static string GetAllVideosInHtml()
        {
            using (var conn = new NpgsqlConnection(@"Server=vm5;Port=5437;Database=video_library_db;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;"))
            {
                conn.Open();
                var database = new Database(conn);

                var videos = database.Query<VideoPoco>("SELECT * FROM videos WHERE deleted=@d ORDER BY video_id DESC", new NpgsqlParameter("d", false));

                var videosInList = new List<string>();
                foreach (var videoPoco in videos)
                {
                    videosInList.Add(HtmlVideo.Create(videoPoco.Title, videoPoco.ChannelName, videoPoco.ThumbnailUrl, videoPoco.Description));
                }

                return HtmlDocument.Create(videosInList);
            }
        }

        private static string SaveUrl(YoutubeVideo video)
        {
            string connectionString = @"Server=vm5;Port=5437;Database=video_library_db;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var database = new Database(connection);

                var parametars = new []
                {
                    new NpgsqlParameter("t", video.Title),
                    new NpgsqlParameter("u", video.Url),
                    new NpgsqlParameter("d", video.Description),
                    new NpgsqlParameter("h", video.ThumbnailUrl),
                    new NpgsqlParameter("c", video.UploadedBy) 

                };

                var listOfVideos = database.Query<VideoPoco>("SELECT * FROM videos WHERE title=@t AND url=@u", new NpgsqlParameter("t", video.Title), new NpgsqlParameter("u", video.Url));

                if (listOfVideos.Count < 1)
                {
                    database.ExecuteNonQuery("INSERT INTO videos (title,url,description,thumbnail_url,channel_name) VALUES (@t,@u,@d,@h,@c)",parametars);
                }
                else
                {
                    Console.WriteLine("There is already video with the same url in the database!");
                }
            }

            return "Saved: " + video.Url;
        }

        private static void DeleteVideo(int id)
        {
            string connectionString = @"Server=vm5;Port=5437;Database=video_library_db;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var database = new Database(connection);

                var result = database.ExecuteNonQuery("UPDATE videos SET deleted=@d WHERE video_id=@i",
                    new NpgsqlParameter("d", true), new NpgsqlParameter("i", id));
                Console.WriteLine(result);
            }
        }
    }
}
