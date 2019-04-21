using gt.entity.general.Model;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
//using ...add here the business namespace for the user interface


namespace gt.community.secure
{
    public class Utility : ApiController
    {

        #region attribute
        internal run gt;
        public string _pathsecure = string.Empty;
        internal bool isAuthenticate = false;
        internal string _ipUser = "";
        internal HttpException _ishttpclient = new HttpException(200, HttpStatusCode.OK.ToString());
        public static int? dateRefresh = null;
        #endregion

        public Utility()
        {

#if !DEBUG
               _pathsecure = "/Secure" ;          
#endif

        }
        /// <summary>
        /// Internal method for population and container customization of gt.business framework[layer.. [current]]
        /// </summary>
        /// <param name="_gt"> Injection Injection Reference for the business framework</param>
        [NotImplExceptionFilter]
        internal void Injected(ref run _gt)
        {
            this.gt = _gt;
            gt.applicationType = configBase.ApplicationType.MVC_API;
            Authenticate();
        }

        /// <summary>
        /// Private method of checking security status, logged-in user information
        /// </summary>
        private void Authenticate()
        {          
                if (gt.applicationType != configBase.ApplicationType.MVC_API)
                {
                    ModelCollection modelcollection = new ModelCollection();
                    gt.controlPopulation(ref modelcollection, Invoke.ORM.NameQuery.sel_adm_user_authenticate);
                }

                if (!Access()) {isAuthenticate = false;
                    _ishttpclient = new HttpException(403, HttpStatusCode.Forbidden.ToString());
                    throw new NotImplementedException(_ishttpclient.Message);
                }
                else isAuthenticate =  true;           
        }



        /// <summary>
        /// Get user value, items restricted to user current
        /// </summary>
        /// <param name="authenticData">Object type: String, json data represents</param>
        public void getInfo(string authenticData)
        {

            if (authenticData != null)
                if (authenticData != string.Empty)
                {
                    ModelCollection modelcollection = new ModelCollection();
                    modelcollection.AddModel(authenticData);
                    gt.controlPopulation(ref modelcollection, Invoke.ORM.NameQuery.sel_adm_user_authenticate);
                }

        }

        internal bool Access()
        {
            try
            {
                _ipUser = Settings.ClientAccess;
                gt.IpUser = _ipUser;
                if (Settings.IPDefault.Contains(_ipUser))
                    return verify.isDateToday();
            }
            catch { dateRefresh = null; }
            return false;

        }

        public async Task<IHttpActionResult> Redir<T>(ModelCollection objectcurren, T objectpredicate)
        {
            try
            {
                switch ((objectcurren as ModelCollection).GetModel<Client>().StatusCode)
                {
                    case HttpStatusCode.Continue:
                        return BadRequest();

                    case HttpStatusCode.OK:
                        return Ok((objectcurren as ModelCollection).GetModel<T>());
                }

            }
            catch (Exception) { }

            return InternalServerError();

        }

    }

   
}
