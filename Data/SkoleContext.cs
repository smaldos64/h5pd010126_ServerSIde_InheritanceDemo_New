using InheritanceDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace InheritanceDemo.Data
{
    public class SkoleContext : DbContext
    {
        public SkoleContext(DbContextOptions<SkoleContext> options) : base(options) { }

        public DbSet<Person> Personer { get; set; }
        public DbSet<Student> Studerende { get; set; }
        public DbSet<Ansat> Ansatte { get; set; }
        public DbSet<Hold> Hold { get; set; }
        public DbSet<Fag> Fag { get; set; }
        public DbSet<Afdeling> Afdelinger { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<EUXStudent> EUXStuderende { get; set; }
        public DbSet<EUDStudent> EUDStuderende { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // --- MULIGHED 1: Table Per Hierarchy (TPH) - Standard ---
            // Én stor tabel "Personer" med en 'Discriminator' kolonne.
            //modelBuilder.Entity<Person>()
            //    .HasDiscriminator<string>("PersonType")
            //    .HasValue<Student>("Student")
            //    .HasValue<Ansat>("Ansat");

            // --- MULIGHED 2: Table Per Type (TPT) ---
            // En tabel til Person, en til Student og en til Ansat. De deler Primary Key.
            modelBuilder.Entity<Person>().ToTable("Personer");
            modelBuilder.Entity<Student>().ToTable("Studerende");
            modelBuilder.Entity<Ansat>().ToTable("Ansatte");
            modelBuilder.Entity<Teacher>().ToTable("Teachers");
            modelBuilder.Entity<EUXStudent>().ToTable("EUXStuderende");
            modelBuilder.Entity<EUDStudent>().ToTable("EUDStuderende");


            // --- MULIGHED 3: Table Per Concrete Type (TPC) ---
            // Ingen "Personer" tabel. Kun "Studerende" og "Ansatte" tabeller med alle felter.
            //modelBuilder.Entity<Student>().UseTpcMappingStrategy();
            //modelBuilder.Entity<Ansat>().UseTpcMappingStrategy();


            // Mange-til-mange (Student <-> Fag) konfigureres ofte automatisk, 
            // men kan ekspliciteres her:
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Fag)
                .WithMany(f => f.Studerende);

            modelBuilder.Entity<Ansat>()
                .Property(a => a.MaanedsLoen).HasPrecision(18, 2);

            // Vi ønsker IKKE Cascade delete på nogen af vores en-til-mange relationer
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Hold)
                .WithMany(h => h.Studerende)
                .HasForeignKey(s => s.HoldId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ansat>()
                .HasOne(s => s.Afdeling)
                .WithMany(a => a.Ansatte)
                .HasForeignKey(s => s.AfdelingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation 1: Teacher <-> Fag
            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Fag)
                .WithMany(f => f.Teachers) // Husk at tilføje List<Teacher> i Fag-klassen
                .UsingEntity(j => j.ToTable("TeacherFag"));

            // Relation 2: Teacher <-> Afdeling
            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Afdelinger)
                .WithMany(a => a.Teachers) // Husk at tilføje List<Teacher> i Afdeling-klassen
                .UsingEntity(j => j.ToTable("TeacherAfdeling"));
        }
    }
}
