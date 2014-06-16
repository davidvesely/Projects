using DataGridExample.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel;

namespace DataGridExample.BusinessLogic
{
    public class OrdersLayer : IDisposable
    {
        private OrdersEntities db = new OrdersEntities();

        public void Dispose()
        {
            db.Dispose();
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public BindingList<user> GetUsers()
        {
            db.users.Load();
            return db.users.Local.ToBindingList();
        }

        public BindingList<order> GetOrders()
        {
            db.orders.Load();
            return db.orders.Local.ToBindingList();
        }

        public BindingList<status> GetStatus()
        {
            db.status.Load();
            return db.status.Local.ToBindingList();
        }
    }
}
