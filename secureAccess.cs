
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace community.secure.validation
{
    public partial class SecureAccess : ActionFilterAttribute
    {

        private bool _isapi = true;
        public static int? dateRefresh = null;
        private string _ipUser = "";
        private List<string> _fail = new List<string>();
        private List<string> fail
        {
            get
            {
                if (_fail.Count == 0)
                {
                    _fail.Add("Without access authorization[" + _ipUser + "]");
                }
                return _fail;
            }
        }

        private bool _isUseCookie;
        public bool isCurrentCookie
        {
            get { return _isUseCookie; }
            set { _isUseCookie = value; }
        }



        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            try
            {

                if (!isticketValid())
                {
                    HttpContext.Current.Response.Redirect("/Login");
                }

                base.OnActionExecuting(filterContext);

            }
            catch (Exception e)
            {

                Register.Log(this, string.Format("*secureAccess - OnActionExecuting[1:{0} 2:{1} 3:{2}]", e.Message, e.Source, e.StackTrace));
            }

        }

        /// <summary>
        /// Verify ticket authentication of session user, case false is invalid
        /// </summary>
        /// <returns>Object type: Bollean</returns>
        private bool isticketValid()
        {

            try
            {
                if (!isPermition())
                {
                    _isUseCookie = isUseCookie();
                    if (HttpContext.Current.Session[Settings.gt_ticketAccess] != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Register.Log(this, string.Format("*secureAccess - isticketvalid [1:{0} 2:{1} 3:{2}]", e.Message, e.Source, e.StackTrace));
            }
            return false;
        }

        private bool isPermition()
        {
            if (_isapi)
                return Access();
            return true;
        }

        /// <summary>
        /// Verify ip user permition, type  case not applicaion  
        /// </summary>
        /// <returns></returns>
        private bool Access()
        {

            try
            {
                _ipUser = Settings.ClientAccess;

                if (Settings.IPDefault.Contains(_ipUser))
                {

                    return verify.isDateToday();
                }

            }
            catch { dateRefresh = null; }
            return false;

        }

    }
    
}



namespace community.secure.http
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Filters;

    public class NotImplExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public NotImplExceptionFilterAttribute()
        {

        }

        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is NotImplementedException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
        }
    }

}