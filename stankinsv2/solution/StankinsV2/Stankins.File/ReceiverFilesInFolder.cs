using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.File
{
    public class ReceiverFilesInFolder : BaseObject
    {
        public ReceiverFilesInFolder(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverCSVFile);
            PathFolder = GetMyDataOrThrow<string>(nameof(PathFolder));
            Filter = GetMyDataOrDefault<string>(nameof(Filter), "*.*");


        }
        public ReceiverFilesInFolder(string pathFolder, string filter) : this(new CtorDictionary()
            {
                {nameof(pathFolder),pathFolder},
                {nameof(filter),filter},

            })
        {
            Filter = filter;
        }

        public string PathFolder { get; }
        public string Filter { get; }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var ret = new DataToSentTable();
            var dt = new DataTable
            {
                TableName = Path.GetDirectoryName(PathFolder)
            };
            dt.Columns.Add(new DataColumn("FileName", typeof(string)));
            dt.Columns.Add(new DataColumn("FullFileName", typeof(string)));
            foreach (var item in Directory.GetFiles(PathFolder, Filter, SearchOption.TopDirectoryOnly))
            {
                dt.Rows.Add(new[] { Path.GetFileName(item), item });
            }
            var id = ret.AddNewTable(dt);
            ret.Metadata.AddTable(dt, id);
            return await Task.FromResult(ret);

        }


        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
