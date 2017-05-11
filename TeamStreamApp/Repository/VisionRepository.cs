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
    public class VisionRepository : IVisionRepository
    {
        private readonly IDbConnection _db;

        public VisionRepository()
        {
            _db = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString);
        }

        public bool InsertVision(VisionAnalysis vision)
        {
            InsertMetadata(vision.metadata, vision.videoId, vision.thumbnailIndex);
            InsertDescription(vision.description, vision.videoId, vision.thumbnailIndex);
            InsertCaption(vision.description, vision.videoId, vision.thumbnailIndex);
            InsertVisionAnalysis(vision);

            return true;
        }

        public bool InsertMetadata(Metadata metadata, Guid videoId, int thumbIndex)
        {
            int rowsAffected = this._db.Execute(@"INSERT INTO [Metadata] ([videoId],[thumbnailIndex],[width],[height],[format]) " +
                "VALUES (@videoId, @thumbnailIndex,@width,@height,@format)",
                new { videoId = videoId, thumbnailIndex = thumbIndex, width = metadata.width, height = metadata.height, format = metadata.format });

            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
        }

        public bool InsertDescription(Description description, Guid videoId, int thumbIndex)
        {
            description.videoId = videoId;
            description.thumbnailIndex = thumbIndex;

            foreach (var item in description.tags)
            {

                this._db.Execute("INSERT INTO [Description]([videoId],[thumbnailIndex],[tags]) " +
                    "VALUES (@videoId, @thumbnailIndex, @tags)", 
                    new { videoId = videoId, thumbnailIndex = thumbIndex, tags = item });
            }

            //TO DO: Better Error Handling
            return true;
           
        }
        
        public bool InsertCaption(Description description, Guid videoId, int thumbIndex)
        {
            description.videoId = videoId;
            description.thumbnailIndex = thumbIndex;

            foreach (var item in description.captions)
            {

                this._db.Execute("INSERT INTO [Caption]([videoId],[thumbnailIndex],[text],[confidence]) " +
                    "VALUES (@videoId, @thumbnailIndex, @text, @confidence)", 
                    new { videoId = videoId, thumbnailIndex = thumbIndex, text = item.text, confidence = item.confidence });
            }

            //TO DO: Better Error Handling
            return true; 
        }

        public bool InsertVisionAnalysis(VisionAnalysis visionAnalysis)
        {
            int rowsAffected = this._db.Execute("INSERT INTO [VisionAnalysis] ([Id],[videoId],[requestId],[thumbnailUrl],[thumbnailIndex],[thumbnailCount]) " +
                "VALUES (@Id,@videoId,@requestId,@thumbnailUrl,@thumbnailIndex,@thumbnailCount)",
                new { Id = visionAnalysis.id, videoId = visionAnalysis.videoId, requestId = visionAnalysis.requestId, thumbnailUrl = visionAnalysis.thumbnailUrl, thumbnailIndex = visionAnalysis.thumbnailIndex, thumbnailCount = visionAnalysis.thumbnailCount });

            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
        }

        public void DeleteAllVision()
        {
            this._db.Execute("DELETE FROM [Caption]");
            this._db.Execute("DELETE FROM [Metadata]");
            this._db.Execute("DELETE FROM [Description]");
            this._db.Execute("DELETE FROM [VisionAnalysis]");
        }


    }
}