// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Common.Test.Mocks
{
    using System;
    using Microsoft.ApplicationServer.Http.Description;

    internal class MockHttpParameterValueConverter : HttpParameterValueConverter
    {
        public MockHttpParameterValueConverter(Type type) : base(type)
        {
        }

        public override object Convert(object value)
        {
            throw new NotImplementedException();
        }

        public override bool IsInstanceOf(object value)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvertFromType(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
