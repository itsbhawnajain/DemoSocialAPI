using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DemoSocialAPI.Helper
{
    public class YouTubeHelper
    {
        public static string GetToken()
        {
            var client = new RestClient("https://accounts.google.com/o/oauth2/v2/auth");
            var request = new RestRequest(Method.GET);
            var Url=HttpContext.Current.Request.UrlReferrer+"YouTube/oauth2callback";
            request.AddParameter("response_type", "code");
            request.AddParameter("redirect_uri", Url);
            request.AddParameter("client_id", ConfigurationManager.AppSettings["YouTubeKey"]);
            request.AddParameter("scope", "https://www.googleapis.com/auth/youtube.readonly");
            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return client.BuildUri(request).ToString();
            }
            else
            {
                return null;
            }


        }
    }
}