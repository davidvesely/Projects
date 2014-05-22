// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData.Test
{
    using System.Net.Http.Formatting.OData.Test.ComplexTypes;

	internal class DataSource
	{
        public static int MaxIndex = 4;

        public static string[] Names = new string[] { "Frank", "Steve", "Tom", "Chandler", "Ross" };

        public static string[] SSN = new string[] { "556-99-7890", "556-98-7898", "556-98-7789", "556-98-7777", "556-98-6666" };

        public static PhoneNumber[] HomePhoneNumbers = new PhoneNumber[]
        { 
            new PhoneNumber() { CountryCode = 1, AreaCode = 425, Number = 9879089, PhoneType = PhoneType.HomePhone },
            new PhoneNumber() { CountryCode = 1, AreaCode = 425, Number = 9879090, PhoneType = PhoneType.HomePhone },
            new PhoneNumber() { CountryCode = 1, AreaCode = 425, Number = 9879091, PhoneType = PhoneType.HomePhone },
            new PhoneNumber() { CountryCode = 1, AreaCode = 425, Number = 9879092, PhoneType = PhoneType.HomePhone },
            new PhoneNumber() { CountryCode = 1, AreaCode = 425, Number = 9879093, PhoneType = PhoneType.HomePhone }
        };

        public static PhoneNumber[] WorkPhoneNumbers = new PhoneNumber[]
        { 
            new PhoneNumber() { CountryCode = 1, AreaCode = 908, Number = 9879089, PhoneType = PhoneType.WorkPhone },
            new PhoneNumber() { CountryCode = 1, AreaCode = 908, Number = 9879090, PhoneType = PhoneType.WorkPhone },
            new PhoneNumber() { CountryCode = 1, AreaCode = 908, Number = 9879091, PhoneType = PhoneType.WorkPhone },
            new PhoneNumber() { CountryCode = 1, AreaCode = 908, Number = 9879092, PhoneType = PhoneType.WorkPhone },
            new PhoneNumber() { CountryCode = 1, AreaCode = 908, Number = 9879093, PhoneType = PhoneType.WorkPhone }
        };

        public static Address[] Address = new Address[]
        {
            new Address() { StreetAddress = "4850 156th Ave NE", City = "Redmond", State = "WA", ZipCode =  98052 },
            new Address() { StreetAddress = "4851 157th Ave NE", City = "Redmond", State = "WA", ZipCode =  98053 },
            new Address() { StreetAddress = "4852 158th Ave NE", City = "Redmond", State = "WA", ZipCode =  98054 },
            new Address() { StreetAddress = "4853 159th Ave NE", City = "Redmond", State = "WA", ZipCode =  98055 },
            new Address() { StreetAddress = "4854 160th Ave NE", City = "Redmond", State = "WA", ZipCode =  98056 },
        };
	}
}
