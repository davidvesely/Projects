// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.TestCommon;
    using Microsoft.TestCommon.WCF.Types;

    /// <summary>
    /// A base class for test data.  A <see cref="TestData"/> instance is associated with a given type, and the <see cref="TestData"/> instance can
    /// provide instances of the given type to use as data in tests.  The same <see cref="TestData"/> instance can also provide instances
    /// of types related to the given type, such as a <see cref="List<>"/> of the type.  See the <see cref="TestDataVariations"/> enum for all the
    /// variations of test data that a <see cref="TestData"/> instance can provide.
    /// </summary>
    public abstract class WcfTestData : TestData
    {
        /// <summary>
        /// Common <see cref="TestData"/> for an <c>enum</c> decorated with a <see cref="DataContractAttribute"/>.
        /// </summary>
        public static readonly ValueTypeTestData<DataContractEnum> DataContractEnumTestData = new ValueTypeTestData<DataContractEnum>(
            DataContractEnum.First, 
            DataContractEnum.Second);

        /// <summary>
        ///  Common <see cref="TestData"/> for the string form of a <see cref="Uri"/>.
        /// </summary>
        public static readonly RefTypeTestData<string> UriTestDataStrings = new RefTypeTestData<string>(() => new List<string>(){ 
            "http://somehost", 
            "http://somehost:8080", 
            "http://somehost/",
            "http://somehost:8080/", 
            "http://somehost/somepath", 
            "http://somehost/somepath/",
            "http://somehost/somepath?somequery=somevalue"});

        /// <summary>
        ///  Common <see cref="TestData"/> for a <see cref="Uri"/>.
        /// </summary>
        public static readonly RefTypeTestData<Uri> UriTestData = new RefTypeTestData<Uri>(() => 
            UriTestDataStrings.Select<string, Uri>((s) => new Uri(s)).ToList());

        /// <summary>
        ///  Common <see cref="TestData"/> for a class type decorated with DataContract attributes.
        /// </summary>
        public static readonly RefTypeTestData<DataContractType> DataContractTypeTestData = new RefTypeTestData<DataContractType>(
            DataContractType.GetTestData, 
            DataContractType.GetDerivedTypeTestData, 
            null);

        /// <summary>
        ///  Common <see cref="TestData"/> for a class type decorated with DataContract attributes that derives from a base DataContract class type.
        /// </summary>
        public static readonly RefTypeTestData<DerivedDataContractType> DerivedDataContractTypeTestData = new RefTypeTestData<DerivedDataContractType>(
            DerivedDataContractType.GetTestData, 
            null, 
            DerivedDataContractType.GetKnownTypeTestData);

        /// <summary>
        ///  Common <see cref="TestData"/> for a class type decorated with DataContract attributes and that has 
        ///  <see cref="DataContactAttribute.IsReference"/> set to <c>true</c>.
        /// </summary>
        public static readonly RefTypeTestData<ReferenceDataContractType> ReferenceDataContractTypeTestData = new RefTypeTestData<ReferenceDataContractType>(
            ReferenceDataContractType.GetTestData);

        /// <summary>
        ///  Common <see cref="TestData"/> for a class type decorated with XmlSerializer attributes.
        /// </summary>
        public static readonly RefTypeTestData<XmlSerializableType> XmlSerializableTypeTestData = new RefTypeTestData<XmlSerializableType>(
            XmlSerializableType.GetTestData, 
            XmlSerializableType.GetDerivedTypeTestData, 
            null);

        /// <summary>
        ///  Common <see cref="TestData"/> for a class type decorated with XmlSerializer attributes that derives from a base XmlSerializerType class.
        /// </summary>
        public static readonly RefTypeTestData<DerivedXmlSerializableType> DerivedXmlSerializableTypeTestData = new RefTypeTestData<DerivedXmlSerializableType>(
            DerivedXmlSerializableType.GetTestData, 
            null, 
            DerivedXmlSerializableType.GetKnownTypeTestData);

        /// <summary>
        ///  Common <see cref="TestData"/> for a POCO class type.
        /// </summary>
        public static readonly RefTypeTestData<WcfPocoType> WcfPocoTypeTestData = new RefTypeTestData<WcfPocoType>(
            WcfPocoType.GetTestData,
            WcfPocoType.GetDerivedTypeTestData,
            null);

        /// <summary>
        ///  Common <see cref="TestData"/> for a POCO class type that includes null values
        ///  for both the base class and derived classes.
        /// </summary>
        public static readonly RefTypeTestData<WcfPocoType> WcfPocoTypeTestDataWithNull = new RefTypeTestData<WcfPocoType>(
            WcfPocoType.GetTestDataWithNull,
            WcfPocoType.GetDerivedTypeTestDataWithNull,
            null);

        /// <summary>
        /// A read-only collection of reference type test data.
        /// </summary>
        public static new readonly ReadOnlyCollection<TestData> RefTypeTestDataCollection = new ReadOnlyCollection<TestData>(new TestData[] { 
            StringTestData, 
            WcfPocoTypeTestData, 
            DataContractTypeTestData, 
            DerivedDataContractTypeTestData, 
            XmlSerializableTypeTestData, 
            DerivedXmlSerializableTypeTestData, 
            ISerializableTypeTestData,  
            ReferenceDataContractTypeTestData});

        /// <summary>
        /// A read-only collection of value and reference type test data.
        /// </summary>
        public static new readonly ReadOnlyCollection<TestData> ValueAndRefTypeTestDataCollection = new ReadOnlyCollection<TestData>(
            ValueTypeTestDataCollection.Concat(RefTypeTestDataCollection).ToList());

        /// <summary>
        /// A read-only collection of representative values and reference type test data.
        /// Uses where exhaustive coverage is not required.
        /// </summary>
        public static new readonly ReadOnlyCollection<TestData> RepresentativeValueAndRefTypeTestDataCollection = new ReadOnlyCollection<TestData>(new TestData[] {
            IntTestData,
            BoolTestData,
            SimpleEnumTestData,
            StringTestData, 
            WcfPocoTypeTestData
        });

        /// <summary>
        /// Initializes a new instance of the <see cref="TestData"/> class.
        /// </summary>
        /// <param name="type">The type associated with the <see cref="TestData"/> instance.</param>
        protected WcfTestData(Type type) : base(type)
        {
        }
    }


    /// <summary>
    /// A generic base class for test data. 
    /// </summary>
    /// <typeparam name="T">The type associated with the test data.</typeparam>
    public abstract class WcfTestData<T> : WcfTestData, IEnumerable<T>
    {
        private static readonly Type OpenIEnumerableType = typeof(IEnumerable<>);
        private static readonly Type OpenListType = typeof(List<>);
        private static readonly Type OpenIQueryableType = typeof(IQueryable<>);
        private static readonly Type OpenGenericDataContractType = typeof(GenericDataContractType<>);
        private static readonly Type OpenGenericXmlSerializableType = typeof(GenericXmlSerializableType<>);

        /// <summary>
        /// Initializes a new instance of the <see cref="TestData&lt;T&gt;"/> class.
        /// </summary>
        protected WcfTestData() 
            : base(typeof(T))
        {
            Type[] typeParams = new Type[] { this.Type };
            
            Type arrayType = this.Type.MakeArrayType();
            Type listType = OpenListType.MakeGenericType(typeParams);
            Type iEnumerableType = OpenIEnumerableType.MakeGenericType(typeParams);
            Type iQueryableType = OpenIQueryableType.MakeGenericType(typeParams);
     
            Type[] typeArrayParams = new Type[] { arrayType };
            Type[] typeListParams = new Type[] { listType };
            Type[] typeIEnumerableParams = new Type[] { iEnumerableType };
            Type[] typeIQueryableParams = new Type[] { iQueryableType };
            
            Type dataContractPropertyType = OpenGenericDataContractType.MakeGenericType(typeParams);
            Type dataContractArrayPropertyType = OpenGenericDataContractType.MakeGenericType(typeArrayParams);
            Type dataContractListPropertyType = OpenGenericDataContractType.MakeGenericType(typeListParams);
            Type dataContractIEnumerablePropertyType = OpenGenericDataContractType.MakeGenericType(typeIEnumerableParams);
            Type dataContractIQueryablePropertyType = OpenGenericDataContractType.MakeGenericType(typeIQueryableParams);

            this.RegisterTestDataVariation(TestDataVariations.AsDataMember, dataContractPropertyType, GetAsInstancePropertyOfDataContractType);
            this.RegisterTestDataVariation(TestDataVariations.AsDataMember | TestDataVariations.AsArray, dataContractArrayPropertyType, GetAsArrayPropertyOfDataContractType);
            this.RegisterTestDataVariation(TestDataVariations.AsDataMember | TestDataVariations.AsList, dataContractListPropertyType, GetAsListPropertyOfDataContractType);
            this.RegisterTestDataVariation(TestDataVariations.AsDataMember | TestDataVariations.AsIEnumerable, dataContractIEnumerablePropertyType, GetAsIEnumerablePropertyOfDataContractType);
            this.RegisterTestDataVariation(TestDataVariations.AsDataMember | TestDataVariations.AsIQueryable, dataContractIQueryablePropertyType, GetAsIQueryablePropertyOfDataContractType);

            Type xmlSerializablePropertyType = OpenGenericXmlSerializableType.MakeGenericType(typeParams);
            Type xmlSerializableArrayPropertyType = OpenGenericXmlSerializableType.MakeGenericType(typeArrayParams);
            Type xmlSerializableListPropertyType = OpenGenericXmlSerializableType.MakeGenericType(typeListParams);
            Type xmlSerializableIEnumerablePropertyType = OpenGenericXmlSerializableType.MakeGenericType(typeIEnumerableParams);
            Type xmlSerializableIQueryablePropertyType = OpenGenericXmlSerializableType.MakeGenericType(typeIQueryableParams);

            this.RegisterTestDataVariation(TestDataVariations.AsXmlElementProperty, xmlSerializablePropertyType, GetAsInstancePropertyOfXmlSerializableType);
            this.RegisterTestDataVariation(TestDataVariations.AsXmlElementProperty | TestDataVariations.AsArray, xmlSerializableArrayPropertyType, GetAsArrayPropertyOfXmlSerializableType);
            this.RegisterTestDataVariation(TestDataVariations.AsXmlElementProperty | TestDataVariations.AsList, xmlSerializableListPropertyType, GetAsListPropertyOfXmlSerializableType);
            this.RegisterTestDataVariation(TestDataVariations.AsXmlElementProperty | TestDataVariations.AsIEnumerable, xmlSerializableIEnumerablePropertyType, GetAsIEnumerablePropertyOfXmlSerializableType);
            this.RegisterTestDataVariation(TestDataVariations.AsXmlElementProperty | TestDataVariations.AsIQueryable, xmlSerializableIQueryablePropertyType, GetAsIQueryablePropertyOfXmlSerializableType);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)this.GetTypedTestData().ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetTypedTestData().ToList().GetEnumerator();
        }

        /// <summary>
        /// Gets the test data as an array.
        /// </summary>
        /// <returns>An array of test data of the given type.</returns>
        public T[] GetTestDataAsArray()
        {
            return this.GetTypedTestData().ToArray();
        }

        /// <summary>
        /// Gets the test data as a <see cref="List<>"/>.
        /// </summary>
        /// <returns>A <see cref="List<>"/> of test data of the given type.</returns>
        public List<T> GetTestDataAsList()
        {
            return this.GetTypedTestData().ToList();
        }

        /// <summary>
        /// Gets the test data as an <see cref="IEnumerable<>"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable<>"/> of test data of the given type.</returns>
        public IEnumerable<T> GetTestDataAsIEnumerable()
        {
            return this.GetTypedTestData().AsEnumerable();
        }

        /// <summary>
        /// Gets the test data as an <see cref="IQueryable<>"/>.
        /// </summary>
        /// <returns>An <see cref="IQueryable<>"/> of test data of the given type.</returns>
        public IQueryable<T> GetTestDataAsIQueryable()
        {
            return this.GetTypedTestData().AsQueryable();
        }

        /// <summary>
        /// Gets a collection of DataContract type instances with a DataMember of the given type.
        /// </summary>
        /// <returns>A collection of DataContract type instances with a DataMember of the given type.</returns>
        public IEnumerable<GenericDataContractType<T>> GetAsInstancePropertyOfDataContractType()
        {
            return this.GetTypedTestData().Select(t => new GenericDataContractType<T>(t));
        }

        /// <summary>
        /// Gets a DataContract instance with a property with an array of the given type.
        /// </summary>
        /// <returns>A DataContract instance with a property with an array of the given type.</returns>
        public GenericDataContractType<T[]> GetAsArrayPropertyOfDataContractType()
        {
            return new GenericDataContractType<T[]>(this.GetTestDataAsArray());
        }

        /// <summary>
        /// Gets a DataContract instance with a property with a <see cref="List<>"/> of the given type.
        /// </summary>
        /// <returns>A DataContract instance with a property with a <see cref="List<>"/> of the given type.</returns>
        public GenericDataContractType<List<T>> GetAsListPropertyOfDataContractType()
        {
            return new GenericDataContractType<List<T>>(this.GetTestDataAsList());
        }

        /// <summary>
        /// Gets a DataContract instance with a property with an <see cref="IEnumerable<>"/> of the given type.
        /// </summary>
        /// <returns>A DataContract instance with a property with an <see cref="IEnumerable<>"/> of the given type.</returns>
        public GenericDataContractType<IEnumerable<T>> GetAsIEnumerablePropertyOfDataContractType()
        {
            return new GenericDataContractType<IEnumerable<T>>(this.GetTestDataAsIEnumerable());
        }

        /// <summary>
        /// Gets a DataContract instance with a property with an <see cref="IQueryable<>"/> of the given type.
        /// </summary>
        /// <returns>A DataContract instance with a property with an <see cref="IQueryable<>"/> of the given type.</returns>
        public GenericDataContractType<IQueryable<T>> GetAsIQueryablePropertyOfDataContractType()
        {
            return new GenericDataContractType<IQueryable<T>>(this.GetTestDataAsIQueryable());
        }

        /// <summary>
        /// Gets a collection of XmlSerializable type instances with an <see cref="XmlElementAttribute"/> property of the given type.
        /// </summary>
        /// <returns>A collection of XmlSerializable type instances with an <see cref="XmlElementAttribute"/> property of the given type.</returns>
        public IEnumerable<GenericXmlSerializableType<T>> GetAsInstancePropertyOfXmlSerializableType()
        {
            return this.GetTypedTestData().Select(t => new GenericXmlSerializableType<T>(t));
        }

        /// <summary>
        /// Gets an XmlSerializable instance with a property with an array of the given type.
        /// </summary>
        /// <returns>An XmlSerializable instance with a property with an array of the given type.</returns>
        public GenericXmlSerializableType<T[]> GetAsArrayPropertyOfXmlSerializableType()
        {
            return new GenericXmlSerializableType<T[]>(this.GetTestDataAsArray());
        }

        /// <summary>
        /// Gets an XmlSerializable instance with a property with an <see cref="List<>"/> of the given type.
        /// </summary>
        /// <returns>An XmlSerializable instance with a property with an <see cref="List<>"/> of the given type.</returns>
        public GenericXmlSerializableType<List<T>> GetAsListPropertyOfXmlSerializableType()
        {
            return new GenericXmlSerializableType<List<T>>(this.GetTestDataAsList());
        }

        /// <summary>
        /// Gets an XmlSerializable instance with a property with an <see cref="IEnumerable<>"/> of the given type.
        /// </summary>
        /// <returns>An XmlSerializable instance with a property with an <see cref="IEnumerable<>"/> of the given type.</returns>
        public GenericXmlSerializableType<IEnumerable<T>> GetAsIEnumerablePropertyOfXmlSerializableType()
        {
            return new GenericXmlSerializableType<IEnumerable<T>>(this.GetTestDataAsIEnumerable());
        }

        /// <summary>
        /// Gets an XmlSerializable instance with a property with an <see cref="IQueryable<>"/> of the given type.
        /// </summary>
        /// <returns>An XmlSerializable instance with a property with an <see cref="IQueryable<>"/> of the given type.</returns>
        public GenericXmlSerializableType<IQueryable<T>> GetAsIQueryablePropertyOfXmlSerializableType()
        {
            return new GenericXmlSerializableType<IQueryable<T>>(this.GetTestDataAsIQueryable());
        }

        /// <summary>
        /// Must be implemented by derived types to return test data of the given type.
        /// </summary>
        /// <returns>Test data of the given type.</returns>
        protected abstract IEnumerable<T> GetTypedTestData();
    }
}
