using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Site.JsonObjects
{
    public class ReviewTag : Tag
    {
        public string ReviewButton { get; set; }

        public string Username { get; set; }

        public ReviewTag(HydrantWiki.Library.Objects.Tag _tag)
            : base(_tag, false, false)
        {
            ReviewButton = string.Format("<a class=\"btn btn-info\" href=\"/reviewtag/{0}\">Review</a>", _tag.Guid);
        }
    }
}