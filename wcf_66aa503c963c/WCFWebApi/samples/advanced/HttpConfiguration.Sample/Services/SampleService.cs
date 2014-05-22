// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace HttpConfiguration.Sample
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    /// <summary>
    /// Sample WCF service
    /// </summary>
    [Export]
    public class SampleService
    {
        private static SampleItems sampleItems;

        public SampleService()
        {
            sampleItems = new SampleItems();
            sampleItems.Add(new SampleItem { Id = 1, StringValue = "Default item 1" });
            sampleItems.Add(new SampleItem { Id = 2, StringValue = "Default item 2" });
            sampleItems.Add(new SampleItem { Id = 3, StringValue = "Default item 3" });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleService"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <remarks>This is used by <see cref="SetInstanceProvider"/></remarks>
        [ImportingConstructor]
        public SampleService(SampleItems items)
        {
            sampleItems = items;
        }

        [WebGet(UriTemplate = "")]
        public SampleItems GetCollection()
        {
            return sampleItems;
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public SampleItem Create(SampleItem instance)
        {
            sampleItems.Add(instance);
            return instance;
        }

        [WebGet(UriTemplate = "item/{id}")]
        public SampleItem GetItem(int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException("The id must be greater than zero.");
            }

            return sampleItems.Find(item => item.Id == id);
        }

        [WebGet(UriTemplate = "stream/{text}")]
        public Stream GetStream(string text)
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memoryStream);
            writer.Write(text);
            writer.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }

    [DataContract(Name = "DataContractSampleItem")]
    public class SampleItem
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string StringValue { get; set; }

        public override string ToString()
        {
            return Id + "," + StringValue;
        }
    }

    public class SampleItems : List<SampleItem>
    {
        public override string ToString()
        {
            string str = string.Empty;
            foreach (var item in this)
            {
                str += item + Environment.NewLine;
            }

            return str;
        }
    }
}