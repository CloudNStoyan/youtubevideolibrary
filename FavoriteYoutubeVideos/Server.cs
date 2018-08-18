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

                    try
                    {
                        Console.WriteLine("Connected!");
                        string page = context.Request.RawUrl;
                        Console.WriteLine(page);

                        if (page == "/")
                        {
                            string request = GetRequest(context);
                            var video = new YoutubeVideo(request);

                            SaveUrl(video.Title, video.Url, video.Description, video.ThumbnailUrl, video.UploadedBy);

                        } else if (page == "/videos")
                        {
                            string response = GetResponse();
                            context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(response);
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            using (Stream stream = context.Response.OutputStream)
                            {
                                using (StreamWriter writer = new StreamWriter(stream))
                                {
                                    writer.Write(response);
                                }
                            }
                        } else if (page == "/delete")
                        {
                            string request = GetRequest(context);
                            DeleteVideo(int.Parse(request));

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string GetResponse()
        {
            string response = string.Empty;

            using (var conn = new NpgsqlConnection(@"Server=vm5;Port=5437;Database=video_library_db;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;"))
            {
                conn.Open();
                var database = new Database(conn);

                var videos = database.Query<VideoPoco>("SELECT * FROM videos WHERE deleted=@d ORDER BY video_id DESC",new NpgsqlParameter("d",false));
                int count = 1;

                var videosInList = new List<string>();
                foreach (var videoPoco in videos)
                {
                    /*if (count % 2 == 0)
                    {
                        response += "<div class='video'>";
                    }
                    else
                    {
                        response += "<div class='video' style='background-color: GhostWhite;'>";
                    }

                    count++;

                    response += $"<div><h2><a href='#' class='title' url-src='{videoPoco.Url}'> {videoPoco.Title} </a> </h2><a class='delete-btn' href='#' video-id='{videoPoco.VideoId}'>X</a></div>";
                    string thumbSrc = videoPoco.ThumbnailUrl != "no thumb url" ? videoPoco.ThumbnailUrl : "https://i.ytimg.com/vi/aaaaaaaaaaa/default.jpg";
                    response += $"<img class='thumbnail' src='{thumbSrc}' title='{videoPoco.ChannelName}'/>";
                    string description = videoPoco.Description.Length > 60
                        ? videoPoco.Description.Substring(0, 60).Trim() + "..."
                        : videoPoco.Description.Trim();
                    response += $"<p style='width: 500px'>{description}</p></div>";
                    */
                    
                    videosInList.Add(HtmlVideo.Create(videoPoco.Title,videoPoco.ChannelName,videoPoco.ThumbnailUrl,videoPoco.Description));
                }

                response = HtmlDocument.Create(videosInList);
            }


            return response;
        }

        private static void SaveUrl(string title,string url,string description,string thumbnailUrl,string channelName)
        {
            string connectionString = @"Server=vm5;Port=5437;Database=video_library_db;Uid=postgres;Pwd=9ae51c68-c9d6-40e8-a1d6-a71be968ba3e;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var database = new Database(connection);

                var parametars = new []
                {
                    new NpgsqlParameter("t", title),
                    new NpgsqlParameter("u", url),
                    new NpgsqlParameter("d", description),
                    new NpgsqlParameter("h", thumbnailUrl),
                    new NpgsqlParameter("c", channelName) 

                };

                var listOfVideos = database.Query<VideoPoco>("SELECT * FROM videos WHERE title=@t AND url=@u", new NpgsqlParameter("t", title), new NpgsqlParameter("u", url));

                if (listOfVideos.Count < 1)
                {
                    database.ExecuteNonQuery("INSERT INTO videos (title,url,description,thumbnail_url,channel_name) VALUES (@t,@u,@d,@h,@c)",parametars);
                }
                else
                {
                    Console.WriteLine("There is already video with the same url in the database!");
                }
            }
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

        private static string GetRequest(HttpListenerContext context)
        {
            var request = context.Request;
            string text;
            using (var reader = new StreamReader(request.InputStream,
                request.ContentEncoding))
            {
                text = reader.ReadToEnd();
            } 

            return text;
        }
    }
}
