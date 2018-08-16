using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using DemoSocialAPI.Helper;
using DemoSocialAPI.Models;
using System.Configuration;

namespace DemoSocialAPI.Controllers
{
    public class YouTubeController : Controller
    {
        private WebClient client = new WebClient();
        private JavaScriptSerializer serializer = new JavaScriptSerializer();
        private static readonly ILog log = LogManager.GetLogger(typeof(HomeController));
        public ActionResult Index()
        {
            ViewBag.Title = "YouTube API Calls";
            return View();
        }

        public ActionResult getTokenYouTube()
        {
            var Url = YouTubeHelper.GetToken();
            if (!string.IsNullOrEmpty(Url))
            {
                return Redirect(Url);
            }
            else
                return Redirect("/");
        }

        public void oauth2callback()
        {
            var client = new RestClient("https://www.googleapis.com/oauth2/v4/token");
            //var Url = HttpContext.Current.Request.UrlReferrer + "YouTube/oauth2callback";
            var Url = System.Web.HttpContext.Current.Request.Url.Scheme+"://"+System.Web.HttpContext.Current.Request.Url.Authority + "/YouTube/oauth2callback"; 
            var request2 = new RestRequest("?code=" + Request.QueryString["code"], Method.POST);
            request2.AddParameter("grant_type", "authorization_code");
            request2.AddParameter("redirect_uri", Url);
            request2.AddParameter("client_id", ConfigurationManager.AppSettings["YouTubeKey"]);
            request2.AddParameter("client_secret", ConfigurationManager.AppSettings["YouTubeSecret"]);
            var response2 = client.Execute(request2);
            if (response2.IsSuccessful)
            {
                var obj = JObject.Parse(response2.Content);
                Session["YouTube_Token"] = obj["access_token"].ToString();
            }
            Response.Redirect("/youtube/");
        }

        public JsonResult YouTubeData()//Without Postman
        {
            //https://developers.google.com/youtube/v3/docs/activities/list
            YoutubeData YoutubeData = new YoutubeData();
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            try
            {
                if (Session["YouTube_Token"] != null)
                {
                    client.Headers.Add("x-li-format", "json");
                    client.Headers.Add("x-original-host", "ngrok.io");
                    //client.Headers.Add("part", "contentDetails");
                    //client.Headers.Add("home", "true");
                    var content = client.DownloadString("https://www.googleapis.com/youtube/v3/activities/?access_token=" + Session["YouTube_Token"].ToString()+ "&part=contentDetails&home=true");
                    serializer.MaxJsonLength = Int32.MaxValue;
                    YoutubeData = serializer.Deserialize<YoutubeData>(content);
                    log.Info("Serialization Completed");
                }

            }
            catch (Exception ex)
            {
                log.Error("Error Message : " + ex.Message);
            }
            return Json(YoutubeData, JsonRequestBehavior.AllowGet);

        }
                
        //public JsonResult YouTubedata()//With Postman
        //{
        //    YoutubeData YoutubeData = new YoutubeData();
        //    try
        //    {
        //        var client = new RestClient("https://api.linkedin.com/v1/people/~?format=json");
        //        var request = new RestRequest(Method.GET);
        //        request.AddHeader("Postman-Token", "e2f54cdf-c9d3-47a1-8c49-c07dc09c76c9");
        //        request.AddHeader("Cache-Control", "no-cache");
        //        request.AddHeader("Authorization", "Bearer AQUpylIsxl51pKuB_vw-ENuWZxgkLSDaJxV-Pzjqdcx5EW_vPgCNAG1rpRYBQZIrVxWX8_SPrSGAH8sraN7KpfWxAQaPHpMPZhAVxFb860MgxjJHclDm7Ns9OImy79bqaguIyXx6er6FfZwLsMC8snP3ByDGK-Lnr4BItMNzsxs06BrmuGabZ-XM2XC74CalsUGzhHphhDNFH-HK8itIfJBmzseQquxd5CvPLqx-7CytpUH1SXh9jyYr-SqZvi99HoEki8tIizmrzCIUJ_ABZpfc1ehrFUxu7wOVVhnFsNYLRWuA64FbwypokE_ZyIBi-IZjsk5ahgRTnoN1PyZqqcuvAU4z3A");
        //        IRestResponse response = client.Execute(request);
        //        YoutubeData = serializer.Deserialize<YoutubeData>(response.Content);

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Error Message : " + ex.Message);
        //    }
        //    return Json(YoutubeData, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult youtubedata2()//oauth1
        {
            YoutubeData youtubeData = new YoutubeData();
            client.Headers.Add("X-Language", "en");
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            try
            {
                var content = client.DownloadString("https://www.googleapis.com/youtube/v3/search?key=AIzaSyB_mVYswwrfIosYeR-UUYBamBeXOyyk7fM&channelId=UCyS42OgDpiOrFw9_u5bGotQ&part=snippet,id&order=date&maxResults=20");
                serializer.MaxJsonLength = Int32.MaxValue;
                youtubeData = serializer.Deserialize<YoutubeData>(content);
                log.Info("Serialization Completed");
            }
            catch (Exception ex)
            {
                log.Error("Error Message : " + ex.Message);
            }
            return Json(youtubeData, JsonRequestBehavior.AllowGet);

        }

    }
}
