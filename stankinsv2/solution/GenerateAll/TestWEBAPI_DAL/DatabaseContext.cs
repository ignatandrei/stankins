@model Stankins.Interfaces.IDataToSent
@{

    var dt= Model.DataToBeSentFurther[0];
}
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TestWebAPI_BL;

namespace TestWEBAPI_DAL
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {

        }
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {
        }

        public virtual DbSet<@dt.TableName> @dt.TableName { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<@dt.TableName>(entity =>
            {
                //entity.Property(e => e.Name).IsUnicode(false);
            });
            
            modelBuilder.Entity<Post>().HasData(
                new { BlogId = 1, PostId = 2, Title = "Second post", Content = "Test 2" });


            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}