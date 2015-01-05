using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web.Compilation;

namespace WebsiteTest.Code
{
    public enum ResxFactoryType
    {
        Global,
        Local,
    }

    /// <summary>
    /// Summary description for SmartITResourceProvider
    /// </summary>
    public class SmartITResourceProvider : IResourceProvider
    {
        private string className;

        private string originalClassName { get; set; }

        public ResxFactoryType FactoryType { get; set; }

        private SmartITResourceReader _ResourceReader;

        private IDictionary _resourceCache;

        public SmartITResourceProvider(string className, string originalClassName, ResxFactoryType type)
        {
            FactoryType = type;
            this.originalClassName = originalClassName;
            this.className = className;
            _resourceCache = new Dictionary<string, Dictionary<string, object>>();
        }

        public object GetObject(string resourceKey, CultureInfo culture)
        {
            string cultureName = null;
            if (culture != null)
                cultureName = culture.Name;
            else
                cultureName = CultureInfo.CurrentUICulture.Name;
            Debug.WriteLine(string.Format("GetObject, culture = \"{0}\", resourceKey = {1}", cultureName, resourceKey));
            return GetObjectInternal(resourceKey, cultureName);
        }

        private object GetObjectInternal(string resourceKey, string cultureName)
        {
            IDictionary resources = this.GetResourceCache(cultureName);

            object value = null;
            if (resources == null)
                value = null;
            else
                value = resources[resourceKey];

            // If we're at a specific culture (en-US) and there's no value, then fall back
            // to the generic culture (en)
            if (value == null && cultureName.Length > 3)
            {
                // Try again with the 2 letter locale
                value = GetObjectInternal(resourceKey, cultureName.Substring(0, 2));
            }

            // If the value is still null get the invariant value
            if (value == null && cultureName.Length > 0)
            {
                value = GetObjectInternal(resourceKey, CultureInfo.InvariantCulture.Name);
            }


            // If the value is still null and we're at the invariant culture
            // fallback to default resource provider
            if (value == null && string.IsNullOrEmpty(cultureName))
            {
                IResourceProvider resxProvider;
                string typeName = "System.Web.Compilation.ResXResourceProviderFactory, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
                ResourceProviderFactory factory = (ResourceProviderFactory)Activator.CreateInstance(Type.GetType(typeName));
                if (FactoryType == ResxFactoryType.Global)
                    resxProvider = factory.CreateGlobalResourceProvider(originalClassName);
                else
                    resxProvider = factory.CreateLocalResourceProvider(originalClassName);

                value = resxProvider.GetObject(resourceKey, new CultureInfo(cultureName));
                (_resourceCache[cultureName] as Dictionary<string, object>).Add(resourceKey, value);
            }
            
            Debug.WriteLine(resourceKey, "Key");
            Debug.WriteLine(value, "Value");
            Debug.WriteLine(className, "Path");
            return value;
        }

        /// <summary>
        /// Get a resource set for current class type.
        /// If the resource set is missing in cache, its loaded at once from database
        /// </summary>
        public IResourceReader ResourceReader
        {
            get
            {
                if (_ResourceReader != null)
                    return _ResourceReader as IResourceReader;

                _ResourceReader = new SmartITResourceReader(GetResourceCache(null));
                return _ResourceReader as IResourceReader;
            }
        }

        /// <summary>
        /// Manages caching of the Resource Sets. Once loaded the values are loaded from the
        /// cache only.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        private IDictionary GetResourceCache(string cultureName)
        {
            Debug.WriteLine(string.Format("GetResourceCache, culture = \"{0}\", className = {1}", cultureName, className));

            if (cultureName == null)
                cultureName = string.Empty;

            if (_resourceCache == null)
                _resourceCache = new Dictionary<string, IDictionary>();

            IDictionary resources = _resourceCache[cultureName] as Dictionary<string, object>;
            if (resources == null)
            {
                using (SmartITResourceDAL db = new SmartITResourceDAL())
                {
                    resources = db.GetResourcesByCulture(cultureName, className);
                    _resourceCache[cultureName] = resources;
                }
            }

            return resources;
        }  
    }
}