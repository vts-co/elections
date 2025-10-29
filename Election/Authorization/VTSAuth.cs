using Election.Dto;
using Election.Utilities;
using System;
using System.Globalization;
using System.Threading;
using System.Web;


namespace Election.Authorization
{
    public class VTSAuth
    {
        readonly string cookiename = "Maain";
        readonly int cookieKeyCount = 1;
        readonly int cookieDays = 30;

        public UserInfo CookieValues { get; set; }
        //private Guid SessionID { get; set; }


        /// <summary>
        /// check if the cookies has values and the values count is equal to or bigger than the cookie keys count
        /// </summary>
        public bool CheckCookies() => HttpContext.Current.Request.Cookies[cookiename] != null && HttpContext.Current.Request.Cookies[cookiename].Values.Count >= cookieKeyCount;
        /// <summary>
        /// Load Data saved in the cookies
        /// </summary>
        /// <returns>true if data existed in the cookies</returns>
        public bool LoadDataFromCookies()
        {
            if (CheckCookies())
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
                CookieValues = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(Security.Decrypt(cookie.Values[0]));
                return true;
            }
            return false;
        }
        public string LoadDataFromCookies1()
        {
            if (CheckCookies())
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
                var values = cookie.Values[0];
                return values;
            }
            return "";
        }

        internal void ClearCookies()
        {
            if (HttpContext.Current.Request.Cookies[cookiename] != null)
            {
                HttpContext.Current.Response.Cookies[cookiename].Expires = AppUtility.GetDateTime().AddDays(-1);
            }
        }

        public void ChangeLanguage(string lang)
        {
            if (lang=="ar")
            {
                lang = "ar-EG";
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            HttpCookie cookie = new HttpCookie("Lang");
            cookie.Value = lang;
            cookie.Expires = DateTime.Now.AddMonths(3);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        internal bool SaveToCookies(UserInfo cookieValues)
        {
            if (cookieValues != null)
            {
                HttpCookie cookie = new HttpCookie(cookiename);
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(cookieValues);
                cookie.Values.Add("k0", Security.Encrypt(jsonString.ToString()));
                cookie.Expires = AppUtility.GetDateTime().AddDays(cookieDays);
                HttpContext.Current.Response.Cookies.Add(cookie);
                return true;
            }
            return false;
        }

        internal bool SaveToCookiesFromSessionData(string sessionData)
        {
            if (!string.IsNullOrEmpty(sessionData))
            {
                HttpCookie cookie = new HttpCookie("sessionData");
                //string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(cookieValues);
                cookie.Values.Add("k0", sessionData);
                cookie.Expires = AppUtility.GetDateTime().AddDays(cookieDays);
                HttpContext.Current.Response.Cookies.Add(cookie);
                return true;
            }
            return false;
        }

        public static bool UpdateUserInfo(string firstName,string lastName)
        {
            var auth = new VTSAuth();
            if (auth.LoadDataFromCookies())
            {
                auth.CookieValues.FirstName = firstName;
                auth.CookieValues.LastName = lastName;
                auth.SaveToCookies(auth.CookieValues);
                return true;
            }
            return false;
        }
    }






}