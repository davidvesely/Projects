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
            var query = from it in db.OPRITEM
                        where it.ID == id
                        select it;
            query.Load();
            return query.ToBindingList();
        }

        public void InsertOPRITEM(OPRITEM item)
        {
            db.OPRITEM.Add(item);

        }

        public List<string> GetDocType()
        {
            List<string> types = (from t in db.OPR
                                  select t.TYPE_DOC).Distinct().ToList();
            return types;
        }

        public int GetNextPos(int id)
        {
            return ((from oi in db.OPRITEM
                    where oi.ID == id
                    select (short?)oi.POS).Max() ?? 0) + 1;
        }

        public int GetNextID()
        {
            return (from o in db.OPR
                    select o.ID).Max() + 1;
        }
    }
}
