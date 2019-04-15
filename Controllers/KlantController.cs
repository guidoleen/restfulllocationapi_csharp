using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using LocationUtil;

namespace RestFullLocationApi.Controllers
{
    [EnableCors(origins: "https://localhost:44312,http://localhost:58387", headers: "*", methods: "*")]
    [Route("api/klant")]
    public class KlantController : ApiController
    {
        private static String strDir = System.AppDomain.CurrentDomain.BaseDirectory + "App_Data/";
        private static String strDb = "world.db" + "; Version = 3;";

        private IDAOKlant klantDao = new KlantDAO(strDir, strDb);
        private LocationUtils locUtil = new LocationUtils();

        // INSERT KLANT
        [Route("api/klant")]
        [HttpPost]
        public JsonResult<String> Post([FromBody] FormDataCollection _klantVars)
        {
            String succesMessage = "Insert gelukt";
            String strInsertM = "";

            String naam = _klantVars.GetValues("naam")[0];
            String pwd = _klantVars.GetValues("pwd")[0];
            String email = _klantVars.GetValues("email")[0];

            // Check if values are empty...
            if( locUtil.isEmpty(naam) && locUtil.isEmpty(pwd) && locUtil.isEmpty(email) )
                return Json("[{ \"message\" :\" " + "empty values" + "\"}]");

            try
            {
                strInsertM = this.klantDao.insert(new Klant(
                            0,
                            naam,
                            pwd,
                            email
                            ),
                            succesMessage
                            );
            }
            catch (NullReferenceException ee)
            {
                return Json("{" + ee.ToString() + "}");
            }

            return Json("[{ \"message\" :\" " + strInsertM + "\"}]"); // Empty Json
            
        }

    }
}

