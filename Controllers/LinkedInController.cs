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
    public class LinkedInController : Controller
    {
        private WebClient client = new WebClient();
        private JavaScriptSerializer serializer = new JavaScriptSerializer();
        private static readonly ILog log = LogManager.GetLogger(typeof(LinkedInController));
        public ActionResult Index()
        {
            ViewBag.Title = "LinkedIN Page";
            return View();
        }

        #region Authentication
        public ActionResult getTokenLinkedIn()
        {
            var Url = LinkedInHelper.GetToken();
            if (!string.IsNullOrEmpty(Url))
            {
                return Redirect(Url);
            }
            else
                return Redirect("/");
        }

        public void oauth2callback()
        {
            var client = new RestClient("https://www.linkedin.com/oauth/v2");
            var request = new RestRequest(Method.POST);
            var request2 = new RestRequest("/accessToken?code=" + Request.QueryString["code"]);
            request2.AddParameter("grant_type", "authorization_code");
            var Url = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/LinkedIn/oauth2callback";
            request2.AddParameter("redirect_uri", Url);
            request2.AddParameter("client_id", ConfigurationManager.AppSettings["LinkedINKey"]);
            request2.AddParameter("client_secret", ConfigurationManager.AppSettings["LinkedINSecret"]);
            var response2 = client.Execute(request2);
            if (response2.IsSuccessful)
            {
                var obj = JObject.Parse(response2.Content);
                Session["Linkedin_Token"] = obj["access_token"].ToString();
                if (Request.QueryString["access_token"] != null)
                {
                    Session["Linkedin_Token"] = Request.QueryString["access_token"].ToString();
                }
            }

            Response.Redirect("/linkedin/");
        }
        #endregion
        public JsonResult profile()//Without Postman
        {
            LinkedIn LinkedIn = new LinkedIn();
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");
            try
            {
                if (Session["Linkedin_Token"] != null)
                {
                    client.Headers.Add("x-li-format", "json");
                    var content = client.DownloadString("https://api.linkedin.com/v1/people/~?oauth2_access_token=" + Session["Linkedin_Token"].ToString());
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

        public JsonResult LinkedIndata()//With Postman
        {
            LinkedIn LinkedInData = new LinkedIn();
            try
            {
                var client = new RestClient("https://api.linkedin.com/v1/people/~?format=json");
                var request = new RestRequest(Method.GET);
                request.AddHeader("Postman-Token", "e2f54cdf-c9d3-47a1-8c49-c07dc09c76c9");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Authorization", "Bearer AQUpylIsxl51pKuB_vw-ENuWZxgkLSDaJxV-Pzjqdcx5EW_vPgCNAG1rpRYBQZIrVxWX8_SPrSGAH8sraN7KpfWxAQaPHpMPZhAVxFb860MgxjJHclDm7Ns9OImy79bqaguIyXx6er6FfZwLsMC8snP3ByDGK-Lnr4BItMNzsxs06BrmuGabZ-XM2XC74CalsUGzhHphhDNFH-HK8itIfJBmzseQquxd5CvPLqx-7CytpUH1SXh9jyYr-SqZvi99HoEki8tIizmrzCIUJ_ABZpfc1ehrFUxu7wOVVhnFsNYLRWuA64FbwypokE_ZyIBi-IZjsk5ahgRTnoN1PyZqqcuvAU4z3A");
                IRestResponse response = client.Execute(request);
                LinkedInData = serializer.Deserialize<LinkedIn>(response.Content);

            }
            catch (Exception ex)
            {
                log.Error("Error Message : " + ex.Message);
            }
            return Json(LinkedInData, JsonRequestBehavior.AllowGet);
        }

    }
}
