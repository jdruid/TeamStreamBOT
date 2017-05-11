using System;
using System.Collections.Generic;
using TeamStreamApp.Models;

namespace TeamStreamApp.Repository
{
    internal interface IVisionRepository
    {
        bool InsertVision(VisionAnalysis vision);
        bool InsertMetadata(Metadata metadata, Guid videoId, int thumbIndex);
        bool InsertDescription(Description description, Guid videoId, int thumbIndex);
        bool InsertCaption(Description description, Guid videoId, int thumbIndex);
        bool InsertVisionAnalysis(VisionAnalysis visionAnalysis);
       
        void DeleteAllVision();
    }
}