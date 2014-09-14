using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace AutoSaveEntityGrid
{
    public partial class MainForm : Form
    {
        private RadGridView gridView = new RadGridView();
        private UserContext userContext = new UserContext();
      
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            userContext.Users.Load();

            gridView.Dock = DockStyle.Fill;
            gridView.Parent = this;
            gridView.UserAddedRow += gridView_UserAddedRow;
            gridView.UserDeletedRow += gridView_UserDeletedRow;
            gridView.CellValueChanged += gridView_CellValueChanged;
            gridView.DataSource = userContext.Users.Local.ToBindingList(); 
            gridView.BestFitColumns();
        }

        void gridView_UserAddedRow(object sender, GridViewRowEventArgs e)
        {
            userContext.SaveChanges();
        }

        void gridView_UserDeletedRow(object sender, GridViewRowEventArgs e)
        {
            userContext.SaveChanges();
        }

        void gridView_CellValueChanged(object sender, GridViewCellEventArgs e)
        {
            userContext.SaveChanges();
        }
    }

    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
    }
}
