using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;

namespace BibleVerses.Models
{
    public class MyContext : DbContext
    {
        public MyContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<BibleVerse> BibleVerses { get; set; }
        public DbSet<BiblePlace> BiblePlaces { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Primary key
            //modelBuilder.Entity<BibleVerse>().HasKey(c => c.ID);
            //modelBuilder.Entity<BiblePlace>().HasKey(c => c.ID);

            //// Identity
            //modelBuilder.Entity<BibleVerse>().Property(c => c.ID)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //modelBuilder.Entity<BiblePlace>().Property(c => c.ID)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //modelBuilder.Entity<BibleVerse>()
            //    .HasOptional(a => a.Place)
            //    .WithRequired(a => a.BibleVerse);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }

    //public class SqlMigrator : 8SqlServerMigrationSqlGenerator
    //{
    //    protected override MigrationStatement Generate(AddForeignKeyOperation addForeignKeyOperation)
    //    {
    //        addForeignKeyOperation.PrincipalTable = addForeignKeyOperation.PrincipalTable.Replace("dbo.", "");
    //        addForeignKeyOperation.DependentTable = addForeignKeyOperation.DependentTable.Replace("dbo.", "");
    //        MigrationStatement ms = base.Generate(addForeignKeyOperation);
    //        return ms;
    //    }
    //    protected override void Generate(DropForeignKeyOperation dropForeignKeyOperation)
    //    {
    //        dropForeignKeyOperation.Name = StripDbo(dropForeignKeyOperation.Name);
    //        base.Generate(dropForeignKeyOperation);
    //    }

    //    protected override void Generate(DropIndexOperation dropIndexOperation)
    //    {
    //        dropIndexOperation.Name = StripDbo(dropIndexOperation.Name);
    //        base.Generate(dropIndexOperation);
    //    }

    //    protected override void Generate(DropPrimaryKeyOperation dropPrimaryKeyOperation)
    //    {
    //        dropPrimaryKeyOperation.Name = StripDbo(dropPrimaryKeyOperation.Name);
    //        base.Generate(dropPrimaryKeyOperation);
    //    }

    //    private string StripDbo(string name)
    //    {
    //        return name.Replace("dbo.", "");
    //    }
    //}
}
