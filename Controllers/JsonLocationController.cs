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
using System.Net.Http.Formatting;
using System.Globalization;
using RestFullLocationApi.Model;

namespace RestFullLocationApi.Controllers
{
    // [DisableCors]
    [EnableCors(origins: "https://localhost:44312,http://localhost:58387", headers: "*", methods: "*")]
    [Route("api/location/{id}")]
    public class JsonLocationController : ApiController
    {
        private String SESSIONUSRID = "userid";
        private String SESSIONTOKEN = "sessiontoken";

        private int userId = -1;

        private static String strDir = System.AppDomain.CurrentDomain.BaseDirectory + "App_Data/";
        private static String strDb = "world.db" + "; Version = 3;";

        private LocationDAO locdao = new LocationDAO(strDir, strDb);
        private SessionLocationDAO sessdao = new SessionLocationDAO(strDir, strDb);
        private KlantDAO klantdao = new KlantDAO(strDir, strDb);

        SessionApi sessApi = new SessionApi();

        // GET api/<controller>
        [HttpGet]
        public JsonResult<String> Get() // IEnumerable<string> Get()
        {
            return Json("{}"); // Empty Json
        }

        // GET api/<controller>/5/1/1 => 1 and 1 have no meaning // DISPLAY CONTROLLER
        [Route("api/location/{id}/{noid}/{noid2}")]
        [HttpPost]
        public JsonResult<String> Get(int id, [FromBody] FormDataCollection _session)
        {
            String returnVal = "{" + "" + "}";
            Boolean validSession = false;
            try
            {
                validSession = this.sessdao.isValidSession(new SessionLocation(id, 
                                                            _session.GetValues("sessionid")[0],
                                                            _session.GetValues("sessiontoken")[0])
                                                           );
                if(validSession) 
                {
                    // First get the location from db in an arraylist
                    ArrayList loclist = this.locdao.getLocations(id);

                    // Second do the json generator
                    IObjectJson jsonGen;

                    // OLD STUFF // String[] strJsonList = new string[loclist.Count];
                    jsonGen = (ObjectToJson)new ObjectToJson(loclist.ToArray());
                    return Json(jsonGen.setToJsonString());
                }
            }
            catch(NullReferenceException ee)
            {
                return Json(returnVal);
            }
            return Json(returnVal);
        }

        // POST api/<controller> // UPDATE CONTROLLER
        [HttpPost]
        public string Post([FromBody] FormDataCollection location) // FormDataCollection
        {
            string strval = "";
            try
            {
                // int _locid, double _lat, double _long, String _bertitle, String _bertext, int _berichtid, int _klantid
                this.locdao.update(new Location(
                    int.Parse(location.GetValues("locid")[0]),
                    double.Parse(location.GetValues("latitude")[0], CultureInfo.InvariantCulture), 
                    double.Parse(location.GetValues("longitude")[0], CultureInfo.InvariantCulture),
                    location.GetValues("bertitel")[0],
                    location.GetValues("bertext")[0],
                    int.Parse(location.GetValues("berichtid")[0]),
                    int.Parse(location.GetValues("klantid")[0])
                    ));

                strval = location.GetValues("locid")[0] + " Lat " + location.GetValues("latitude")[0];

            }
            catch( Exception ee )
            {
                return ee.ToString();
            }
            
            return strval + " Gelukt";
        }

        // PUT api/<controller>/5 // INSERT CONTROLLER
        // [DisableCors]
        // https://localhost:44312

        // Put is the insert function //
        [HttpPut]
        public string Put(int id, [FromBody] FormDataCollection location)
        {
            string strVal = "";

            try
            {
                // int _locid, double _lat, double _long, String _bertitle, String _bertext, int _berichtid, int _klantid
                this.locdao.save(new Location(
                    int.Parse(location.GetValues("locid")[0]),
                    double.Parse(location.GetValues("latitude")[0], CultureInfo.InvariantCulture),
                    double.Parse(location.GetValues("longitude")[0], CultureInfo.InvariantCulture),
                    location.GetValues("bertitel")[0],
                    location.GetValues("bertext")[0],
                    int.Parse(location.GetValues("berichtid")[0]),
                    id
                    ));

                strVal = location.GetValues("locid")[0] + " Lat " + location.GetValues("latitude")[0];


            }
            catch (Exception ee)
            {
                return ee.ToString();
            }

            return strVal + "Insert gelukt";
        }

        // DELETE api/<controller>/5 // DELETE CONTROLLER
        [HttpDelete]
        public string Delete(int id, [FromBody] FormDataCollection location)
        {
            string strVal = "";

            try
            {
                // int _locid, double _lat, double _long, String _bertitle, String _bertext, int _berichtid, int _klantid
                this.locdao.delete(new Location(
                    int.Parse(location.GetValues("locid")[0]),
                    double.Parse(location.GetValues("latitude")[0], CultureInfo.InvariantCulture),
                    double.Parse(location.GetValues("longitude")[0], CultureInfo.InvariantCulture),
                    location.GetValues("bertitel")[0],
                    location.GetValues("bertext")[0],
                    int.Parse(location.GetValues("berichtid")[0]),
                    id
                    ));

                strVal = location.GetValues("locid")[0] + " BerichtId " + location.GetValues("berichtid")[0]
                    + " KlantId " + id; 
            }
            catch (Exception ee)
            {
                return ee.ToString();
            }

            return strVal + "Delete gelukt....";
        }

        // String conversion to double // NOT IN USE !!!!
        public double GetDouble(string value, double defaultValue)
        {
            double result;

            // Try parsing in the current culture
            if (!double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                // Then try in US english
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                // Then in neutral language
                !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        // LOGIN CONTROLLER
        [EnableCors(origins: "https://localhost:44312,http://localhost:58387", headers: "*", methods: "*")]
        [Route("api/location/{id}/{loginYN}")]
        [HttpPost]
        public JsonResult<String> Post(int id, int loginYN, [FromBody] FormDataCollection sessionVar)
        {
            int validKlant = 0;
            String strSessionId = "";
            String strSessionToken = "";

            if(loginYN == 0) // Logout is ok
            {
                this.sessApi.sessionClear();
                strSessionId = this.sessApi.sessionGetId();
                this.sessdao.deleteSession( new SessionLocation(id, "", "") );
            }   
            else // Login 
            {
                // Id is not possible
                validKlant = klantdao.isValidKlant( new Klant(0, "",
                                                    sessionVar.GetValues("pwd")[0],
                                                    sessionVar.GetValues("email")[0])
                                                    );

                if (validKlant != 0 ) // If klant is valid
                {
                    strSessionId = sessApi.sessionGetId();
                    strSessionToken = Encrypt.EncryptString(strSessionId, ""); // Encrypt the sessionId for token

                    //this.sessApi.sessionAdd(this.SESSIONUSRID, id.ToString());
                    //this.sessApi.sessionAdd(this.SESSIONTOKEN, strSessionToken);

                    // Save to the session
                    try
                    {
                        this.sessdao.saveSession(new SessionLocation(validKlant, strSessionId, strSessionToken)); // new SessionLocation(id, "Blala", "Blala"));
                    }
                    catch (Exception ee)
                    {
                        strSessionId = ee.ToString();
                    }
                }
            }
            
            return Json("[{ \"klantid\" :\" " + validKlant + "\" , \"sessionid\" :\" " + strSessionId + "\" , \"sessiontoken\": \"" + strSessionToken + "\"}]"); // Empty Json
        }
    }
}