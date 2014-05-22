// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http.Formatting.OData.Test.EntityTypes
{
    public class WorkItem
    {
        //Automatically is made Key
        public int WorkItemID;

        public int EmployeeID;

        public bool IsCompleted;

        public float NumberOfHours;
    }

    // Used as a type on which keys can be explicitly set
    public class DerivedWorkItem : WorkItem
    {

    }
}
