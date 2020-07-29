@model Stankins.Interfaces.IDataToSent
@{

    var dt= Model.DataToBeSentFurther[0];
    var nrRows=dt.Rows.Count;
    var nrColumns = dt.Columns.Count;
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

            

            OnModelCreatingPartial(modelBuilder);
            Seed(modelBuilder);
        }
        void Seed(ModelBuilder modelBuilder){
            @for(var iRow=0;iRow<nrRows;iRow++){
                string text="";
                for(var iCol=0;iCol<nrColumns;iCol++){
                    var column=dt.Columns[iCol];
                    string nameColumn = column.ColumnName;
                    switch(column.DataType.Name.ToLower()){
                        case "string":
                            text+=", "+  nameColumn +" = " + "\"" + dt.Rows[iRow][iCol] + "\"" ;
                            break;
                        case "int32":
                            text+=", "+  nameColumn +" = " +  dt.Rows[iRow][iCol]  ;
                            break;
                        
                        default:
                            text+=", "+ column.DataType.Name +"???"+ nameColumn +" = "+ dt.Rows[iRow][iCol];  
                            break;  
                    };
                    
                }
                <text>
                modelBuilder.Entity<@(dt.TableName)>().HasData(
                    new @(dt.TableName)(){ ID = @(iRow+1) @Raw(text) });
                </text>
            }

            OnSeed(modelBuilder);


        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        partial void OnSeed(ModelBuilder modelBuilder);       
    }
}