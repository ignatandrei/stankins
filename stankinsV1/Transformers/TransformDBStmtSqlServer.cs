using System;
using System.Collections.Generic;
using System.Text;
using StankinsInterfaces;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.IO;
using System.Data;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace Transformers
{
    public class TransformDBStmtSqlServer
    {
        public TransformDBStmtSqlServer(string connectionString, CommandType commandType, string commandText, ExecutionMode transformerExecutionMode, string inputColumns, string outputColumns, string identifierColumn)
        {
            this.ConnectionString = connectionString;
            this.CommandType = commandType;
            this.CommandText = commandText;
            this.TransformerExecutionMode = transformerExecutionMode;
            this.InputColumns = inputColumns;
            this.OutputColumns = outputColumns;
            this.IdentifierColumn = identifierColumn;

            this.Name = $"executing SQL Server statement {this.CommandText}";
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }

        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }
        public ExecutionMode TransformerExecutionMode { get; set; }
        public string InputColumns { get; set; } //Column1;Column2
        public string OutputColumns { get; set; } //ColumnNew1;ColumnNew2
        public string IdentifierColumn { get; set; } //Single column

        protected string[] _inputColumns;
        protected string[] _outputColumns;

        protected void Init()
        {
            if(this.TransformerExecutionMode == ExecutionMode.RowByRow)
            {
                throw new NotImplementedException("ExecutionMode.RowByRow not implemented");
            }

            _inputColumns = this.InputColumns.Split(';');
            _outputColumns = this.OutputColumns.Split(';');
        }

        public async Task Run()
        {
            Init();

            valuesTransformed = valuesRead;

            if (this.TransformerExecutionMode == ExecutionMode.SingleCall)
            {
                //Generating and XML document with values of input columns
                XElement xmlInputValues = new XElement("root");
                foreach(var row in this.valuesRead)
                {
                    XElement xmlRowDocument = new XElement("row");
                    xmlInputValues.Add(xmlRowDocument);
                    xmlRowDocument.Add(new XAttribute(this.IdentifierColumn, row.Values[this.IdentifierColumn]));
                    foreach (var column in _inputColumns)
                    {
                        if (row.Values.ContainsKey(column) && row.Values[column] != null)
                        {
                            xmlRowDocument.Add(new XAttribute(column, row.Values[column]));
                        }
                    }
                }

                //Call SQL code using xmlInputValues as parameter
                //This call should return a DataReader with following columns: IdentifierColumn + OutputColumn
                using (var conn = new SqlConnection(this.ConnectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = this.CommandType;
                        cmd.CommandText = this.CommandText;
                        cmd.Parameters.AddWithValue("@xmlInputValues", xmlInputValues.ToString(SaveOptions.None));

                        var rdr = await cmd.ExecuteReaderAsync();

                        //Merge valuesTransformed and DataReader 
                        while (rdr.Read())
                        {
                            var idValueOut = rdr.GetValue(rdr.GetOrdinal(this.IdentifierColumn));

                            foreach(var row in valuesTransformed)
                            {
                                var idValueIn = row.Values[this.IdentifierColumn];
                                if (idValueIn.Equals(idValueOut))
                                {
                                    foreach(var column in _outputColumns)
                                    {
                                        row.Values[column] = rdr.GetValue(rdr.GetOrdinal(column));
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

            }
        }

        public enum ExecutionMode { RowByRow = 0, SingleCall = 1, BatchMode = 2 } // For BatchMode we have to add a new property: BatchSize
    }
}
