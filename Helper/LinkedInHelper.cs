using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DemoSocialAPI.Helper
{
    public class LinkedInHelper
    {
        public static string GetToken()
        {
            var client = new RestClient("https://www.linkedin.com/oauth/v2/authorization");
            var request = new RestRequest(Method.GET);
            request.AddParameter("response_type", "code");
            var Url = HttpContext.Current.Request.UrlReferrer + "LinkedIn/oauth2callback";
            request.AddParameter("redirect_uri", Url);
            request.AddParameter("client_id", ConfigurationManager.AppSettings["LinkedINKey"]);
            request.AddParameter("state", "81hoaydxwht5yz123456");
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