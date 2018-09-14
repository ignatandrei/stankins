using StankinsCommon;
using StankinsV2Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StankinsV2Objects
{
    public class BaseObjectInSerial : BaseObject, ITransformer
    {
        public List<string> Types { get; set; }
        public BaseObjectInSerial(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            Types = new List<string>();
        }
        public void AddType(string type)
        {
            Types.Add(type);
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var data = receiveData;
            foreach(var type in Types)
            {
                var constructType = Activator.CreateInstance(Type.GetType(type), dataNeeded) as BaseObject;
                data = await constructType.TransformData(data);

            }
            return data;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
    public class BaseObjectInSerial<T,U> : BaseObject, ITransformer
        where T: BaseObject
        where U : BaseObject
    {
        public BaseObjectInSerial(CtorDictionary dataNeeded):base(dataNeeded)
        {
            
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var first = Activator.CreateInstance(typeof(T), dataNeeded) as BaseObject;
            var second = Activator.CreateInstance(typeof(U), dataNeeded) as BaseObject;

            var data = await first.TransformData(receiveData);
            data = await second.TransformData(data);
            return data;
            
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
