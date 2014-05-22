// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData.Test.ComplexTypes
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    
    [DataContract]
	public class Person
	{
        string[] alias;
        Address homeAddress;        

        [DataMember]
        public int Age;

        [DataMember]
        public Gender Gender;        

        [DataMember(IsRequired = true)]
        public string FirstName;        

        [DataMember(Name = "OtherNames")]
        public string[] Alias
        {
            get
            {
                return this.alias;
            }
            set
            {
                this.alias = value;
            }
        }
        
        [DataMember(Name = "HomeAddress")]
        public Address Address
        {
            get
            {
                return this.homeAddress;
            }
            set
            {
                this.homeAddress = value;
            }
        }

        [DataMember(EmitDefaultValue = true)]
        public PhoneNumber HomeNumber;                

        public string UnserializableSSN;

        [DataMember]
        public IActivity FavoriteHobby;

        public Person(int index , ReferenceDepthContext context)
        {
            this.Age = index + 20;
            this.Address = new Address(index, context);
            this.Alias = new string[] { "Alias" + index };
            this.FirstName = DataSource.Names[index];
            this.Gender = Gender.Male;
            this.HomeNumber = DataSource.HomePhoneNumbers[index];
            this.UnserializableSSN = DataSource.SSN[index];
            this.FavoriteHobby = new HobbyActivity("Xbox Gaming");  
        }

        [DataContract(Namespace="", Name="HobbyActivity")]
        public class HobbyActivity : IActivity
        {
            public HobbyActivity(string hobbyName)
            {
                this.ActivityName = hobbyName;
            }

            [DataMember]
            public string ActivityName
            {
                get;
                set;
            }

            public void DoActivity()
            {
                // Some Action
            }
        }
	}
}
