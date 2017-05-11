using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Dapper;
using TeamStreamApp.Models;

namespace TeamStreamApp.Repository
{
    public class VideoRepository : IVideoRepository
    {
        private readonly IDbConnection _db;

        public VideoRepository()
        {
            _db = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString);
        }

        public List<Video> GetVideos()
        {
            return this._db.Query<Video>("SELECT TOP 10 [Id],[Name],[Keywords],[RawUrl],[Date] FROM [Video]").ToList();
        }

        public Video GetVideo(Guid videoId)
        {
            return _db.Query<Video>("SELECT[Id],[Name],[Keywords],[RawUrl],[Date] FROM [Video] WHERE Id =@VideoId", new { VideoId = videoId }).SingleOrDefault();
        }

        public bool InsertVideo(Video video)
        {
            int rowsAffected = this._db.Execute(@"INSERT Video([Id],[Name],[Keywords],[RawUrl],[Date]) values (@Id, @Name, @Keywords, @RawUrl, @Date)",
                new { Id = video.Id, Name = video.Name, Keywords = video.Keywords, RawUrl = video.RawUrl, Date = video.Date });

            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
        }
    }
}