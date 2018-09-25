using HtmlAgilityPack;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stankins.HTML
{
    public class ReceiverHtmlRegex: Receiver
    {
        public string File { get; }
        public Encoding Encoding { get; }
        public string Expression { get; }

        public ReceiverHtmlRegex(string file, Encoding encoding, string expression) : this(new CtorDictionary()
        {
            { nameof(file), file},
            {nameof(expression),expression },
            {nameof(encoding),encoding}
        })
        {
            
        }
        public ReceiverHtmlRegex(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            File = GetMyDataOrThrow<string>(nameof(File));
            Expression = GetMyDataOrThrow<string>(nameof(Expression));
            Encoding = GetMyDataOrDefault<Encoding>(nameof(Encoding), Encoding.UTF8);
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var file = new ReadFileToString
            {
                FileEnconding = this.Encoding,
                FileToRead = this.File
            };

            var data = await file.LoadData();
            bool web = file.FileType == FileType.Web;

            var ret = new DataToSentTable();

            var dt = new DataTable
            {
                TableName = $"TableLinks"
            };

            var regex = new Regex(Expression);
            var names = regex.
                GetGroupNames().
                Where(it => !int.TryParse(it, out var _)).
                ToArray();

            foreach (var n in names)
            {
                dt.Columns.Add(new DataColumn(n, typeof(string)));                
            }
            RegexOptions options = RegexOptions.Multiline;
            MatchCollection matches = Regex.Matches(data, Expression, options);

            foreach (Match g in matches)
            {
                if (!g.Success)
                    continue;
                var item = new string[names.Length];
                var groups = g.Groups;
                int i = -1;
                foreach (var n in names)
                {
                    i++;
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

                        item[i] = gr.Value; 
                    }
                }
                dt.Rows.Add(item);

            }
            var id = ret.AddNewTable(dt);
            ret.Metadata.AddTable(dt, id);
            return ret;


        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
