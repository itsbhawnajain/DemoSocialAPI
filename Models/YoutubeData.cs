using System;
using System.Collections.Generic;

namespace DemoSocialAPI.Models
{
    public class YoutubeData
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextPageToken { get; set; }
        public PageInfo pageInfo { get; set; }
        public List<Item> items { get; set; }
    }
    public class PageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class Upload
    {
        public string videoId { get; set; }
    }

    public class ContentDetails
    {
        public Upload upload { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public ContentDetails contentDetails { get; set; }
    }
}

