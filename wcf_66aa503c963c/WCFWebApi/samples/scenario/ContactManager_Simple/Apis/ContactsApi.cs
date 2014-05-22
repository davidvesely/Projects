// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager_Simple
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel.Web;

    public class ContactsApi
    {
        private readonly IContactRepository repository;

        public ContactsApi():this(new ContactRepository())
        {
        }

        public ContactsApi(IContactRepository repository)
        {
            this.repository = repository;
        }
        
        [WebGet]
        public List<Contact> Get()
        {
            return this.repository.GetAll();
        }

        [WebInvoke]
        public HttpResponseMessage<Contact> Post(Contact contact)
        {
            this.repository.Post(contact);
            var response = new HttpResponseMessage<Contact>(contact);
            response.StatusCode = HttpStatusCode.Created;
            return response;
        }
    }
}