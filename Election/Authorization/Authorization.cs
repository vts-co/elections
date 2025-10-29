using Election.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;


namespace Election.Authorization
{
    public class Authorized : ActionFilterAttribute, IExceptionFilter
    {
        bool OutMenu = false;
        public Authorized()
        {
            OutMenu = false;
        }
       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          
            string controller = filterContext.RequestContext.RouteData.Values["controller"].ToString();

            string action = filterContext.RequestContext.RouteData.Values["action"].ToString();

          
            if (controller != null)
            {


                VTSAuth auth = new VTSAuth() { CookieValues = new UserInfo { } };



                var check = auth.LoadDataFromCookies();
                if (check)
                {

                }
                else
                {

                }
            
                filterContext.Controller.TempData["UserInfo"] = auth.CookieValues;
                filterContext.Controller.TempData["UserId"] = auth.CookieValues.UserId;
                filterContext.Controller.TempData["PersonId"] = auth.CookieValues.PersonId;
                filterContext.Controller.TempData["UserName"] = auth.CookieValues.FirstName;
          

            }

           
        }

        public void OnException(ExceptionContext filterContext)
        {
         
            //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "", controller = "Account", action = "SignIn", returnUrl = filterContext.HttpContext.Request.Url.ToString() }));
            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult() { ViewName = "Error" };
        }
    }

   
}