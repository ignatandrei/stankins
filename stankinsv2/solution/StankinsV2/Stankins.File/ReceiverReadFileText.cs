using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.File
{
    public class ReceiverReadFileText : BaseObject, IReceive
    {
        private readonly string fileName;

        public ReceiverReadFileText(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverReadFileText);
            this.fileName = GetMyDataOrThrow<string>(nameof(fileName));


        }

        public ReceiverReadFileText(string fileName) : this(new CtorDictionary()
        {
            {nameof(fileName), fileName},


        })
        {

        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
                receiveData = new DataToSentTable();

            var file = new ReadFileToString
            {
                FileEnconding = Encoding.UTF8,
                FileToRead = fileName
            };
            var data = await file.LoadData();
            var dt = new DataTable
            {
                TableName = "FileContents"
            };
            dt.Columns.Add(new DataColumn($"FilePath", typeof(string)));
            dt.Columns.Add(new DataColumn($"FileContents", typeof(string)));
            dt.Rows.Add(fileName, data);
            FastAddTable(receiveData, dt);
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
    
