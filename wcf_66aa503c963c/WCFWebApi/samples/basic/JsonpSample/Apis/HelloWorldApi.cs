using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Web;

namespace JsonpSample.Apis
{
    public class HelloWorldApi
    {
        [WebGet(UriTemplate="hello{ext}")]
        public string GetHelloWorld()
        {
            return "Hello, World!";
        }
    }
}