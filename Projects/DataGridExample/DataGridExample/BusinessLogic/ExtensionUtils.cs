using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;

namespace DataGridExample.BusinessLogic
{
    public static class ExtensionUtils
    {
        public static BindingList<T> ToBindingList<T>(this IQueryable<T> range)
        {
            return new BindingList<T>(range.ToList());
        }

        public static void SetColumnDataSource(this RadGridView grid, string columnName, object dataSource)
        {
            GridViewComboBoxColumn column = grid.Columns[columnName] as GridViewComboBoxColumn;
            column.DataSource = dataSource;
        }

        public static T ConvertRowToEntity<T>(GridViewCellInfoCollection cells) where T : class, new()
        {
            T entity = new T();
            foreach (var prop in entity.GetType().GetProperties())
            {
                object value = cells[prop.Name].Value;
                if (value != null)
                {
                    prop.SetValue(entity, value);
                }
            }

            return entity;
        }
    }
}
