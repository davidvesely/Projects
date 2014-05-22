// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.TestCommon;
    using System.Net.Http.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, UnitTestLevel(UnitTestLevel.Complete)]
    public class MediaTypeFormatterCollectionTests : UnitTest<MediaTypeFormatterCollection>
    {
        #region Type

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeFormatterCollection is public, concrete, and unsealed.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsPublicVisibleClass, typeof(Collection<MediaTypeFormatter>));
        }

        #endregion Type

        #region Constructors

        #region MediaTypeFormatterCollection()

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeFormatterCollection() initializes default formatters.")]
        public void Constructor()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection();
            Assert.AreEqual(4, collection.Count, "Expected default ctor to initialize with this many default formatters.");
            Assert.IsNotNull(collection.XmlFormatter, "XmlFormatter was not set.");
            Assert.IsNotNull(collection.JsonValueFormatter, "JsonValueFormatter was not set.");
            Assert.IsNotNull(collection.JsonFormatter, "JsonFormatter was not set.");
            Assert.IsNotNull(collection.FormUrlEncodedFormatter, "FormUrlEncodedFormatter was not set.");
        }

        #endregion MediaTypeFormatterCollection()

        #region MediaTypeFormatterCollection(IEnumerable<MediaTypeFormatter>)

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeFormatterCollection(IEnumerable<MediaTypeFormatter>) accepts empty collection and does not add to it.")]
        public void Constructor1AcceptsEmptyList()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(new MediaTypeFormatter[0]);
            Assert.AreEqual(0, collection.Count, "Collection should not contain any formatters.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeFormatterCollection(IEnumerable<MediaTypeFormatter>) sets XmlFormatter, JsonValueFormatter and JsonFormatter for all known collections of formatters that contain them.")]
        public void Constructor1SetsProperties()
        {
            // All combination of formatters presented to ctor should still set XmlFormatter
            foreach (IEnumerable<MediaTypeFormatter> formatterCollection in DataSets.Http.AllFormatterCollections)
            {
                MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(formatterCollection);
                if (collection.OfType<XmlMediaTypeFormatter>().Any())
                {
                    Assert.IsNotNull(collection.XmlFormatter, "XmlFormatter was not set.");
                }
                else
                {
                    Assert.IsNull(collection.XmlFormatter, "XmlFormatter should not be set.");
                }

                if (collection.OfType<JsonValueMediaTypeFormatter>().Any())
                {
                    Assert.IsNotNull(collection.JsonValueFormatter, "JsonValueFormatter was not set.");
                }
                else
                {
                    Assert.IsNull(collection.JsonValueFormatter, "JsonValueFormatter should not be set.");
                }

                if (collection.OfType<JsonMediaTypeFormatter>().Any())
                {
                    Assert.IsNotNull(collection.JsonFormatter, "JsonFormatter was not set.");
                }
                else
                {
                    Assert.IsNull(collection.JsonFormatter, "JsonFormatter should not be set.");
                }
            }
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeFormatterCollection(IEnumerable<MediaTypeFormatter>) sets derived classes of Xml and Json formatters.")]
        public void Constructor1SetsDerivedFormatters()
        {
            // force to array to get stable instances
            MediaTypeFormatter[] derivedFormatters = DataSets.Http.DerivedFormatters.ToArray();
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(derivedFormatters);
            CollectionAssert.AreEqual(derivedFormatters, collection, "Derived formatters should have been in collection.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeFormatterCollection(IEnumerable<MediaTypeFormatter>) throws with null formatters collection.")]
        public void Constructor1ThrowsWithNullFormatters()
        {
            Asserters.Exception.ThrowsArgumentNull("formatters", () => new MediaTypeFormatterCollection(null));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeFormatterCollection(IEnumerable<MediaTypeFormatter>) throws with null formatter in formatters collection.")]
        public void Constructor1ThrowsWithNullFormatterInCollection()
        {
            Asserters.Exception.ThrowsArgument(
                "formatters",
                SR.CannotHaveNullInList(typeof(MediaTypeFormatter).Name),
                () => new MediaTypeFormatterCollection(new MediaTypeFormatter[] { null }));
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("MediaTypeFormatterCollection(IEnumerable<MediaTypeFormatter>) accepts multiple instances of same formatter type.")]
        public void Constructor1AcceptsDuplicateFormatterTypes()
        {
            MediaTypeFormatter[] formatters = new MediaTypeFormatter[]
            {
                new XmlMediaTypeFormatter(),
                new JsonValueMediaTypeFormatter(),
                new JsonMediaTypeFormatter(),
                new FormUrlEncodedMediaTypeFormatter(),
                new XmlMediaTypeFormatter(),
                new JsonValueMediaTypeFormatter(),
                new JsonMediaTypeFormatter(),
                new FormUrlEncodedMediaTypeFormatter(),
            };

            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(formatters);
            CollectionAssert.AreEqual(formatters, collection, "Collections should have been identical");
        }

        #endregion MediaTypeFormatterCollection(IEnumerable<MediaTypeFormatter>)

        #endregion Constructors

        #region Properties

        #region XmlFormatter

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("XmlFormatter is set by ctor.")]
        public void XmlFormatterSetByCtor()
        {
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(new MediaTypeFormatter[] { formatter });
            Assert.AreSame(formatter, collection.XmlFormatter, "XmlFormatter was not set by the ctor.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("XmlFormatter is cleared by ctor with empty collection.")]
        public void XmlFormatterClearedByCtor()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(new MediaTypeFormatter[0]);
            Assert.IsNull(collection.XmlFormatter, "XmlFormatter should not be set.");
        }

        #endregion XmlFormatter

        #region JsonValueFormatter

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonValueFormatter is set by ctor.")]
        public void JsonValueFormatterSetByCtor()
        {
            JsonValueMediaTypeFormatter formatter = new JsonValueMediaTypeFormatter();
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(new MediaTypeFormatter[] { formatter });
            Assert.AreSame(formatter, collection.JsonValueFormatter, "JsonValueFormatter was not set by the ctor.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonValueFormatter is cleared by ctor with empty collection.")]
        public void JsonValueFormatterClearedByCtor()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(new MediaTypeFormatter[0]);
            Assert.IsNull(collection.JsonValueFormatter, "JsonValueFormatter should not be set.");
        }

        #endregion JsonValueFormatter

        #region JsonFormatter

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonFormatter is set by ctor.")]
        public void JsonFormatterSetByCtor()
        {
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(new MediaTypeFormatter[] { formatter });
            Assert.AreSame(formatter, collection.JsonFormatter, "JsonFormatter was not set by the ctor.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("JsonFormatter is cleared by ctor with empty collection.")]
        public void JsonFormatterClearedByCtor()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(new MediaTypeFormatter[0]);
            Assert.IsNull(collection.JsonFormatter, "JsonFormatter should not be set.");
        }

        #endregion JsonFormatter


        #region FormUrlEncodedFormatter

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FormUrlEncodedFormatter is set by ctor.")]
        public void FormUrlEncodedFormatterSetByCtor()
        {
            FormUrlEncodedMediaTypeFormatter formatter = new FormUrlEncodedMediaTypeFormatter();
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(new MediaTypeFormatter[] { formatter });
            Assert.AreSame(formatter, collection.FormUrlEncodedFormatter, "FormUrlEncodedFormatter was not set by the ctor.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("FormUrlEncodedFormatter is cleared by ctor with empty collection.")]
        public void FormUrlEncodedFormatterClearedByCtor()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection(new MediaTypeFormatter[0]);
            Assert.IsNull(collection.FormUrlEncodedFormatter, "FormUrlEncodedFormatter should not be set.");
        }

        #endregion FormUrlEncodedFormatter

        #endregion Properties

        #region Methods

        #endregion Methods

        #region Base Methods

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Remove(MediaTypeFormatter) sets XmlFormatter to null.")]
        public void RemoveSetsXmlFormatter()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection();
            int count = collection.Count;
            collection.Remove(collection.XmlFormatter);
            Assert.IsNull(collection.XmlFormatter, "Formatter was not cleared.");
            Assert.AreEqual(count - 1, collection.Count, "Collection count was incorrect.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Remove(MediaTypeFormatter) sets JsonValueFormatter to null.")]
        public void RemoveSetsJsonValueFormatter()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection();
            int count = collection.Count;
            collection.Remove(collection.JsonValueFormatter);
            Assert.IsNull(collection.JsonValueFormatter, "Formatter was not cleared.");
            Assert.AreEqual(count - 1, collection.Count, "Collection count was incorrect.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Remove(MediaTypeFormatter) sets JsonFormatter to null.")]
        public void RemoveSetsJsonFormatter()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection();
            int count = collection.Count;
            collection.Remove(collection.JsonFormatter);
            Assert.IsNull(collection.JsonFormatter, "Formatter was not cleared.");
            Assert.AreEqual(count - 1, collection.Count, "Collection count was incorrect.");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        [Description("Insert(int, MediaTypeFormatter) sets XmlFormatter.")]
        public void InsertSetsXmlFormatter()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection();
            int count = collection.Count;
            XmlMediaTypeFormatter formatter = new XmlMediaTypeFormatter();
            collection.Insert(0, formatter);
            Assert.AreSame(formatter, collection.XmlFormatter, "Formatter was set.");
            Assert.AreEqual(count + 1, collection.Count, "Collection count was incorrect.");
        }

        [TestMethod]
        [Description("Insert(int, MediaTypeFormatter) sets JsonValueFormatter.")]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void InsertSetsJsonValueFormatter()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection();
            int count = collection.Count;
            JsonValueMediaTypeFormatter formatter = new JsonValueMediaTypeFormatter();
            collection.Insert(0, formatter);
            Assert.AreSame(formatter, collection.JsonValueFormatter, "Formatter was set.");
            Assert.AreEqual(count + 1, collection.Count, "Collection count was incorrect.");
        }

        [TestMethod]
        [Description("Insert(int, MediaTypeFormatter) sets JsonFormatter.")]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("dravva")]
        public void InsertSetsJsonFormatter()
        {
            MediaTypeFormatterCollection collection = new MediaTypeFormatterCollection();
            int count = collection.Count;
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            collection.Insert(0, formatter);
            Assert.AreSame(formatter, collection.JsonFormatter, "Formatter was set.");
            Assert.AreEqual(count + 1, collection.Count, "Collection count was incorrect.");
        }

        #endregion Base Methods
    }
}
