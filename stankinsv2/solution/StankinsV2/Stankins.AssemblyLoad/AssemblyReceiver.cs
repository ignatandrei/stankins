using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stankins.AssemblyLoad
{
    public class AssemblyReceiver : BaseObject, IReceive
    {
        public AssemblyReceiver(CtorDictionary dataNeeded):base(dataNeeded)
        {
        }

       

        
      

        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            throw new NotImplementedException();
        }

        

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
