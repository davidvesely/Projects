using EntityFramework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01.CustomersDAO
{
    public class CustomersDAO
    {
        public static void InsertCustomer(
            NorthwindEntities northwindDBContext, string customerID, string companyName, string contactName = null,
            string contactTitle = null, string address = null, string city = null, string region = null,
            string postalCode = null, string country = null, string phone = null, string fax = null)
        {
            var customer = new Customer
            {
                CustomerID = customerID,
                CompanyName = companyName,
                ContactName = contactName,
                ContactTitle = contactTitle,
                Address = address,
                City = city,
                Region = region,
                PostalCode = postalCode,
                Country = country,
                Phone = phone,
                Fax = fax
            };

            northwindDBContext.Customers.Add(customer);

            northwindDBContext.SaveChanges();
            Console.WriteLine("Row is inserted.");
        }

        public static void UpdateCustomer(NorthwindEntities northwindDBContext,
            string customerID, string companyName, string contactName = null, string contactTitle = null,
            string address = null, string city = null, string region = null, string postalCode = null,
            string country = null, string phone = null, string fax = null)
        {
                Customer customerToUpdate = northwindDBContext.Customers.First(x => x.CustomerID == customerID);

                customerToUpdate.CompanyName = companyName ?? customerToUpdate.CompanyName;
                customerToUpdate.ContactName = contactName ?? customerToUpdate.ContactName;
                customerToUpdate.ContactTitle = contactTitle ?? customerToUpdate.ContactTitle;
                customerToUpdate.Address = address ?? customerToUpdate.Address;
                customerToUpdate.City = city ?? customerToUpdate.City;
                customerToUpdate.Region = region ?? customerToUpdate.Region;
                customerToUpdate.PostalCode = postalCode ?? customerToUpdate.PostalCode;
                customerToUpdate.Country = country ?? customerToUpdate.Country;
                customerToUpdate.Phone = phone ?? customerToUpdate.Phone;
                customerToUpdate.Fax = fax ?? customerToUpdate.Fax;

                northwindDBContext.SaveChanges();
                Console.WriteLine("Row is updated.");
        }

        public static void DeleteCustomer(NorthwindEntities northwindDBContext, string customerID)
        {
            Customer customer = northwindDBContext.Customers.First(x => x.CustomerID == customerID);
            northwindDBContext.Customers.Remove(customer);
            northwindDBContext.SaveChanges();
            Console.WriteLine("Row is deleted.");
        }

        public static void CustomersWithOrdersIn1997FromCanada(NorthwindEntities northwindDBContext)
        {
            var customers = northwindDBContext.Orders
                                              .Where(o => o.OrderDate.Value.Year == 1997 && o.ShipCountry == "Canada")
                                              .GroupBy(o => o.Customer.CompanyName)
                                              .ToList();
            northwindDBContext.SaveChanges(); 

            foreach (var customer in customers)
            {
                Console.WriteLine(customer.Key);
            }
        }

        public static void CustomersWithOrdersIn1997FromCanada_SQLQuery(NorthwindEntities northwindDBContext)
        {
            var customers = northwindDBContext.Database.SqlQuery<string>(@"SELECT c.CompanyName FROM Orders o 
                                                                                JOIN Customers c ON o.CustomerID = c.CustomerID
                                                                        WHERE YEAR(o.OrderDate) = 1997 AND o.ShipCountry = 'Canada'
                                                                        GROUP BY c.CompanyName");
            northwindDBContext.SaveChanges();

            foreach (var customer in customers)
            {
                Console.WriteLine(customer);
            }
        }
    }
}
