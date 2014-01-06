using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Data;

namespace _01.CustomersDAO
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new NorthwindEntities();
            Console.WriteLine("CustomersWithOrdersIn1997FromCanada");
            CustomersDAO.CustomersWithOrdersIn1997FromCanada(context);
            Console.WriteLine("CustomersWithOrdersIn1997FromCanada_SQLQuery");
            CustomersDAO.CustomersWithOrdersIn1997FromCanada_SQLQuery(context);
            Console.WriteLine("InsertCustomer");
            CustomersDAO.InsertCustomer(context, "ABCDE", "Company1", "David Vesely");
        }
    }
}
