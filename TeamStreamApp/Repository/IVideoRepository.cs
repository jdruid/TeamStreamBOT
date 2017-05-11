using System;
using System.Collections.Generic;
using TeamStreamApp.Models;

namespace TeamStreamApp.Repository
{
    internal interface IVideoRepository
    {
        List<Video> GetVideos();

        Video GetVideo(Guid videoId);

        bool InsertVideo(Video video);

    }
}