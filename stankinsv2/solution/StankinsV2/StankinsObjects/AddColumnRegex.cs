using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StankinsObjects 
{
    public class AddColumnRegex : BaseObject, ITransformer
    {
        public AddColumnRegex(string columnName,string expression) : this(new CtorDictionary()
        {
            { nameof(columnName), columnName},
            {nameof(expression),expression }
        })
        {
            
        }
        public AddColumnRegex(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            ColumnName = GetMyDataOrThrow<string>(nameof( ColumnName));
            Expression = GetMyDataOrThrow<string>(nameof(Expression));
        }

        public string ColumnName { get; }
        public string Expression { get; }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var regex = new Regex(Expression);
            var names = regex.
                GetGroupNames().
                Where(it=>!int.TryParse(it,out var _)).
                ToArray();
            var tables = base.FindTableAfterColumnName(ColumnName, receiveData);
            foreach (var table in tables)
            {
                foreach (var n in names)
                {
                    table.Value.Columns.Add(new DataColumn(n, typeof(string)));
                    receiveData.Metadata.Columns.Add(new Column() { Id = receiveData.Metadata.Columns.Count, IDTable = table.Key, Name = n });
                }
            }
            foreach (var table in tables)
            {
                var t = table.Value;
                foreach (DataRow item in t.Rows)
                {
                    var data = item[ColumnName]?.ToString();
                    if (string.IsNullOrWhiteSpace(data))
                        continue;
                    var g = regex.Match(data);
                    if (!g.Success)
                    {
                        //TODO: log - or write in an error metadata table
                        continue;
                    }
                    var groups = g.Groups;


                    foreach (var n in names)
                    {
                        Group gr;
                        try
                        {
                            gr = groups[n];
                        }
                        catch
                        {
                            //TODO: log exception or put in exception table
                            continue;
                        }

                        if (gr.Success)
                        {
                            string val = gr.Value;
                            try
                            {
                                item[n] = val;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                if (item[n]?.ToString() == val)
                                    continue;
                            }
                        }

                    }
                }
            }
            return await Task.FromResult(receiveData);
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
