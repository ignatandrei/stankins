using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Threading.Tasks;

namespace Stankins.Alive
{
    public class ReceiverPing : AliveStatus, IReceive
    {
        public ReceiverPing(CtorDictionary dataNeeded) : base(dataNeeded)
        {
        }

        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            throw new NotImplementedException();
        }
    }
}
