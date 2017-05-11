using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using TeamStreamApp.Models;
using TeamStreamApp.Repository;

namespace TeamStreamApp.Controllers
{
    //[Route("api/[controller]")]
    public class VisionAPIController : ApiController
    {
        private VisionRepository _visionRespository; 

        public VisionAPIController()
        {
            _visionRespository = new VisionRepository();
        }
        //// GET: api/Api
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Api/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Api
        [HttpPost]
        public void Post([FromBody]Models.VisionAnalysis model)
        {
            //Got the model...break up the calls.
            _visionRespository.InsertVision(model);
            
        }

        

        //// PUT: api/Api/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/Api/5
        //public void Delete(int id)
        //{
        //}
    }
}
