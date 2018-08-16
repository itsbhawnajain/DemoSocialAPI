using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DemoSocialAPI.Helper
{
    public class TwitterHelper
    {
        public static string GetToken()
        {
            
            string key = ConfigurationManager.AppSettings["TwitterKey"];
            string SecretKey = ConfigurationManager.AppSettings["TwitterSecret"];
            string CallBackUrl = HttpContext.Current.Request.UrlReferrer + "Twitter/oauth2callback";
            var client = new RestClient("https://api.twitter.com");
            client.Authenticator = OAuth1Authenticator.ForRequestToken(key, SecretKey, CallBackUrl);
            var request = new RestRequest("/oauth/request_token", Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            var response = client.Execute(request);
            var qs=HttpUtility.ParseQueryString(response.Content);
            
            var request2 = new RestRequest("/oauth/authorize?oauth_token=" + qs["oauth_token"]);
                if (response.IsSuccessful)
            {
                return client.BuildUri(request2).ToString();
            }
            else
            {
                return null;
            }

        }
    }
}