using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;


namespace Election.Utilities
{
    public static class Buttons
    {
        public static MvcHtmlString Create(this UrlHelper urlHelper, string action = "Create", string controller = "", object routeValues = null)
        {
            return new MvcHtmlString($"<a href='{(string.IsNullOrWhiteSpace(controller) ? urlHelper.Action(action, routeValues) : urlHelper.Action(action, controller, routeValues))}' class='btn btn-soft-primary  waves-effect waves-light px-4'><i class=\"fas fa-plus me-3\"></i> انشاء جديد</a>");
        }
      
        public static MvcHtmlString Details(this UrlHelper urlHelper, string action = "Details", string controller = "", object routeValues = null)
        {
            return new MvcHtmlString($"<a href='{(string.IsNullOrWhiteSpace(controller) ? urlHelper.Action(action, routeValues) : urlHelper.Action(action, controller, routeValues))}' title='التفاصيل' class='btn btn-soft-primary  waves-effect waves-light p-2'><i class=\"fas fa-eye\"></i></a>");
        }
        public static MvcHtmlString Edit(this UrlHelper urlHelper, string action = "Edit", string controller = "", object routeValues = null)
        {
            return new MvcHtmlString($"<a href='{(string.IsNullOrWhiteSpace(controller) ? urlHelper.Action(action, routeValues) : urlHelper.Action(action, controller, routeValues))}' title='تعديل' class='btn btn-soft-success  waves-effect waves-light p-2'><i class=\"fas fa-edit\"></i></a>");
        }
        public static MvcHtmlString Delete(this UrlHelper urlHelper, string action = "Delete", string controller = "", object routeValues = null)
        {
            return new MvcHtmlString($"<a href='{(string.IsNullOrWhiteSpace(controller) ? urlHelper.Action(action, routeValues) : urlHelper.Action(action, controller, routeValues))}' title='حذف' class='btn btn-soft-danger   waves-effect waves-light p-2' onclick = \"return deleteCheck(this)\"><i class=\"fas fa-trash\"></i></a>");
        }
       
        public static MvcHtmlString BackToList(this UrlHelper urlHelper, string action = "Index", string controller = "", object routeValues = null)
        {
            return new MvcHtmlString($"<a href='{(string.IsNullOrWhiteSpace(controller) ? urlHelper.Action(action, routeValues) : urlHelper.Action(action, controller, routeValues))}' title='عودة' class='btn btn-soft-info  waves-effect waves-light m-1 col-md-auto px-3'><i class=\"fas fa-arrow-left me-1\"></i>عودة</a>");
        }

        public static MvcHtmlString SubmitCreate(this UrlHelper urlHelper)
        {
            return new MvcHtmlString($"<button type='submit' class='btn btn-primary  waves-effect waves-light px-3 m-1 col-12 col-md-auto'><i class='fas fa-plus me-1'></i>انشاء جديد</button>");
        }
        public static MvcHtmlString SubmitAdd(this UrlHelper urlHelper)
        {
            return new MvcHtmlString($"<button type='submit' class='btn btn-primary  waves-effect waves-light px-3 m-1 col-12 col-md-auto'><i class='fas fa-plus me-1'></i>اضافة</button>");
        }
        public static MvcHtmlString SubmitSave(this UrlHelper urlHelper)
        {
            return new MvcHtmlString($"<button type='submit' class='btn btn-success  waves-effect waves-light px-3 m-1 col-12 col-md-auto'><i class='fas fa-save me-1'></i>حفظ التعديلات</button>");
        }
    }
}