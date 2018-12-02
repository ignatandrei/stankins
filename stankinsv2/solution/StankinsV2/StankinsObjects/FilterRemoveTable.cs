﻿using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public class FilterRemoveTable : BaseObject, IFilter
    {
        public FilterRemoveTable(string nameTable) : this(new CtorDictionary() {
            { nameof(nameTable), nameTable }
            }
            )
        {

        }
        public FilterRemoveTable(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.NameTable = base.GetMyDataOrThrow<string>(nameof(NameTable));
        }

        public string NameTable { get; }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var id = receiveData.FindAfterName(NameTable);
            receiveData.DataToBeSentFurther.Remove(id.Key);
            
            receiveData.Metadata.Tables.RemoveAt(id.Key);
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}