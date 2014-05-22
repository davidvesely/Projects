// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData.Test.Integration
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public class TestService
    {
        [WebGet(UriTemplate = "Get/{id}")]
        string Get(string id)
        {
            return string.Format("Get with id:{0}", id);
        }

        [WebGet(UriTemplate = "GetPerson")]
        public Person GetPerson()
        {
            Person obj = new Person() { MyGuid = new Guid("f99080c0-2f9e-472e-8c72-1a8ecd9f902d"), PerId = 10, Age = 10, Name = "Asha", Order = new Order() { OrderName = "FirstOrder", OrderAmount = 235342 } };
            return obj;
        }

        
        [WebGet(UriTemplate = "GetOrders")]
        public List<Order> GetOrdersAsIEnumerable()
        {
            List<Order> myList = new List<Order>();
            myList.Add(new Order() { OrderName = "FirstOrder", OrderAmount = 11, });
            myList.Add(new Order() { OrderName = "SecondOrder" , OrderAmount = 12, });
            return myList;
        }

        [WebGet(UriTemplate = "GetInt")]
        public int GetInt()
        {
            return 5;
        }
    }
}
