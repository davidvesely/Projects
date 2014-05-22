// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
namespace System.Net.Http.Formatting.OData.Test
{
    using System;
    using System.Net.Http.Formatting.OData.Test.ComplexTypes;
    using System.Net.Http.Formatting.OData.Test.EntityTypes;
    
    public static class TypeInitializer
    {                
        public static object GetInstance(SupportedTypes type, int index = 0, int maxReferenceDepth = 7)
        {
            if (index > DataSource.MaxIndex)
            {
                throw new ArgumentException(string.Format("The max supported index is : {0}", DataSource.MaxIndex));
            }
            
            return InternalGetInstance(type, index, new ReferenceDepthContext(maxReferenceDepth));
        }

        internal static object InternalGetInstance(SupportedTypes type, int index, ReferenceDepthContext context)
        {
            if (!context.IncreamentCounter())
            {
                return null;
            }

            if (type == SupportedTypes.Person)
            {
                return new Person(index, context);
            }
            else if (type == SupportedTypes.Employee)
            {
                return new Employee(index, context);                
            }
            else if (type == SupportedTypes.MultipleKeyEmployee)
            {
                return new MultipleKeyEmployee(index, context);            
            }
            else if (type == SupportedTypes.Address)
            {
                return new Address(index, context);
            }
            else if (type == SupportedTypes.WorkItem)
            {
                return new WorkItem() { EmployeeID = index, IsCompleted = false, NumberOfHours = 100, WorkItemID = 25 };
            }

            context.DecrementCounter();

            throw new ArgumentException(string.Format("Cannot initialize an instance for {0} type.", type.ToString()));

        }
    }    
}
