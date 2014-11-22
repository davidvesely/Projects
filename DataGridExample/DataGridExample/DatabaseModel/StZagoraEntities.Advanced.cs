using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGridExample.BusinessLogic;

namespace DataGridExample.DatabaseModel
{
    public class StZagoraEntitiesEx : StZagoraEntities
    {
        public StZagoraEntitiesEx()
            : base()
        {
            this.ChangeDatabase(
                userId: "D400",
                password: "bobid400",
                configConnectionStringName: "StZagoraEntities"
            );
        }
    }
}
