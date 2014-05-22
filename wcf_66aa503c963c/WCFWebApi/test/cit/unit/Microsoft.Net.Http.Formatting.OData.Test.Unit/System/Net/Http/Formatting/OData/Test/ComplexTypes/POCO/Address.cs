// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData.Test.ComplexTypes
{
    public class Address
    {
        public string StreetAddress;

        public string City;

        public string State;

        public Address()
        {
        }

        public Address(int index, ReferenceDepthContext context)
        {
            Address sourceAddress = DataSource.Address[index];
            this.StreetAddress = sourceAddress.StreetAddress;
            this.City = sourceAddress.City;
            this.State = sourceAddress.State;
            this.ZipCode = sourceAddress.ZipCode;
        }

        public int ZipCode { get; set; }

        
    }
}
