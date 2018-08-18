using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace FavoriteYoutubeVideos
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Run("http://localhost:2905/");
        }
    }
}
