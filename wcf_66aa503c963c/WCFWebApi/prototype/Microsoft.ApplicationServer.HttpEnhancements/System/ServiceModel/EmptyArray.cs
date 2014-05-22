// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.ServiceModel
{
    using System.Collections.Generic;

    internal class EmptyArray<T>
    {
        private static T[] instance;

        private EmptyArray()
        {
        }

        public static T[] Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T[0];
                }

                return instance;
            }
        }

        public static T[] Allocate(int n)
        {
            if (n == 0)
            {
                return Instance;
            }
            else
            {
                return new T[n];
            }
        }

        public static T[] ToArray(IList<T> collection)
        {
            if (collection.Count == 0)
            {
                return EmptyArray<T>.Instance;
            }
            else
            {
                T[] array = new T[collection.Count];
                collection.CopyTo(array, 0);
                return array;
            }
        }

        public static T[] ToArray(SynchronizedCollection<T> collection)
        {
            lock (collection.SyncRoot)
            {
                return EmptyArray<T>.ToArray((IList<T>)collection);
            }
        }
    }

    internal class EmptyArray
    {
        private static object[] instance = new object[0];

        private EmptyArray()
        {
        }

        public static object[] Instance
        {
            get
            {
                return instance;
            }
        }

        public static object[] Allocate(int n)
        {
            if (n == 0)
            {
                return Instance;
            }
            else
            {
                return new object[n];
            }
        }
    }
}