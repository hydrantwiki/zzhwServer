using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Site.JsonObjects
{
    public class TagTableResponse
    {
        public string Result { get; set; }

        public TagTableResponse()
        {
            Data = new List<Tag>();
        }

        public List<Tag> Data { get; set; } 
    }
}