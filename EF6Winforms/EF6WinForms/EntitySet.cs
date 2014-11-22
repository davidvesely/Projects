using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EFWinforms
{
    /// <summary>
    /// Exposes an DbSet in the current DbContext as a bindable data source.
    /// </summary>
    /// <remarks>
    /// This class implements the IListSource interface and returns an IBindingList 
    /// built on top of the underlying DbSet.
    /// </remarks>
    public class EntitySet :
        IListSource,
        IQueryable
    {
        //-------------------------------------------------------------------------
        #region ** fields

        EntityDataSource _ds;       // EntityDataSource that created this set
        IQueryable _query;          // the value of the property
        IEntityBindingList _list;   // default view for this set
        PropertyInfo _pi;           // the property on the object context that gets the objects in this set
        Type _elementType;          // the type of object in this set
        ListDictionary _dctLookup;  // lookup dictionary (used to show and edit related entities in grid cells)

        #endregion

        //-------------------------------------------------------------------------
        #region ** ctor

        /// <summary>
        /// Initializes a new instance of a <see cref="EntitySet"/>.
        /// </summary>
        /// <param name="ds"><see cref="EntityDataSource"/> that owns the entities.</param>
        /// <param name="pi"><see cref="PropertyInfo"/> used to retrieve the set from the context.</param>
        internal EntitySet(EntityDataSource ds, PropertyInfo pi)
        {
            var type = pi.PropertyType;
            Debug.Assert(
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                type.GetGenericArguments().Length == 1);

            _ds = ds;
            _pi = pi;
            _elementType = type.GetGenericArguments()[0];
        }

        #endregion

        //-------------------------------------------------------------------------
        #region ** object model

        /// <summary>
        /// Gets the <see cref="EntityDataSource"/> that owns this entity set.
        /// </summary>
        public EntityDataSource DataSource
        {
            get { return _ds; }
        }
        /// <summary>
        /// Gets the name of this entity set.
        /// </summary>
        public string Name
        {
            get { return _pi != null ? _pi.Name : null; }
        }
        /// <summary>
        /// Gets the type of entity in this entity set.
        /// </summary>
        /// <remarks>
        /// Name chosen for consistency with EntitySet.ElementType 
        /// (EntityType would seem more appropriate).
        /// </remarks>
        public Type ElementType
        {
            get { return _elementType; }
        }
        /// <summary>
        /// Gets the <see cref="IQueryable"/> object that retrieves the entities in this set.
        /// </summary>
        public IQueryable Query
        {
            get 
            {
                if (_query == null && _ds.DbContext != null && _pi != null)
                {
                    _query = _pi.GetValue(_ds.DbContext, null) as IQueryable;
                }
                return _query; 
            }
        }
        /// <summary>
        /// Gets a list of the entities in the set that have not been deleted or detached.
        /// </summary>
        public IEnumerable ActiveEntities
        {
            get { return GetActiveEntities(Query); }
        }
        /// <summary>
        /// Gets a list of the entities in the set that have not been deleted or detached.
        /// </summary>
        internal static IEnumerable GetActiveEntities(IEnumerable query)
        {
            if (query != null)
            {
                foreach (object item in query)
                {
                    var state = item is EntityObject
                        ? ((EntityObject)item).EntityState
                        : EntityState.Unchanged;
                    switch (state)
                    {
                        case EntityState.Deleted:
                        case EntityState.Detached:
                            break;
                        default:
                            yield return item;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Cancels any pending changes on this entity set.
        /// </summary>
        internal void CancelChanges()
        {
            if (_list != null && Query != null)
            {
                var ctx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)_ds.DbContext).ObjectContext;
                ctx.Refresh(RefreshMode.StoreWins, Query);
                _list.Refresh();
            }
        }
        /// <summary>
        /// Refreshes this set's view by re-loading from the database.
        /// </summary>
        public void RefreshView()
        {
            if (_list != null && Query != null)
            {
                var ctx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)_ds.DbContext).ObjectContext;
                ctx.Refresh(RefreshMode.ClientWins, Query);
                _list.Refresh();
            }
        }
        /// <summary>
        /// Gets an <see cref="IBindingListView"/> that can be used as a data source for bound controls.
        /// </summary>
        public IBindingListView List
        {
            get { return GetBindingList(); }
        }
        /// <summary>
        /// Gets a dictionary containing entities as keys and their string representation as values.
        /// </summary>
        /// <remarks>
        /// The data map is useful for displaying and editing entities in grid cells.
        /// </remarks>
        public ListDictionary LookupDictionary
        {
            get
            {
                if (_dctLookup == null)
                {
                    _dctLookup = BuildLookupDictionary();
                }
                return _dctLookup;
            }
        }
        #endregion

        //-------------------------------------------------------------------------
        #region ** IListSource

        bool IListSource.ContainsListCollection
        {
            get { return false; }
        }
        IList IListSource.GetList()
        {
            return GetBindingList();
        }

        #endregion

        //-------------------------------------------------------------------------
        #region ** implementation

        // gets an IBindingListView for this entity set
        IBindingListView GetBindingList()
        {
            if (_list == null)
            {
                // create the list
                var listType = typeof(EntityBindingList<>);
                listType = listType.MakeGenericType(this.ElementType);
                _list = (IEntityBindingList)Activator.CreateInstance(listType, _ds, this.Query, Guid.NewGuid().ToString());// this.Name);

                // and listen to changes in the new list
                _list.ListChanged += _list_ListChanged;
            }
            return _list;
        }

        // update data map when list changes
        void _list_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_dctLookup != null)
            {
                // clear old dictionary
                _dctLookup.Clear();

                // build new dictionary
                var map = BuildLookupDictionary(_list);
                foreach (var kvp in map)
                {
                    _dctLookup.Add(kvp.Key, kvp.Value);
                }
            }
        }

        // build a data map for this entity set
        ListDictionary BuildLookupDictionary()
        {
            return BuildLookupDictionary(ActiveEntities);
        }
        ListDictionary BuildLookupDictionary(IEnumerable entities)
        {
            // if the entity implements "ToString", then use it
            var mi = _elementType.GetMethod("ToString");
            if (mi != null && mi.DeclaringType == _elementType)
            {
                var list = new List<KVPair>();
                foreach (object item in entities)
                {
                    list.Add(new KVPair(item, item.ToString()));
                }
                return BuildLookupDictionary(list);
            }

            // use "DefaultProperty"
            var atts = _elementType.GetCustomAttributes(typeof(DefaultPropertyAttribute), false);
            if (atts != null && atts.Length > 0)
            {
                var dpa = atts[0] as DefaultPropertyAttribute;
                var pi = _elementType.GetProperty(dpa.Name);
                if (pi != null && pi.PropertyType == typeof(string))
                {
                    var list = new List<KVPair>();
                    foreach (object item in entities)
                    {
                        list.Add(new KVPair(item, (string)pi.GetValue(item, null)));
                    }
                    return BuildLookupDictionary(list);
                }
            }

            // no default property: look for properties of type string with 
            // names that contain "Name" or "Description"
            foreach (var pi in _elementType.GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    if (pi.Name.IndexOf("Name", StringComparison.OrdinalIgnoreCase) > -1 ||
                        pi.Name.IndexOf("Description", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        var list = new List<KVPair>();
                        foreach (object item in entities)
                        {
                            list.Add(new KVPair(item, (string)pi.GetValue(item, null)));
                        }
                        return BuildLookupDictionary(list);
                    }
                }
            }

            // no dice
            return null;
        }
        ListDictionary BuildLookupDictionary(List<KVPair> list)
        {
            // sort list display value
            list.Sort();

            // create data map
            var map = new ListDictionary();
            foreach (var kvp in list)
            {
                map.Add(kvp.Key, kvp.Value);
            }

            // done
            return map;
        }
        class KVPair : IComparable
        {
            public KVPair(object key, string value)
            {
                Key = key;
                Value = value;
            }
            public object Key { get; set; }
            public string Value { get; set; }
            int IComparable.CompareTo(object obj)
            {
 	            return string.Compare(this.Value, ((KVPair)obj).Value, StringComparison.OrdinalIgnoreCase);
            }
        }

        #endregion

        //-------------------------------------------------------------------------
        #region ** IQueryable

        Type IQueryable.ElementType
        {
            get { return _elementType; }
        }
        Expression IQueryable.Expression
        {
            get { return Query.Expression; }
        }
        IQueryProvider IQueryable.Provider
        {
            get { return Query.Provider; }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Query.GetEnumerator();
        }

        #endregion
    }
    /// <summary>
    /// Collection of EntitySet objects.
    /// </summary>
    public class EntitySetCollection : ObservableCollection<EntitySet>
    {
        public EntitySet this[string name]
        {
            get
            {
                var index = this.IndexOf(name);
                return index > -1 ? this[index] : null;
            }
        }
        public bool Contains(string name)
        {
            return IndexOf(name) > -1;
        }
        public int IndexOf(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                    return i;
            }
            return -1;
        }
    }
    /// <summary>
    /// Dictionary that implements IListSource (used for implementing lookup dictionaries)
    /// </summary>
    public class ListDictionary : Dictionary<object, string>, IListSource
    {
        public bool ContainsListCollection
        {
            get { throw new NotImplementedException(); }
        }
        public IList GetList()
        {
            var list = new List<KeyValuePair<object, string>>();
            foreach (var item in this)
            {
                list.Add(item);
            }
            return list;
        }
    }
}
