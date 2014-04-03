using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfServiceJson
{
    public class JsonTestService : IJsonTestService
    {
        public string GetData(string value)
        {
            return string.Format("You entered: {0}", value);
        }

        public string TestJsonValue(JsonObject test)
        {
            return test["test1"].ReadAs<string>();
        }
    }
}
