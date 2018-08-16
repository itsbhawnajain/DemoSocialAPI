using RestSharp;
using DemoSocialAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using log4net;
using System.Web.Script.Serialization;
using DemoSocialAPI.Helper;
using Newtonsoft.Json.Linq;

namespace DemoSocialAPI.Controllers
{
    public class TwitterController : Controller
    {
        private WebClient client = new WebClient();
        private JavaScriptSerializer serializer = new JavaScriptSerializer();
        private static readonly ILog log = LogManager.GetLogger(typeof(HomeController));

        public ActionResult getTokenTwitter()
        {
            var Url=TwitterHelper.GetToken();
            if (!string.IsNullOrEmpty(Url)){
                return Redirect(Url);
            }
            else
                return Redirect("/");
        }
        
       
        public void oauth2callback()
        {
            var client = new RestClient("https://api.twitter.com/oauth/access_token");
            var request2 = new RestRequest("?oauth_verifier=" + Request.QueryString["oauth_verifier"], Method.POST);
            //var request2 = new RestRequest("?oauth_token=" + Request.QueryString["oauth_token"].ToString(),Method.GET);
            //request2.AddParameter("grant_type", "authorization_code");
            //var Url = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/LinkedIn/oauth2callback";
            request2.AddParameter("x_auth_mode", "client_auth");
            var response2 = client.Execute(request2);
            if (response2.IsSuccessful)
            {
                var obj = JObject.Parse(response2.Content);
                if (obj["oauth_token"] != null)
                {
                    Session["Twitter_Token"] = obj["oauth_token"].ToString();
                    Session["Twitter_Token_s"] = obj["oauth_verifier"].ToString();
                }
            }
            Response.Redirect("/twitter/");

        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page for Twitter";
            return View();
        }

        public JsonResult getplayList()
        {
            //getTokenTwitter();
            LinkedIn LinkedIn = new LinkedIn();
            //client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            try
            {
                if (Session["Twitter_Token"] != null)
                {
                    //client.Headers.Add("x-li-format", "json");
                    var content = client.DownloadString("https://api.twitter.com/1.1/statuses/retweets_of_me.json?oauth_token=" + Session["Twitter_Token"].ToString()+"&oauth_token_secret="+ Session["Twitter_Token_s"].ToString());

                    //var content = client.DownloadString("https://api.twitter.com/1.1/statuses/retweets_of_me.json");
                    serializer.MaxJsonLength = Int32.MaxValue;
                    LinkedIn = serializer.Deserialize<LinkedIn>(content);
                    log.Info("Serialization Completed");
                }

            }
            catch (Exception ex)
            {
                log.Error("Error Message : " + ex.Message);
            }
            return Json(LinkedIn, JsonRequestBehavior.AllowGet);
        }

    }
}
