using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReceiverDatabaseObjects
{
    public abstract class ReceiverRelational:IReceive
    {
        public string ConnectionString{ get; set; }
        public string DatabaseName { get; set; }
        
        public string Name { get; set; }

        public IRowReceive[] valuesRead { get; set; }

        public ReceiverRelational()
        {
            Name = "get details from relational server";
        }
        protected abstract Task<KeyValuePair<string, object>[]> GetServerDetails();
        protected abstract Task<KeyValuePair<string,string>[]> GetDatabases();
        protected abstract Task<KeyValuePair<string, string>[]> GetTables(KeyValuePair<string,string> database);
        protected abstract Task<KeyValuePair<string, string>[]> GetColumns(KeyValuePair<string, string> table);
        public async Task LoadData()
        {
            var rr = new Dictionary<string, RowReadRelation>();
            var rrhServer= new RowReadRelation();
            foreach (var item in await GetServerDetails())
            {
                rrhServer.Values.Add(item.Key,item.Value);
            }
            

            var dbsList = new List<IRowReceiveRelation>();
            rrhServer.Relations.Add("databases", dbsList);
            
            var dbs = await GetDatabases();
            bool allDatabases= string.IsNullOrWhiteSpace(DatabaseName);
            foreach(var db in dbs)
            {
                if (!allDatabases)
                {
                    bool ThisDatabase = string.Compare(db.Value, DatabaseName, StringComparison.OrdinalIgnoreCase) == 0;
                    if (!ThisDatabase)
                        continue;
                }
                var rrDatabase = new RowReadRelation();
                rrDatabase.Values.Add("ID", db.Key);
                rrDatabase.Values.Add("Name", db.Value);
                rr.Add(db.Key, rrDatabase);
                dbsList.Add(rrDatabase);
                var dbsTables= new List<IRowReceiveRelation>();
                rrDatabase.Relations.Add("tables", dbsTables);
                
                var tables=await GetTables(db);
                foreach(var table in tables)
                {
                    var rrTable = new RowReadRelation();
                    dbsTables.Add(rrTable);
                    rrTable.Values.Add("ID", table.Key);
                    rrTable.Values.Add("Name", table.Value);

                    rr.Add(table.Key, rrTable);
                    


                    var dbsColumns= new List<IRowReceiveRelation>();
                    rrTable.Relations.Add("columns", dbsColumns);
                    var cols = await GetColumns(table);
                    foreach (var item in cols)
                    {
                        var rrCol= new RowReadRelation();
                        rrCol.Values.Add("ID", table.Key);
                        rrCol.Values.Add("Name", table.Value);
                        dbsColumns.Add(rrCol);
                    }

                }

                //TODO: add relationships
                this.valuesRead = new IRowReceive[] { rrhServer };
            }
        }

        
    }
}
