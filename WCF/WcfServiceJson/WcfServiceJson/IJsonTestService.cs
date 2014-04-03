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
    [ServiceContract]
    public interface IJsonTestService
    {

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetData/{value}")]
        string GetData(string value);

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string TestJsonValue(JsonObject test);

    }

    [Serializable]
    public class JsonDictionary : ISerializable
    {
        private Dictionary<string, object> m_entries;

        public JsonDictionary()
        {
            m_entries = new Dictionary<string, object>();
        }

        public IEnumerable<KeyValuePair<string, object>> Entries
        {
            get { return m_entries; }
        }

        protected JsonDictionary(SerializationInfo info, StreamingContext context)
        {
            m_entries = new Dictionary<string, object>();
            foreach (var entry in info)
            {
                m_entries.Add(entry.Name, entry.Value);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var entry in m_entries)
                info.AddValue(entry.Key, entry.Value);
        }
    }
}
