using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;

namespace WebsiteTest.Code
{
    public class SmartITResourceReader : IResourceReader
    {
        private IDictionary _resources;

        public SmartITResourceReader(IDictionary resources)
        {
            _resources = resources;
        }

        IDictionaryEnumerator IResourceReader.GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        void IResourceReader.Close()
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        void IDisposable.Dispose()
        {
        }
    }

    /// <summary>
    /// Summary description for SmartITResourceReader
    /// </summary>
    //public class SmartITResourceReader : IResourceReader, IEnumerable<KeyValuePair<string, string>>
    //{
    //    private Dictionary<string, string> resourceDictionary;

    //    public SmartITResourceReader(Dictionary<string, string> resourceDictionary)
    //    {
    //        this.resourceDictionary = resourceDictionary;
    //    }

    //    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    //    {
    //        foreach (var res in this.resourceDictionary)
    //        {
    //            yield return res;
    //        }
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this.GetEnumerator();
    //    }

    //    public void Close()
    //    {
    //        resourceDictionary = null;
    //    }

    //    IDictionaryEnumerator IResourceReader.GetEnumerator()
    //    {
    //        return this.resourceDictionary.GetEnumerator();
    //    }

    //    public void Dispose()
    //    {
    //        resourceDictionary = null;
    //    }
    //}
}