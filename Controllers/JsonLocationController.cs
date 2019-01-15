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

namespace RestFullLocationApi.Controllers
{
    [Route("api/location")]
    public class JsonLocationController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        public JsonResult<String> Get() // IEnumerable<string> Get()
        {
            // First get the location from db in an arraylist
            LocationDAO locdao = new LocationDAO(System.AppDomain.CurrentDomain.BaseDirectory + "App_Data/", "world.db" + "; Version = 3;");
            ArrayList loclist = locdao.getLocations();

            // Second do the json generator
            IObjectJson jsonGen;

            // OLD STUFF // String[] strJsonList = new string[loclist.Count];
            string strJsonList = "";
            foreach (Location item in loclist)
            {
                jsonGen = (ObjectToJson)new ObjectToJson(item);
                strJsonList += jsonGen.setToJsonString(); // OLD STUFF // strJsonList[i]
            }

            return Json(strJsonList);
        }

        // GET api/<controller>/5
        [HttpGet]
        public string Get(int id)
        {
            return "value" + id;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}