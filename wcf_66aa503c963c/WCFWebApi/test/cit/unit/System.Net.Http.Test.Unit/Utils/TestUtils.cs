using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Net.Test.Common.Logging;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace System.Net.Test.Common
{
    internal static class TestUtils
    {
        public static string Format(string message, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, message, args);
        }

        public static void AcceptAllCertificates()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, 
                X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
        }

        public static T LoadXmlFromFile<T>(string filePath, XmlSchemaSet schema) where T : class
        {
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return LoadXmlFromStream<T>(stream, schema);
        }

        public static T LoadXmlFromStream<T>(Stream xmlStream, XmlSchemaSet schema) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlReader reader = null;

            if (schema != null)
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas = schema;
                settings.ValidationEventHandler += SchemaValidationFailed;

                reader = XmlReader.Create(xmlStream, settings);
            }
            else
            {
                reader = XmlReader.Create(xmlStream);
            }            

            T result = serializer.Deserialize(reader) as T;

            return result;
        }

        private static void SchemaValidationFailed(object sender, ValidationEventArgs e)
        {
            switch (e.Severity) 
            {
                case XmlSeverityType.Error:                    
                    Log.Error(e.Message);
                    break;
                case XmlSeverityType.Warning:
                    Log.Warn(e.Message);
                    break;
                default:
                    throw new InvalidOperationException(TestUtils.Format("Unknown XmlSeverityType value '{0}'",
                        Enum.GetName(typeof(XmlSeverityType), e.Severity)));
            }

            if (e.Exception != null)
            {
                throw e.Exception;
            }
        }
    }
}
