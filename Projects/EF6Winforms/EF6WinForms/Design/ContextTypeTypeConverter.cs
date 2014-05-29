using System;
using System.Globalization;
using System.Reflection;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EFWinforms.Design
{
    /// <summary>
    /// TypeConverter that shows a list of types that inherit from ObjectContext and
    /// can be assigned to the ObjectContextType property.
    /// </summary>
    public class DbContextTypeTypeConverter : ReferenceConverter
    {
        //----------------------------------------------------------------------------
        #region ** ctor

        public DbContextTypeTypeConverter()
            : base(typeof(Type))
        {
        }

        #endregion

        //----------------------------------------------------------------------------
        #region ** overrides

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<Type> values = new List<Type>();

            // value for no data source
            values.Add(null);
            
            // all types that derive from ObjectContext
            foreach (Type t in GetDbContextTypes(context))
            {
                values.Add(t);
            }

            // done
            return new StandardValuesCollection(values);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var values = GetDbContextTypes(context);
                foreach (var t in values)
                {
                    if (t.ToString() == (string)value)
                    {
                        return t;
                    }
                }
                return null;
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Type)
            {
                return value.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        List<Type> GetDbContextTypes(ITypeDescriptorContext context)
        {
            var values = new List<Type>();
            var tds = context.GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService;
            if (tds != null)
            {
                foreach (Type t in tds.GetTypes(typeof(System.Data.Entity.DbContext), true))
                {
                    if (t.IsPublic && t.IsVisible && !t.IsAbstract && t != typeof(System.Data.Entity.DbContext))
                    {
                        values.Add(t);
                    }
                }
            }
            return values;
        }

        #endregion
    }
}
