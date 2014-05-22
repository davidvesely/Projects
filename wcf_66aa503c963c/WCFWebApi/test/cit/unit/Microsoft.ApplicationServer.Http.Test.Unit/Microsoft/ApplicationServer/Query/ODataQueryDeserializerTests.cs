// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

using System;
using System.Linq;
using Microsoft.TestCommon;
using Microsoft.TestCommon.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.ApplicationServer.Query
{
    [TestClass]
    [UnitTestType(typeof(ODataQueryDeserializer))]
    [UnitTestLevel(TestCommon.UnitTestLevel.InProgress)]
    public class ODataQueryDeserializerTests : Microsoft.ApplicationServer.Http.UnitTest
    {
        private IQueryable<Product> baseQuery = new Product[0].AsQueryable();

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles a simple multipart query deserializes correctly")]
        public void DeserializeSimpleMultipartQuery()
        {
            VerifyQueryDeserialization(
                "$filter=ProductName eq 'Doritos'&$orderby=UnitPrice&$top=100",
                "Where(Param_0 => (Param_0.ProductName == \"Doritos\")).OrderBy(Param_1 => Param_1.UnitPrice).Take(100)");
        }

        #region Ordering
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles default orderby expression")]
        public void DeserializeOrderBy()
        {
            VerifyQueryDeserialization(
                "$orderby=UnitPrice", 
                "OrderBy(Param_0 => Param_0.UnitPrice)");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles orderby ascending")]
        public void DeserializeOrderByAscending()
        {
            VerifyQueryDeserialization(
                "$orderby=UnitPrice asc",
                "OrderBy(Param_0 => Param_0.UnitPrice)");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles orderby descending")]
        public void DeserializeOrderByDescending()
        {
            VerifyQueryDeserialization(
                "$orderby=UnitPrice desc", 
                "OrderByDescending(Param_0 => Param_0.UnitPrice)");
        }
        #endregion

        #region Inequalities
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'eq' operator")]
        public void DeserializeEqualityOperator()
        {
            VerifyQueryDeserialization(
                "$filter=ProductName eq 'Doritos'", 
                "Where(Param_0 => (Param_0.ProductName == \"Doritos\"))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'ne' operator")]
        public void DeserializeNotEqualOperator()
        {
            VerifyQueryDeserialization(
                "$filter=ProductName ne 'Doritos'",
                "Where(Param_0 => (Param_0.ProductName != \"Doritos\"))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'gt' operator")]
        public void DeserializeGreaterThanOperator()
        {
            VerifyQueryDeserialization(
                "$filter=UnitPrice gt 5.00",
                "Where(Param_0 => (Param_0.UnitPrice > 5.00))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'ge' operator")]
        public void DeserializeGreaterThanEqualOperator()
        {
            VerifyQueryDeserialization(
                "$filter=UnitPrice ge 5.00",
                "Where(Param_0 => (Param_0.UnitPrice >= 5.00))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'lt' operator")]
        public void DeserializeLessThanOperator()
        {
            VerifyQueryDeserialization(
                "$filter=UnitPrice lt 5.00",
                "Where(Param_0 => (Param_0.UnitPrice < 5.00))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'le' operator")]
        public void DeserializeLessThanOrEqualOperator()
        {
            VerifyQueryDeserialization(
                "$filter=UnitPrice le 5.00",
                "Where(Param_0 => (Param_0.UnitPrice <= 5.00))");
        }
        #endregion

        #region Logical Operators
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'or' operator")]
        public void DeserializeOrOperator()
        {
            VerifyQueryDeserialization(
                "$filter=UnitPrice eq 5.00 or UnitPrice eq 10.00",
                "Where(Param_0 => ((Param_0.UnitPrice == 5.00) OrElse (Param_0.UnitPrice == 10.00)))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'and' operator")]
        public void DeserializeAndOperator()
        {
            VerifyQueryDeserialization(
                "$filter=UnitPrice eq 5.00 and UnitPrice eq 10.00",
                "Where(Param_0 => ((Param_0.UnitPrice == 5.00) AndAlso (Param_0.UnitPrice == 10.00)))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'eq' operator")]
        public void DeserializeNegation()
        {
            VerifyQueryDeserialization(
                "$filter=not (UnitPrice eq 5.00)",
                "Where(Param_0 => Not((Param_0.UnitPrice == 5.00)))");
        }
        #endregion

        #region Arithmetic Operators
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'sub' operator")]
        public void DeserializeSubtraction()
        {
            VerifyQueryDeserialization(
                "$filter=UnitPrice sub 1.00 lt 5.00",
                "Where(Param_0 => ((Param_0.UnitPrice - 1.00) < 5.00))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'add' operator")]
        public void DeserializeAddition()
        {
            VerifyQueryDeserialization(
                "$filter=UnitPrice add 1.00 lt 5.00",
                "Where(Param_0 => ((Param_0.UnitPrice + 1.00) < 5.00))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'mul' operator")]
        public void DeserializeMultiplication()
        {
            VerifyQueryDeserialization(
                "$filter=UnitPrice mul 1.00 lt 5.00",
                "Where(Param_0 => ((Param_0.UnitPrice * 1.00) < 5.00))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'div' operator")]
        public void DeserializeDivision()
        {
            VerifyQueryDeserialization(
                "$filter=UnitPrice div 1.00 lt 5.00",
                "Where(Param_0 => ((Param_0.UnitPrice / 1.00) < 5.00))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'mod' operator")]
        public void DeserializeModulo()
        {
            VerifyQueryDeserialization(
                "$filter=UnitPrice mod 1.00 lt 5.00",
                "Where(Param_0 => ((Param_0.UnitPrice % 1.00) < 5.00))");
        }
        #endregion

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles expression grouping")]
        public void DeserializeGrouping()
        {
            VerifyQueryDeserialization(
                "$filter=((ProductName ne 'Doritos') or (UnitPrice lt 5.00))",
                "Where(Param_0 => ((Param_0.ProductName != \"Doritos\") OrElse (Param_0.UnitPrice < 5.00)))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles multilevel member expressions")]
        public void DeserializeMemberExpressions()
        {
            VerifyQueryDeserialization(
                "$filter=Category/CategoryName eq 'Snacks'",
                "Where(Param_0 => (Param_0.Category.CategoryName == \"Snacks\"))");
        }

        #region String Functions
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'substring' function")]
        public void DeserializeStringContains()
        {
            VerifyQueryDeserialization(
                "$filter=substringof(ProductName, 'Abc') eq true",
                "Where(Param_0 => (Param_0.ProductName.Contains(\"Abc\") == True))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'startswith' function")]
        public void DeserializeStringStartsWith()
        {
            VerifyQueryDeserialization(
                "$filter=startswith(ProductName, 'Abc') eq true",
                "Where(Param_0 => (Param_0.ProductName.StartsWith(\"Abc\") == True))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'endswith' function")]
        public void DeserializeStringEndsWith()
        {
            VerifyQueryDeserialization(
                "$filter=endswith(ProductName, 'Abc') eq true",
                "Where(Param_0 => (Param_0.ProductName.EndsWith(\"Abc\") == True))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'length' function")]
        public void DeserializeStringLength()
        {
            VerifyQueryDeserialization(
                "$filter=length(ProductName) gt 0",
                "Where(Param_0 => (Param_0.ProductName.Length > 0))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'indexof' function")]
        public void DeserializeStringIndexOf()
        {
            VerifyQueryDeserialization(
                "$filter=indexof(ProductName, 'Abc') eq 5",
                "Where(Param_0 => (Param_0.ProductName.IndexOf(\"Abc\") == 5))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'replace' function")]
        public void DeserializeStringReplace()
        {
            VerifyQueryDeserialization(
                "$filter=replace(ProductName, 'Abc', 'Def') eq \"FooDef\"",
                "Where(Param_0 => (Param_0.ProductName.Replace(\"Abc\", \"Def\") == \"FooDef\"))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'substring' function")]
        public void DeserializeStringSubstring()
        {
            VerifyQueryDeserialization(
                "$filter=substring(ProductName, 3) eq \"uctName\"",
                "Where(Param_0 => (Param_0.ProductName.Substring(3) == \"uctName\"))");

            VerifyQueryDeserialization(
                "$filter=substring(ProductName, 3, 4) eq \"uctN\"",
                "Where(Param_0 => (Param_0.ProductName.Substring(3, 4) == \"uctN\"))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'tolower' function")]
        public void DeserializeStringToLower()
        {
            VerifyQueryDeserialization(
                "$filter=tolower(ProductName) eq 'tasty treats'",
                "Where(Param_0 => (Param_0.ProductName.ToLower() == \"tasty treats\"))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'toupper' function")]
        public void DeserializeStringToUpper()
        {
            VerifyQueryDeserialization(
                "$filter=toupper(ProductName) eq 'TASTY TREATS'",
                "Where(Param_0 => (Param_0.ProductName.ToUpper() == \"TASTY TREATS\"))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'trim' function")]
        public void DeserializeStringTrim()
        {
            VerifyQueryDeserialization(
                "$filter=trim(ProductName) eq 'Tasty Treats'",
                "Where(Param_0 => (Param_0.ProductName.Trim() == \"Tasty Treats\"))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'concat' function")]
        public void DeserializeStringConcat()
        {
            VerifyQueryDeserialization(
                "$filter=concat('Foo', 'Bar') eq 'FooBar'",
                "Where(Param_0 => (Concat(\"Foo\", \"Bar\") == \"FooBar\"))");
        }
        #endregion

        #region Date Functions
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'day' function")]
        public void DeserializeDateDay()
        {
            VerifyQueryDeserialization(
                "$filter=day(DiscontinuedDate) eq 8",
                "Where(Param_0 => (Param_0.DiscontinuedDate.Day == 8))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'month' function")]
        public void DeserializeDateMonth()
        {
            VerifyQueryDeserialization(
                "$filter=month(DiscontinuedDate) eq 8",
                "Where(Param_0 => (Param_0.DiscontinuedDate.Month == 8))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'year' function")]
        public void DeserializeDateYear()
        {
            VerifyQueryDeserialization(
                "$filter=year(DiscontinuedDate) eq 1974",
                "Where(Param_0 => (Param_0.DiscontinuedDate.Year == 1974))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'hour' function")]
        public void DeserializeDateHour()
        {
            VerifyQueryDeserialization("$filter=hour(DiscontinuedDate) eq 8",
                "Where(Param_0 => (Param_0.DiscontinuedDate.Hour == 8))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'minute' function")]
        public void DeserializeDateMinute()
        {
            VerifyQueryDeserialization(
                "$filter=minute(DiscontinuedDate) eq 12",
                "Where(Param_0 => (Param_0.DiscontinuedDate.Minute == 12))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'second' function")]
        public void DeserializeDateSecond()
        {
            VerifyQueryDeserialization(
                "$filter=second(DiscontinuedDate) eq 33",
                "Where(Param_0 => (Param_0.DiscontinuedDate.Second == 33))");
        }
        #endregion

        #region Math Functions
        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'round' function")]
        public void DeserializeMathRound()
        {
            VerifyQueryDeserialization(
                "$filter=round(UnitPrice) gt 5.00",
                "Where(Param_0 => (Round(Param_0.UnitPrice) > 5.00))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'floor' function")]
        public void DeserializeMathFloor()
        {
            VerifyQueryDeserialization(
                "$filter=floor(UnitPrice) eq 5",
                "Where(Param_0 => (Floor(Param_0.UnitPrice) == 5))");
        }

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("Deserialize(IQueryable,Uri) correctly handles the 'ceiling' function")]
        public void DeserializeMathCeiling()
        {
            VerifyQueryDeserialization(
                "$filter=ceiling(UnitPrice) eq 5",
                "Where(Param_0 => (Ceiling(Param_0.UnitPrice) == 5))");
        }
        #endregion

        [TestMethod]
        [TestCategory("CIT"), Timeout(TimeoutConstant.DefaultTimeout), Owner("mathewc")]
        [Description("ODataQueryDeserializer is internal.")]
        public void TypeIsCorrect()
        {
            Asserters.Type.HasProperties(this.TypeUnderTest, TypeAssert.TypeProperties.IsStatic | TypeAssert.TypeProperties.IsClass);
        }

        /// <summary>
        /// Call the query deserializer and verify the results
        /// </summary>
        /// <param name="queryString">The URL query string to deserialize (e.g. $filter=ProductName eq 'Doritos')</param>
        /// <param name="expectedResult">The Expression.ToString() representation of the expected result (e.g. Where(Param_0 => (Param_0.ProductName == \"Doritos\"))</param>
        private void VerifyQueryDeserialization(string queryString, string expectedResult)
        {
            string uri = "http://myhost/odata.svc/GetProducts?" + queryString;

            IQueryable<Product> resultQuery = (IQueryable<Product>)ODataQueryDeserializer.Deserialize(baseQuery, new Uri(uri));
            VerifyExpression(resultQuery, expectedResult);
        }

        private void VerifyExpression(IQueryable query, string expectedExpression)
        {
            // strip off the beginning part of the expression to get to the first
            // actual query operator
            string resultExpression = query.Expression.ToString();
            int startIdx = (query.ElementType.FullName + "[]").Length + 1;
            resultExpression = resultExpression.Substring(startIdx);

            Assert.AreEqual(resultExpression, expectedExpression,
                string.Format("Expected expression '{0}' but the deserializer produced '{1}'", expectedExpression, resultExpression));
        }
    }
}
