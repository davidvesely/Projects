using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace EFWinforms
{
    /// <summary>
    /// DataSource component that encapsulates an EntityFramework DbContext and exposes
    /// all DbSets in the context as binding EntitySets.
    /// </summary>
    /// <remarks>
    /// For binding purposes, the EntityDataSource component corresponds to a DataSet and 
    /// the EntitySets correspond to the contained DataTables. For example, to bind a grid 
    /// to an object set you should set the grid's DataSource property to the DataSource 
    /// component and the DataMember property to one of the EntitySets.
    /// </remarks>
    [
    ProvideProperty("AutoLookup", typeof(Control)),
    ToolboxItem(true),
    ToolboxBitmap(typeof(EntityDataSource), "EntityDataSource.png"),
    DefaultProperty("DbContextType")
    ]
    public partial class EntityDataSource :
        Component,
        IListSource,
        IExtenderProvider
    {
        //-------------------------------------------------------------------------
        #region ** fields

        DbContext _ctx;
        EntitySetCollection _entSets = new EntitySetCollection();
        Type _ctxType; // to support design-time

        #endregion

        //-------------------------------------------------------------------------
        #region ** ctors

        /// <summary>
        /// Initializes a new instance of a <see cref="EntityDataSource"/>.
        /// </summary>
        public EntityDataSource()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes a new instance of a <see cref="EntityDataSource"/>.
        /// </summary>
        public EntityDataSource(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        #endregion

        //-------------------------------------------------------------------------
        #region ** object model

        /// <summary>
        /// Gets or sets the type of DbContext to use as a data source.
        /// </summary>
        /// <remarks>
        /// This property is normally set at design time. Once it is set, the 
        /// component will automatically create an DbContext of the appropriate type.
        /// </remarks>
        [
        TypeConverter(typeof(Design.DbContextTypeTypeConverter)),
        ]
        public Type DbContextType
        {
            get { return _ctxType; }
            set
            {
                if (value != _ctxType)
                {
                    // notify
                    OnDbContextTypeChanging(EventArgs.Empty);

                    // clear existing object context
                    _ctx = null;

                    // set new object context type 
                    _ctxType = value;

                    // generate object sets (will re-create object context if appropriate)
                    GenerateEntitySets(_ctxType);

                    // notify
                    OnDbContextTypeChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the DbContext used as a data source.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public DbContext DbContext
        {
            get
            {
                if (_ctx == null && _ctxType != null && !DesignMode)
                {
                    try
                    {
                        DbContext = Activator.CreateInstance((Type)_ctxType) as DbContext;
                    }
                    catch { }
                }
                return _ctx;
            }
            set
            {
                if (_ctx != value)
                {
                    // notify
                    OnDbContextChanging(EventArgs.Empty);

                    // save the new context
                    _ctx = value;

                    // update the context type
                    _ctxType = _ctx != null ? _ctx.GetType() : null;

                    // generate object sets
                    GenerateEntitySets(_ctxType);

                    // notify
                    OnDbContextChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets the collection of EntitySets available in this EntityDataSource.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public EntitySetCollection EntitySets
        {
            get { return _entSets; }
        }
        /// <summary>
        /// Saves all changes made to all entity sets back to the database.
        /// </summary>
        public int SaveChanges()
        {
            // notify
            var e = new CancelEventArgs();
            OnSavingChanges(e);

            // save the changes
            int count = 0;
            if (!e.Cancel)
            {
                try
                {
                    count = _ctx.SaveChanges();
                    Debug.WriteLine(string.Format("Done. {0} changes saved.", count));

                    // notify
                    OnSavedChanges(e);
                }
                catch (Exception x)
                {
                    OnDataError(new DataErrorEventArgs(x));
                }
            }

            // done
            return count;
        }
        /// <summary>
        /// Cancels all changes made to all entity sets.
        /// </summary>
        public void CancelChanges()
        {
            // notify
            var e = new CancelEventArgs();
            OnCancelingChanges(e);

            if (_ctx != null && !e.Cancel)
            {
                try
                {
                    foreach (var entSet in EntitySets)
                    {
                        entSet.CancelChanges();
                    }
                    //_ctx.AcceptAllChanges();
                }
                catch (Exception x)
                {
                    OnDataError(new DataErrorEventArgs(x));
                }

                // notify
                OnCanceledChanges(EventArgs.Empty);
            }
            Debug.WriteLine("Done. All changes canceled.");
        }
        /// <summary>
        /// Refreshes all views by loading their data from the database.
        /// </summary>
        public void Refresh()
        {
            // notify
            var e = new CancelEventArgs();
            OnRefreshing(e);

            if (!e.Cancel)
            {
                try
                {
                    foreach (var entSet in EntitySets)
                    {
                        entSet.RefreshView();
                    }
                }
                catch (Exception x)
                {
                    OnDataError(new DataErrorEventArgs(x));
                }

                // notify
                OnRefreshed(EventArgs.Empty);
                Debug.WriteLine("Done. All views refreshed.");
            }
        }
        /// <summary>
        /// Gets a lookup dictionary for a given element type.
        /// </summary>
        /// <param name="elementType">Type of element for which to return a lookup dictionary.</param>
        /// <returns>A lookup dictionary for a given element type.</returns>
        /// <remarks>
        /// <para>The lookup dictionary has keys that correspond to the items on a list and
        /// values that contain a string representation of the items.</para>
        /// <para>When lists of entities are sorted on a column that contains entity references, 
        /// the lookup dictionary is used to provide the sorting order. For example, if you sort a 
        /// list of products by category, the data map associated with the Categories list determines 
        /// the order in which the categories are compared while sorting.</para>
        /// <para>Lookup dictionaries are especially useful for setting the DataMap property on the 
        /// columns of a C1FlexGrid control. For example, if a C1FlexGrid is bound to a list of products,
        /// and the Product entity contains references to other entities (e.g. Category or Supplier),
        /// then the grid uses the lookup dictionary to provide a useful string representation of the 
        /// related entities (e.g. category name). The grid also uses the data map to provide combo boxes 
        /// for editing the cell values.</para>
        /// <para>If you add a C1FlexGrid control to a form that contains a 
        /// <see cref="EntityDataSource"/>, the data source will provide an extender property 
        /// called <b>AutoLookup</b> that may be set on the grids to create and assign lookup 
        /// dictionaries to their columns automatically.</para>
        /// </remarks>
        public ListDictionary GetLookupDictionary(Type elementType)
        {
            foreach (var es in EntitySets)
            {
                if (es.ElementType == elementType)
                {
                    return es.LookupDictionary;
                }
            }
            return null;
        }
        /// <summary>
        /// Creates an IBindingList based on a given query.
        /// </summary>
        /// <param name="query"><see cref="IEnumerable"/> used as a data source for the list.</param>
        /// <returns>An <see cref="IBindingList"/> that provides a sortable/filterable view of the data.</returns>
        public IBindingList CreateView(IEnumerable query)
        {
            // get the query type
            Type type = typeof(object);
            foreach (var item in query)
            {
                type = item.GetType();
                break;
            }

            // create the binding list
            var listType = typeof(EntityBindingList<>);
            listType = listType.MakeGenericType(type);
            var list = (IEntityBindingList)Activator.CreateInstance(listType, this, query, type.Name);
            return list;
        }
        /// <summary>
        /// Gets a value that determines whether the component is in design mode.
        /// </summary>
        new internal protected bool DesignMode
        {
            get { return base.DesignMode; }
        }
        #endregion

        //-------------------------------------------------------------------------
        #region ** implementation

        /// <summary>
        /// Populates the EntitySets collection from the current DomainContext.
        /// </summary>
        void GenerateEntitySets(Type ctxType)
        {
            _entSets.Clear();
            if (ctxType != null)
            {
                var ctx = DbContext;
                foreach (var pi in ctxType.GetProperties())
                {
                    var type = pi.PropertyType;
                    if (type.IsGenericType &&
                        type.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                        type.GetGenericArguments().Length == 1)
                    {
                        var objSet = new EntitySet(this, pi);
                        _entSets.Add(objSet);
                    }
                }
            }
        }

        #endregion

        //-------------------------------------------------------------------------
        #region ** events

        /// <summary>
        /// Occurs before the value of the <see cref="DbContextType"/> property changes.
        /// </summary>
        public event EventHandler DbContextTypeChanging;
        /// <summary>
        /// Raises the <see cref="DbContextTypeChanging"/> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> that contains the event parameters.</param>
        protected virtual void OnDbContextTypeChanging(EventArgs e)
        {
            if (DbContextTypeChanging != null)
                DbContextTypeChanging(this, e);
        }
        /// <summary>
        /// Occurs after the value of the <see cref="DbContextType"/> property changes.
        /// </summary>
        public event EventHandler DbContextTypeChanged;
        /// <summary>
        /// Raises the <see cref="DbContextTypeChanged"/> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> that contains the event parameters.</param>
        protected virtual void OnDbContextTypeChanged(EventArgs e)
        {
            if (DbContextTypeChanged != null)
                DbContextTypeChanged(this, e);
        }
        /// <summary>
        /// Occurs before the value of the <see cref="DbContext"/> property changes.
        /// </summary>
        public event EventHandler DbContextChanging;
        /// <summary>
        /// Raises the <see cref="DbContextChanging"/> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> that contains the event parameters.</param>
        protected virtual void OnDbContextChanging(EventArgs e)
        {
            if (DbContextChanging != null)
                DbContextChanging(this, e);
        }
        /// <summary>
        /// Occurs after the value of the <see cref="DbContext"/> property changes.
        /// </summary>
        public event EventHandler DbContextChanged;
        /// <summary>
        /// Raises the <see cref="DbContextChanged"/> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> that contains the event parameters.</param>
        protected virtual void OnDbContextChanged(EventArgs e)
        {
            if (DbContextChanged != null)
                DbContextChanged(this, e);
        }
        /// <summary>
        /// Occurs before changes are saved to the database.
        /// </summary>
        public event CancelEventHandler SavingChanges;
        /// <summary>
        /// Raises the <see cref="SavingChanges"/> event.
        /// </summary>
        /// <param name="e"><see cref="CancelEventArgs"/> that contains the event parameters.</param>
        protected virtual void OnSavingChanges(CancelEventArgs e)
        {
            if (SavingChanges != null)
                SavingChanges(this, e);
        }
        /// <summary>
        /// Occurs after changes are saved to the database.
        /// </summary>
        public event EventHandler SavedChanges;
        /// <summary>
        /// Raises the <see cref="SavedChanges"/> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> that contains the event parameters.</param>
        protected virtual void OnSavedChanges(EventArgs e)
        {
            if (SavedChanges != null)
                SavedChanges(this, e);
        }
        /// <summary>
        /// Occurs before changes are canceled and values are reloaded from the database.
        /// </summary>
        public event CancelEventHandler CancelingChanges;
        /// <summary>
        /// Raises the <see cref="CancelingChanges"/> event.
        /// </summary>
        /// <param name="e"><see cref="CancelEventArgs"/> that contains the event parameters.</param>
        protected virtual void OnCancelingChanges(CancelEventArgs e)
        {
            if (CancelingChanges != null)
                CancelingChanges(this, e);
        }
        /// <summary>
        /// Occurs after changes are canceled and values are reloaded from the database.
        /// </summary>
        public event EventHandler CanceledChanges;
        /// <summary>
        /// Raises the <see cref="CanceledChanges"/> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> that contains the event parameters.</param>
        protected virtual void OnCanceledChanges(EventArgs e)
        {
            if (CanceledChanges != null)
                CanceledChanges(this, e);
        }
        /// <summary>
        /// Occurs before values are refreshed from the database.
        /// </summary>
        public event CancelEventHandler Refreshing;
        /// <summary>
        /// Raises the <see cref="Refreshing"/> event.
        /// </summary>
        /// <param name="e"><see cref="CancelEventArgs"/> that contains the event parameters.</param>
        protected virtual void OnRefreshing(CancelEventArgs e)
        {
            if (Refreshing != null)
                Refreshing(this, e);
        }
        /// <summary>
        /// Occurs after values are refreshed from the database.
        /// </summary>
        public event EventHandler Refreshed;
        /// <summary>
        /// Raises the <see cref="Refreshed"/> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> that contains the event parameters.</param>
        protected virtual void OnRefreshed(EventArgs e)
        {
            if (Refreshed != null)
                Refreshed(this, e);
        }
        /// <summary>
        /// Occurs when an error is detected while loading data from or saving data to the database.
        /// </summary>
        public event EventHandler<DataErrorEventArgs> DataError;
        /// <summary>
        /// Raises the <see cref="DataError"/> event.
        /// </summary>
        /// <param name="e"><see cref="DataErrorEventArgs"/> that contains the event parameters.</param>
        protected virtual void OnDataError(DataErrorEventArgs e)
        {
            if (DataError != null)
            {
                DataError(this, e);
            }
            if (!e.Handled)
            {
                throw e.Exception;
            }
        }

        #endregion

        //-------------------------------------------------------------------------
        #region ** IListSource

        bool IListSource.ContainsListCollection
        {
            get { return true; }
        }
        IList IListSource.GetList()
        {
            // This method is completely undocumented in MSDN.
            //
            // Instead of returning a list of the lists available (nice and logical),
            // you should return a list with ONE object, which exposes the lists available 
            // as if they were properties of this ONE object. UGH!
            //
            var list = new List<Design.EntitySetTypeDescriptor>();
            list.Add(new Design.EntitySetTypeDescriptor(this));
            return list;
        }

        #endregion

        //-------------------------------------------------------------------------
        #region ** IExtenderProvider

        /// <summary>
        /// We can extend DataGridView and C1FlexGrid controls.
        /// </summary>
        /// <param name="extendee"></param>
        /// <returns></returns>
        bool IExtenderProvider.CanExtend(object extendee)
        {
            if (extendee is Control)
            {
                for (var type = extendee.GetType(); type != null; type = type.BaseType)
                {
                    // DataGridView
                    if (type == typeof(DataGridView))
                    {
                        return true;
                    }

                    // C1FlexGrid
                    var typeName = type.ToString();
                    if (typeName.IndexOf("C1FlexGrid", StringComparison.Ordinal) > -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Add or remove automatic data maps for the columns on a C1FlexGrid control.
        /// </summary>
        /// <param name="grid">DataGridView or C1FlexGrid control.</param>
        /// <param name="autoLookups">Whether to enable or disable automatic data maps for the columns on the <paramref name="grid"/> control.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetAutoLookup(Control grid, bool autoLookups)
        {
            var oldAutoLookups = _autoLookup.ContainsKey(grid);
            if (oldAutoLookups != autoLookups)
            {
                if (autoLookups)
                {
                    _autoLookup.Add(grid, true);
                }
                else
                {
                    _autoLookup.Remove(grid);
                }
                if (!DesignMode)
                {
                    EnableAutoLookup(grid, autoLookups);
                }
            }
        }
        /// <summary>
        /// Gets a value that determines whether automatic data maps are enabled for a given C1FlexGrid control.
        /// </summary>
        /// <param name="grid">DataGridView or C1FlexGrid control.</param>
        /// <returns>Whether automatic data maps are is enabled for the columns on the <paramref name="grid"/> control.</returns>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DefaultValue(false),
        ]
        public bool GetAutoLookup(Control grid)
        {
            return _autoLookup.ContainsKey(grid);
        }

        // enabled/disable automatic data maps for a control
        Dictionary<Control, bool> _autoLookup = new Dictionary<Control, bool>();
        static void EnableAutoLookup(Control control, bool map)
        {
            // get event handlers
            var ctlType = control.GetType();
            var dsChanged = ctlType.GetEvent("DataSourceChanged");
            var dmChanged = ctlType.GetEvent("DataMemberChanged");
            var bcChanged = ctlType.GetEvent("BindingContextChanged");

            // sanity
            if (dsChanged == null || dmChanged == null || bcChanged == null)
            {
                throw new Exception("Cannot connect event handlers for this control.");
            }

            // connect/disconnect event handlers
            var handler = new EventHandler(control_DataSourceChanged);
            if (map)
            {
                dsChanged.AddEventHandler(control, handler);
                dmChanged.AddEventHandler(control, handler);
                bcChanged.AddEventHandler(control, handler);
            }
            else
            {
                dsChanged.RemoveEventHandler(control, handler);
                dmChanged.RemoveEventHandler(control, handler);
                bcChanged.RemoveEventHandler(control, handler);
            }
        }
        static void control_DataSourceChanged(object sender, EventArgs e)
        {
            CustomizeGrid(sender as Control);
        }

        // customize the columns of a DataGridView or C1FlexGrid control 
        // use dynamic type for C1FlexGrid in order to avoid dependencies.
        static void CustomizeGrid(Control ctl)
        {
            dynamic grid = ctl;
            if (grid.DataSource != null && grid.BindingContext != null)
            {
                // get currency manager
                CurrencyManager cm = null;
                try
                {
                    cm = grid.BindingContext[grid.DataSource, grid.DataMember] as CurrencyManager;
                }
                catch { }

                // get source list from currency manager bound directly to EntityDataSource
                var list = cm != null ? cm.List as IEntityBindingList : null;

                // if failed, try binding via BindingSource component
                if (list == null && cm.List is BindingSource)
                {
                    list = ((BindingSource)(cm.List)).List as IEntityBindingList;
                }

                // customize the columns
                if (list != null && list.DataSource != null)
                {
                    var entType = list.ElementType;
                    var entDataSource = list.DataSource;

                    // customize grid columns
                    var dgv = ctl as DataGridView;
                    if (dgv != null)
                    {
                        // customize DataGridView
                        CustomizeDataGridView(dgv, entType, entDataSource);
                    }
                    else
                    {
                        // customize C1FlexGrid
                        CustomizeFlexGrid(ctl, entType, entDataSource);
                    }
                }
            }
        }

        // customize the columns of a DataGridView
        static void CustomizeDataGridView(DataGridView dgv, Type entType, EntityDataSource entDataSource)
        {
            // configure columns
            for (int colIndex = 0; colIndex < dgv.Columns.Count; colIndex++)
            {
                // get column
                var c = dgv.Columns[colIndex];

                // if the column is a key, make it read-only
                var pi = entType.GetProperty(c.DataPropertyName);
                if (pi != null)
                {
                    var atts = pi.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), false);
                    if (atts.Length > 0)
                    {
                        var att = atts[0] as EdmScalarPropertyAttribute;
                        if (att.EntityKeyProperty)
                        {
                            c.ReadOnly = true;
                        }
                    }

                    // if the column holds entities, give it a data map
                    var type = pi.PropertyType;
                    if (!type.IsPrimitive)
                    //if (typeof(EntityObject).IsAssignableFrom(type))
                    {
                        var map = entDataSource.GetLookupDictionary(type);
                        if (map != null)
                        {
                            var col = new DataGridViewComboBoxColumn();
                            col.HeaderText = c.HeaderText;
                            col.DataPropertyName = c.DataPropertyName;
                            col.Width = c.Width;
                            col.DefaultCellStyle = c.DefaultCellStyle;
                            col.DataSource = map;
                            col.ValueMember = "Key";
                            col.DisplayMember = "Value";
                            dgv.Columns.RemoveAt(colIndex);
                            dgv.Columns.Insert(colIndex, col);
                        }
                    }
                }
            }
        }

        // customize the columns of a C1FlexGrid control 
        // use dynamic type in order to avoid dependencies.
        static void CustomizeFlexGrid(dynamic flex, Type entType, EntityDataSource entDataSource)
        {
            // configure columns
            foreach (dynamic c in flex.Cols)
            {
                // if the column is a key, make it read-only
                var pi = entType.GetProperty(c.Name);
                if (pi != null)
                {
                    var atts = pi.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), false);
                    if (atts.Length > 0)
                    {
                        var att = atts[0] as EdmScalarPropertyAttribute;
                        if (att.EntityKeyProperty)
                        {
                            c.AllowEditing = false;
                        }
                    }

                    // if the column holds entities, give it a data map
                    var type = pi.PropertyType;
                    if (!type.IsPrimitive)// typeof(EntityObject).IsAssignableFrom(type))
                    {
                        c.DataMap = entDataSource.GetLookupDictionary(type);
                    }
                }
            }
        }

        #endregion
    }
    /// <summary>
    /// Provides data for the <see cref="DataError"/> event.
    /// </summary>
    public class DataErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="DataErrorEventArgs"/>.
        /// </summary>
        /// <param name="x"><see cref="Exception"/> that triggered the event.</param>
        public DataErrorEventArgs(Exception x)
        {
            Exception = x;
        }
        /// <summary>
        /// Gets or sets the <see cref="Exception"/> that triggered the event.
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// Whether the error was handled and the source exception should be ignored.
        /// </summary>
        public bool Handled { get; set; }
    }
}
