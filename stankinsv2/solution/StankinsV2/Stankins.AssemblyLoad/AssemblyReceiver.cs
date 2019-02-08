using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace Stankins.AssemblyLoad
{
    public class AssemblyReceiver : BaseObject, IReceive
    {
        private readonly string fullAssemblyName;

        public AssemblyReceiver(CtorDictionary dataNeeded):base(dataNeeded)
        {
            this.fullAssemblyName = GetMyDataOrThrow<string>(nameof(fullAssemblyName));
        }
        public AssemblyReceiver(string fullAssemblyName) : this(new CtorDictionary()
        {
            {nameof(fullAssemblyName),fullAssemblyName }
        }
            )
        {

        }


        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if(receiveData == null)
            {
                receiveData = new DataToSentTable();
            }
            var ass = Assembly.Load(fullAssemblyName);
            var dt = new DataTable("types");
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            foreach (var item in ass.GetTypes())
            {
                dt.Rows.Add(item.FullName, item.Name);
            }
            FastAddTable(receiveData, dt);
            return await Task.FromResult(receiveData);

        }

        

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
