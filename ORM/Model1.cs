using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ORM
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Auteur> Auteurs { get; set; }
        public virtual DbSet<Livre> Livres { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auteur>()
                .HasMany(e => e.Livres)
                .WithRequired(e => e.Auteur)
                .WillCascadeOnDelete(false);
        }
    }
}
