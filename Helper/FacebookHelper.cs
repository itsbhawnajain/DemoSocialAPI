using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DemoSocialAPI.Helper
{
    public class FacebookHelper
    {
        public static string GetToken()
        {
            var client = new RestClient("https://graph.facebook.com/v3.1");
            var request = new RestRequest("/oauth/access_token", Method.GET);
            var Url = HttpContext.Current.Request.UrlReferrer + "facebook/oauth2callback";
            request.AddParameter("redirect_uri", Url);
            request.AddParameter("client_id", ConfigurationManager.AppSettings["FacebookKey"]);
            request.AddParameter("client_secret", ConfigurationManager.AppSettings["FacebookSecret"]);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("authorization_code", "offline_access");
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