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



            throw new NotImplementedException();
        }

        private static void InsertMetadata(Metadata metadata)
        {

        }
    }
}