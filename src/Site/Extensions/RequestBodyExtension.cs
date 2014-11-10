using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Nancy.IO;

namespace Site.Extensions
{
    public static class RequestBodyExtensions
    {
        public static string ReadAsString(this RequestStream _requestStream)
        {
            using (var reader = new StreamReader(_requestStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}