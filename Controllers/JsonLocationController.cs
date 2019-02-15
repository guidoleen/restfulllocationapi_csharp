using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using JsonObjectConvertor;
using LocationUtil;
using System.Web;
using System.Web.Http.Results;
using System.Web.Http.Cors;

namespace RestFullLocationApi.Controllers
{
    // [DisableCors]
    [EnableCors(origins: "https://localhost:44312", headers: "*", methods: "*")]
    [Route("api/location/{id}")]
    public class JsonLocationController : ApiController
    {
        private LocationDAO locdao = new LocationDAO(System.AppDomain.CurrentDomain.BaseDirectory + "App_Data/", "world.db" + "; Version = 3;");

        // GET api/<controller>
        [HttpGet]
        public JsonResult<String> Get() // IEnumerable<string> Get()
        {
            return Json("{}"); // Empty Json
        }

        // GET api/<controller>/5
        [HttpGet]
        public JsonResult<String> Get(int id)
        {
            // First get the location from db in an arraylist
            ArrayList loclist = this.locdao.getLocations(id);

            // Second do the json generator
            IObjectJson jsonGen;

            // OLD STUFF // String[] strJsonList = new string[loclist.Count];
            jsonGen = (ObjectToJson)new ObjectToJson(loclist.ToArray());

            return Json(jsonGen.setToJsonString());
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
            Console.Write(value);
        }

        // PUT api/<controller>/5
        // [DisableCors]
        // https://localhost:44312

        [HttpPut]
        public void Put(int id, [FromBody]string value)
        {
            // int _locid, double _lat, double _long, String _bertitle, String _bertext, int _berichtid
            this.locdao.save(new Location(0, 52.1, 4.8, "bla titel", "bla_text dus", 0, id));
            Console.Write(value + id.ToString());
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}