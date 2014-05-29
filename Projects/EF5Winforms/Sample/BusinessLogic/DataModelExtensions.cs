using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample
{
    /// <summary>
    /// Add a field with total order amount.
    /// </summary>
    public partial class Order
    {
        // calculate amount for this order
        public decimal Amount
        {
            get
            {
                var q = from od in this.Order_Details select od.Amount;
                return q.Sum();
            }
        }
    }
    /// <summary>
    /// Add a field with order detail amount.
    /// </summary>
    public partial class Order_Detail 
    {
        // calculate amount for this order detail
        public decimal Amount
        {
            get { return Quantity * UnitPrice * (1 - (decimal)Discount); }
        }
    }
    /// <summary>
    /// Add a field with the employee's full name
    /// </summary>
    public partial class Employee
    {
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
        public override string ToString()
        {
            return FullName;
        }
    }
}
