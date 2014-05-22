// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Common.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    [ServiceContract]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class ContactsService
    {
        static Dictionary<int, String> contacts = new Dictionary<int, string>();

        [WebInvoke(Method = "PUT", UriTemplate = "/Contacts/Id={id}")]
        HttpResponseMessage PutContact(string name, int id)
        {
            Console.WriteLine("Name:{0}, Id:{1}", name, id);
            contacts.Add(id, name);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [WebGet(UriTemplate = "/Contacts/{id}")]
        String GetContact(int id)
        {
            if (contacts.ContainsKey(id))
            {
                return contacts[id];
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }
    }
}
