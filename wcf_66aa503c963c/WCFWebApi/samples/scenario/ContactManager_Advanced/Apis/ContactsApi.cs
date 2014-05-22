// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ContactManager_Advanced
{
    using System.Linq;
    using System.ComponentModel.Composition;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel.Web;

    [Export]
    public class ContactsApi
    {
        private readonly IContactRepository repository;

        [ImportingConstructor]
        public ContactsApi(IContactRepository repository)
        {
            this.repository = repository;
        }
        
        [WebGet]
        public IQueryable<Contact> Get()
        {
            return this.repository.GetAll().AsQueryable();
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