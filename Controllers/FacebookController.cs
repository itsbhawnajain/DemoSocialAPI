using System;
using System.Collections.Specialized;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using log4net;
using Newtonsoft.Json.Linq;
using RestSharp;
using DemoSocialAPI.Helper;
using DemoSocialAPI.Models;
using System.Configuration;

namespace DemoSocialAPI.Controllers
{
    public class FacebookController : Controller
    {
        private WebClient client = new WebClient();
        private JavaScriptSerializer serializer = new JavaScriptSerializer();
        private static readonly ILog log = LogManager.GetLogger(typeof(FacebookController));
        public ActionResult Index()
        {
            ViewBag.Title = "Facebook API Calls";
            return View();
        }

        #region Authentication
        public ActionResult getTokenFacebook()
        {
            Facebook LinkedIn = new Facebook();
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");

            var Url = FacebookHelper.GetToken();
            if (!string.IsNullOrEmpty(Url))
            {
                var content = client.DownloadString(Url);
                serializer.MaxJsonLength = Int32.MaxValue;
                LinkedIn = serializer.Deserialize<Facebook>(content);
                Session["Facebook_Token"] = LinkedIn.access_token;
                log.Info("Serialization Completed");
            
            }
            
                return Redirect("/facebook/");
        }

        public void oauth2callback()
        {
            var client = new RestClient("https://www.linkedin.com/oauth/v2");
            var request = new RestRequest(Method.POST);
            var request2 = new RestRequest("/accessToken?code=" + Request.QueryString["code"]);
            var Url = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/facebook/oauth2callback";

            request.AddParameter("redirect_uri", Url);
           request.AddParameter("client_id", ConfigurationManager.AppSettings["FacebookKey"]);
            request.AddParameter("client_secret", ConfigurationManager.AppSettings["FacebookSecret"]);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("authorization_code", "offline_access");
            var response2 = client.Execute(request2);
            if (response2.IsSuccessful)
            {
                var obj = JObject.Parse(response2.Content);
                Session["Facebook_Token"] = obj["access_token"].ToString();

            }
            if (Request.QueryString["access_token"] != null)
            {
                Session["Facebook_Token"] = Request.QueryString["access_token"].ToString();
            }
            Response.Redirect("/facebook/");
        }
        #endregion
        public JsonResult getProfile()//Without Postman
        {//https://graph.facebook.com/appid/accounts
            FacebookData FacebookData = new FacebookData();
           try
            {
                if (Session["Facebook_Token"] != null)
                {
                    var content = client.DownloadString("https://graph.facebook.com/459893707813555?access_token=" + Session["Facebook_Token"].ToString());
                    serializer.MaxJsonLength = Int32.MaxValue;
                    FacebookData = serializer.Deserialize<FacebookData>(content);
                    log.Info("Serialization Completed");
                }

            }
            catch (Exception ex)
            {
                log.Error("Error Message : " + ex.Message);
            }
            return Json(FacebookData, JsonRequestBehavior.AllowGet);

        }



    }
}
