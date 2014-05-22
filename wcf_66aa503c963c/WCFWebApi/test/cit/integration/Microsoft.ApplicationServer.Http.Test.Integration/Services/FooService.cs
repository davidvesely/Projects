// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public class FooService
    {
        [WebInvoke(UriTemplate = "foo3", Method = "PUT")]
        [DataContractFormat]
        public foo PutFoo3(foo foo)
        {
            return foo;
        }

        [WebInvoke(Method = "POST", UriTemplate = "/voidTest/{test}")]
        public void PostVoid(int test)
        {
        }

        [WebGet(UriTemplate = "/nullableTest/{test}")]
        public Nullable<int> GetNullable(int test)
        {
            if (test == 0)
            {
                return null;
            }
            else
            {
                return new Nullable<int>(test);
            }
        }

        [WebInvoke(Method = "POST", UriTemplate = "/nullableTest/{test}")]
        public Nullable<int> PostNullable(int test)
        {
            if (test == 0)
            {
                return null;
            }
            else
            {
                return new Nullable<int>(test);
            }
        }

        [WebInvoke(Method="POST", UriTemplate = "/nullableNullTest")]
        public int PostNullableWithNull(Nullable<int> test)
        {
            if (!test.HasValue)
            {
                return -1;
            }
            else
            {
                return test.Value;
            }
        }
    }

    public class foo
    {
        public string Bar { get; set; }
    }
}
