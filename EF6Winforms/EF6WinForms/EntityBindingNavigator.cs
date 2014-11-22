using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EFWinforms
{
    /// <summary>
    /// DataSource component that exposes EntityFramework ObjectSet and provides navigation,
    /// add/remove, and save/cancel changes buttons.
    /// </summary>
    /// <remarks>
    /// This class contains a <see cref="EntityDataSource"/> and exposes one of the
    /// entity sets available for binding.
    /// </remarks>r
    [
    ToolboxItem(true),
    ToolboxBitmap(typeof(EntityBindingNavigator), "EntityBindingNavigator.png"),
    DefaultProperty("DataSource"),
    ComplexBindingProperties("DataSource", "DataMember")
    ]
    public partial class EntityBindingNavigator :
        ToolStrip
    {
        //-------------------------------------------------------------
        #region ** fields

        object _dataSource;
        string _dataMember = string.Empty;
        CurrencyManager _cm;
        bool _showBtnNav, _showBtnAdd, _showBtnSave;


        #endregion

        //-------------------------------------------------------------
        #region ** ctor

        /// <summary>
        /// Initializes a new instance of a <see cref="EntityBindingNavigator"/>.
        /// </summary>
        public EntityBindingNavigator()
        {
            InitializeComponent();
            Dock = DockStyle.Top;
            ShowNavigationButtons = ShowAddRemoveButtons = ShowSaveUndoRefreshButtons = true;
            UpdateUI();
        }

        #endregion

        //-------------------------------------------------------------
        #region ** object model

        /// <summary>
        /// Gets or sets the data source for this navigator.
        /// </summary>
        [
        DefaultValue(null),
        AttributeProvider(typeof(IListSource))
        ]
        public object DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                UpdateCurrencyManager();
            }
        }
        /// <summary>
        /// Gets or sets the specific list in a <see cref="DataSource"/> object that the navigator should display.
        /// </summary>
        [
        DefaultValue(""),
        Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string DataMember
        {
            get { return _dataMember; }
            set
            {
                _dataMember = value;
                UpdateCurrencyManager();
            }
        }
        /// <summary>
        /// Sets the DataSource and DataMember properties at the same time.
        /// </summary>
        public void SetDataBinding(object dataSource, string dataMember)
        {
            _dataSource = dataSource;
            _dataMember = dataMember;
            UpdateCurrencyManager();
        }
        /// <summary>
        /// Gets a reference to the list being managed by this navigator.
        /// </summary>
        [
        Browsable(false)
        ]
        public IBindingList List
        {
            get { return _cm != null ? _cm.List as IBindingList : null; }
        }
        /// <summary>
        /// Gets the item that is currently selected.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public object CurrentItem
        {
            get { return _cm != null ? _cm.Current : null; }
        }
        /// <summary>
        /// Gets or sets the index of the item that is currently selected.
        /// </summary>
        [
        Browsable(false), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int Position
        {
            get { return _cm != null ? _cm.Position : -1; }
            set { if (_cm != null) _cm.Position = value; }
        }
        /// <summary>
        /// Gets or sets whether the control should display the navigation buttons.
        /// </summary>
        [
        DefaultValue(true)
        ]
        public bool ShowNavigationButtons
        {
            get { return _showBtnNav; }
            set
            {
                _showBtnNav = 
                    _btnFirst.Visible = 
                    _btnPrev.Visible = 
                    _btnNext.Visible = 
                    _lblCurrent.Visible =
                    _btnLast.Visible = 
                    _sepNav.Visible = value;
            }
        }
        /// <summary>
        /// Gets or sets whether the control should display the add/remove item buttons.
        /// </summary>
        [
        DefaultValue(true)
        ]
        public bool ShowAddRemoveButtons
        {
            get { return _showBtnAdd; }
            set
            {
                _showBtnAdd = 
                    _btnAdd.Visible = 
                    _btnRemove.Visible = 
                    _sepAddRemove.Visible = value;
            }
        }
        /// <summary>
        /// Gets or sets whether the control should display the save/undo/refresh buttons.
        /// </summary>
        [
        DefaultValue(true)
        ]
        public bool ShowSaveUndoRefreshButtons
        {
            get { return _showBtnSave; }
            set
            {
                _showBtnSave = 
                    _btnSave.Visible =
                    _btnUndo.Visible =
                    _btnRefresh.Visible = value;
            }
        }

        #endregion

        //-------------------------------------------------------------
        #region ** events

        /// <summary>
        /// Occurs when the current item changes.
        /// </summary>
        public event EventHandler PositionChanged;
        /// <summary>
        /// Raises the <see cref="PositionChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPositionChanged(EventArgs e)
        {
            if (PositionChanged != null)
                PositionChanged(this, e);
        }
        /// <summary>
        /// Occurs when the current item changes.
        /// </summary>
        public event ListChangedEventHandler ListChanged;
        /// <summary>
        /// Raises the <see cref="ListChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            if (ListChanged != null)
                ListChanged(this, e);
        }
        /// <summary>
        /// Occurs before a new item is added to the list.
        /// </summary>
        public event CancelEventHandler AddingNew;
        /// <summary>
        /// Raises the <see cref="AddingNew"/> event.
        /// </summary>
        /// <param name="e"><see cref="CancelEventArgs"/> that contains the event parameters.</param>
        protected virtual void OnAddingNew(CancelEventArgs e)
        {
            if (AddingNew != null)
                AddingNew(this, e);
        }
        /// <summary>
        /// Occurs after a new item is added to the list.
        /// </summary>
        public event EventHandler AddedNew;
        /// <summary>
        /// Raises the <see cref="AddedNew"/> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> that contains the event parameters.</param>
        protected virtual void OnAddedNew(EventArgs e)
        {
            if (AddedNew != null)
                AddedNew(this, e);
        }
        /// <summary>
        /// Occurs before an item is removed from the list.
        /// </summary>
        public event CancelEventHandler RemovingItem;
        /// <summary>
        /// Raises the <see cref="RemovingItem"/> event.
        /// </summary>
        /// <param name="e"><see cref="CancelEventArgs"/> that contains the event parameters.</param>
        protected virtual void OnRemovingItem(CancelEventArgs e)
        {
            if (RemovingItem != null)
                RemovingItem(this, e);
        }
        /// <summary>
        /// Occurs after an item is removed from the list.
        /// </summary>
        public event EventHandler RemovedItem;
        /// <summary>
        /// Raises the <see cref="RemovedItem"/> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> that contains the event parameters.</param>
        protected virtual void OnRemovedItem(EventArgs e)
        {
            if (RemovedItem != null)
                RemovedItem(this, e);
        }

        #endregion

        //-------------------------------------------------------------
        #region ** overrides

        /// <summary>
        /// Update the internal CurrencyManager when the BindingContext changes.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBindingContextChanged(EventArgs e)
        {
            base.OnBindingContextChanged(e);
            UpdateCurrencyManager();
        }
        /// <summary>
        /// Gets or sets how the control is docked in the parent container.
        /// </summary>
        [DefaultValue(DockStyle.Top)]
        public override DockStyle Dock
        {
            get { return base.Dock; }
            set { base.Dock = value; }
        }

        #endregion

        //-------------------------------------------------------------
        #region ** implementation

        ToolStripButton _btnFirst, _btnPrev, _btnNext, _btnLast;
        ToolStripLabel _lblCurrent;
        ToolStripButton _btnSave, _btnUndo, _btnRefresh;
        ToolStripButton _btnAdd, _btnRemove;
        ToolStripSeparator _sepNav, _sepAddRemove;

        void InitializeComponent()
        {
            _btnFirst = new ToolStripButton("First Record", Properties.Resources.FirstRecord_small, _btnFirst_Click);
            _btnPrev = new ToolStripButton("Previous Record", Properties.Resources.PreviousRecord_small, _btnPrev_Click);
            _lblCurrent = new ToolStripLabel("0 of 0");
            _btnNext = new ToolStripButton("Next Record", Properties.Resources.NextRecord_small, _btnNext_Click);
            _btnLast = new ToolStripButton("Last Record", Properties.Resources.LastRecord_small, _btnLast_Click);
            foreach (var btn in new ToolStripButton[] { _btnFirst, _btnPrev, _btnNext, _btnLast })
            {
                btn.DisplayStyle = ToolStripItemDisplayStyle.Image;
                Items.Add(btn);
            }
            Items.Insert(2, _lblCurrent);

            _sepNav = new ToolStripSeparator();
            Items.Add(_sepNav);

            _btnAdd = new ToolStripButton("Add Item", Properties.Resources.NewItem_small, _btnAdd_Click);
            _btnRemove = new ToolStripButton("Remove Item", Properties.Resources.Cancel2_small, _btnRemove_Click);
            foreach (var btn in new ToolStripButton[] { _btnAdd, _btnRemove })
            {
                btn.DisplayStyle = ToolStripItemDisplayStyle.Image;
                Items.Add(btn);
            }
            _sepAddRemove = new ToolStripSeparator();
            Items.Add(_sepAddRemove);

            _btnSave = new ToolStripButton("Save", Properties.Resources.Save_small, _btnSave_Click);
            _btnUndo = new ToolStripButton("Undo", Properties.Resources.Undo_small, _btnCancel_Click);
            _btnRefresh = new ToolStripButton("Refresh", Properties.Resources.Refresh_small, _btnRefresh_Click);
            foreach (var btn in new ToolStripButton[] { _btnRefresh, _btnUndo, _btnSave })
            {
                btn.Alignment = ToolStripItemAlignment.Right;
                Items.Add(btn);
            }

            foreach (ToolStripItem item in Items)
            {
                item.Visible = true;
            }
        }

        // update UI when postion/list changes
        void _cm_PositionChanged(object sender, EventArgs e)
        {
            UpdateUI();
            OnPositionChanged(e);
        }
        void _cm_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType != ListChangedType.ItemChanged)
            {
                UpdateUI();
            }
            OnListChanged(e);
        }
        void UpdateUI()
        {
            if (_cm != null)
            {
                _lblCurrent.Text = string.Format("{0} of {1}", _cm.Position + 1, _cm.Count);
                _btnFirst.Enabled = _btnPrev.Enabled = _cm.Position > 0;
                _btnLast.Enabled = _btnNext.Enabled = _cm.Position < _cm.Count - 1;

                var bl = _cm.List as IBindingList;
                _btnAdd.Enabled = bl != null && bl.AllowNew;
                _btnRemove.Enabled = bl != null && bl.AllowRemove;
            }
            else
            {
                _btnFirst.Enabled = _btnPrev.Enabled = false;
                _btnLast.Enabled = _btnNext.Enabled = false;
                _btnAdd.Enabled = _btnRemove.Enabled = false;
            }

            _btnSave.Enabled = _btnUndo.Enabled = _btnRefresh.Enabled = EntityDataSource != null;
        }

        // navigation
        void _btnFirst_Click(object sender, EventArgs e)
        {
            if (_cm != null)
            {
                _cm.Position = 0;
            }
        }
        void _btnPrev_Click(object sender, EventArgs e)
        {
            if (_cm != null && _cm.Position > 0)
            {
                _cm.Position--;
            }
        }
        void _btnNext_Click(object sender, EventArgs e)
        {
            if (_cm != null && _cm.Position < _cm.Count - 1)
            {
                _cm.Position++;
            }
        }
        void _btnLast_Click(object sender, EventArgs e)
        {
            if (_cm != null)
            {
                _cm.Position = _cm.Count - 1;
            }
        }

        // add/remove records
        void _btnAdd_Click(object sender, EventArgs e)
        {
            // notify
            var ce = new CancelEventArgs();
            OnAddingNew(ce);

            if (_cm != null && _cm.List is IBindingList && !ce.Cancel)
            {
                // add new
                var bl = _cm.List as IBindingList;
                var newItem = bl.AddNew();
             
                // notify
                OnAddedNew(e);

                // make sure new item is selected
                _cm.Position = bl.IndexOf(newItem);
            }

        }
        void _btnRemove_Click(object sender, EventArgs e)
        {
            // notify
            var ce = new CancelEventArgs();
            OnRemovingItem(ce);

            if (_cm != null && _cm.Current != null && _cm.List is IBindingList && !ce.Cancel)
            {
                // remove current item
                var bl = _cm.List as IBindingList;
                bl.Remove(_cm.Current);

                // notify
                OnRemovedItem(e);
            }
        }

        // save/cancel/refresh
        void _btnSave_Click(object sender, EventArgs e)
        {
            if (EntityDataSource != null)
            {
                EntityDataSource.SaveChanges();
            }
        }
        void _btnCancel_Click(object sender, EventArgs e)
        {
            if (EntityDataSource != null)
            {
                EntityDataSource.CancelChanges();
            }
        }
        void _btnRefresh_Click(object sender, EventArgs e)
        {
            if (EntityDataSource != null)
            {
                EntityDataSource.Refresh();
            }
        }

        // update the currency manager to handle a new DataSource, DataMember, or BindingContext
        void UpdateCurrencyManager()
        {
            // disconnect old
            if (_cm != null)
            {
                _cm.PositionChanged -= _cm_PositionChanged;
                _cm.ListChanged -= _cm_ListChanged;
            }

            // get new currency manager
            _cm = null;
            if (DataSource != null && Parent != null && BindingContext != null)
            {
                try
                {
                    _cm = BindingContext[DataSource, DataMember] as CurrencyManager;
                }
                catch { }
            }

            // connect new
            if (_cm != null)
            {
                _cm.PositionChanged += _cm_PositionChanged;
                _cm.ListChanged += _cm_ListChanged;
            }

            // show UI
            UpdateUI();
        }
        EntityDataSource EntityDataSource
        {
            get { return _dataSource as EntityDataSource; }
        }

        #endregion
    }
}
