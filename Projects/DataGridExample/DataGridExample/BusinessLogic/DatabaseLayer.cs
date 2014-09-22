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
    public class DatabaseLayer : IDisposable
    {
        private StZagoraEntities db = new StZagoraEntities();

        public void Dispose()
        {
            db.Dispose();
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        // Заявка за подаване на DataSource на грид
        public BindingList<OPR> GetOPRRows()
        {
            //var objects = db.OPR.ToList();

            db.OPR
                //.OrderBy(a => a.NOM_DOC)
                .OrderBy(a => a.ID)
                .Load();
            return db.OPR.Local.ToBindingList();
        }

        public BindingList<OPRITEM> GetOPRITEMRows(int id)
        {
            // LINQ заявка, за взимането на всички редове с ID = id
            var query = from it in db.OPRITEM
                        where it.ID == id
                        select it;
            // Данните се зареждат в паметта
            query.Load();
            // За да може грида да следи промяната на данните, и след това да ги записва, query-то се превръща в BindingList
            return query.ToBindingList();
        }

        public void InsertOPRITEM(OPRITEM item)
        {
            db.OPRITEM.Add(item);
        }

        public List<string> GetDocType()
        {
            // Взима всички уникални (distinct) стойности в колоната TYPE_DOC
            List<string> types = (from t in db.OPR
                                  select t.TYPE_DOC).Distinct().ToList();
            return types;
        }

        public int GetNextPos(int id)
        {
            // LINQ заявка за взимане на MAX стойност на POS със съответното ID
            // select (short?)oi.POS -> това се прави защото ако select-а не върне данни, ще върне null
            // short? е nullable
            var maxId = (from oi in db.OPRITEM
                         where oi.ID == id
                         select (short?)oi.POS).Max();
            return (maxId ?? 0) + 1;
        }

        public int GetNextID()
        {
            return (from o in db.OPR
                    select o.ID).Max() + 1;
        }
    }
}
