using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SQLite.CodeFirst;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildPlaygroundTracker
{

    public class Database : DbContext
    {
        public Database() : base("PlaygroundDB") { }

        public DbSet<Parent> Parents { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Visit> Visits { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Child>()
       .HasRequired(c => c.Parent)
       .WithMany()
       .HasForeignKey(c => c.ParentId);
            modelBuilder.Entity<Visit>()
        .HasRequired(v => v.Child)
        .WithMany()
        .HasForeignKey(v => v.ChildId);

            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<Database>(modelBuilder);
            System.Data.Entity.Database.SetInitializer(sqliteConnectionInitializer);
        }
    }

    public class Parent
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
    }

    public class Child
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public int ParentId { get; set; }
        public virtual Parent Parent { get; set; }

        [NotMapped] // Это свойство не будет сохраняться в БД
        public string FullName => $"{LastName} {FirstName} {Patronymic}";
    }

    public class Visit
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int ChildId { get; set; }

        public virtual Child Child { get; set; }

        public string Duration =>
            EndTime.HasValue
            ? (EndTime.Value - StartTime).ToString(@"hh\:mm\:ss")
            : (DateTime.Now - StartTime).ToString(@"hh\:mm\:ss");
    }

}
