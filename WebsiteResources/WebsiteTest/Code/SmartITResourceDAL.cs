using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace WebsiteTest.Code
{
    /// <summary>
    /// Summary description for SmartITResourceDALC
    /// DALC - Data Access Layer
    /// </summary>
    public class SmartITResourceDAL : IDisposable
    {
        private FakeDB db;

        public SmartITResourceDAL()
        {
            db = new FakeDB();
        }

        public Dictionary<string, object> GetResourcesByCulture(string culture, string className)
        {
            var resources = db.ResourceGlobalizations
                .Where(a => a.CultureCode == culture && a.ResourceType == className)
                .ToDictionary(a => a.ResourceKey, a => (object)a.ResourceValue);
            return resources;
        }

        public void Dispose()
        {
            
        }
    }
}