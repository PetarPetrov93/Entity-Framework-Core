using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using P01_StudentSystem.Data.Common;
using P01_StudentSystem.Data.Models;
using System;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        //Used when developing the app
        //Also used when testing the app locally on our pc
        public StudentSystemContext()
        {
            
        }

        //Used for Judge
        //Also usefull in real application. Loading of the DvContext with Dependancy Injection
        public StudentSystemContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Homework> Homeworks { get; set; } = null!;
        public DbSet<Resource> Resources { get; set; } = null!;
        public DbSet<StudentCourse> StudentsCourses { get; set; } = null!;


        //Connection configuration
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //Setting a default connection string
                //When someone has used empty constructor of our DbContext
                optionsBuilder.UseSqlServer(DbConfig.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        //Fluent API(setting the relations and configurations of the model classes)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(sc => new { sc.StudentId, sc.CourseId });
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                   entity
                    .Property(r => r.Url)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity
                .Property(s => s.PhoneNumber)
                .IsUnicode(false)
                .IsFixedLength(true)
                .HasMaxLength(10);
            });
        }
    }
}
