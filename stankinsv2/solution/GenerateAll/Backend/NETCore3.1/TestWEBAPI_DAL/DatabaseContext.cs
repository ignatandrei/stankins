@model Stankins.Interfaces.IDataToSent
@{

    var ds= Model.FindAfterName("DataSource").Value;
    
    var nrRowsDS=ds.Rows.Count;
    
    var nameTablesToRender = new string[nrRowsDS];
    var tables=new System.Data.DataTable[nrRowsDS];
    for (int iRowDS = 0; iRowDS < nrRowsDS; iRowDS++)
    {
        var nameTable = ds.Rows[iRowDS]["TableName"].ToString();
        var renderTable = Model.FindAfterName(nameTable).Value;
        nameTablesToRender[iRowDS] = nameTable;
        tables[iRowDS]=renderTable;
    }

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
        @foreach(var nameTable in nameTablesToRender){
            <text>
            public virtual DbSet<@nameTable> @nameTable { get; set; }
            </text>
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        @foreach(var nameTable in nameTablesToRender){
         <text>
            modelBuilder.Entity<@nameTable>(entity =>
            {
                //entity.Property(e => e.Name).IsUnicode(false);
            });
         </text>
        }            

            OnModelCreatingPartial(modelBuilder);
            Seed(modelBuilder);
        }
        void Seed(ModelBuilder modelBuilder){

            @foreach(var dt in tables){
                var nrRows =dt.Rows.Count; 
                var nrColumns = dt.Columns.Count;
                for(var iRow=0;iRow<nrRows;iRow++){
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
            }

            OnSeed(modelBuilder);


        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        partial void OnSeed(ModelBuilder modelBuilder);       
    }
}