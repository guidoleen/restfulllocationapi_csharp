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

        // INSERT KLANT
        [Route("api/klant")]
        [HttpPost]
        public JsonResult<String> Post([FromBody] FormDataCollection _klantVars)
        {
            String succesMessage = "Insert gelukt";
            String strInsertM = "";
            try
            {
                strInsertM = this.klantDao.insert(new Klant(
                            0,
                            _klantVars.GetValues("naam")[0],
                            _klantVars.GetValues("pwd")[0],
                            _klantVars.GetValues("email")[0]
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

