using System;
using System.Collections.Generic;
using TeamStreamApp.Models;

namespace TeamStreamApp.Repository
{
    internal interface IVisionRepository
    {
        bool InsertVision(VisionAnalysis vision);
    }
}