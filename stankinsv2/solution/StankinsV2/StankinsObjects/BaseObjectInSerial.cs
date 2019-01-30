using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StankinsObjects 
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


    public class BaseObjectInSerial<T1, T2, T3> : BaseObject, ITransformer
        where T1 : BaseObject
        where T2 : BaseObject
        where T3:BaseObject
    {
        public BaseObjectInSerial(CtorDictionary dataNeeded) : base(dataNeeded)
        {

        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
           
            var f1 = Activator.CreateInstance(typeof(T1), dataNeeded) as BaseObject;
            var f2 = Activator.CreateInstance(typeof(T2), dataNeeded) as BaseObject;
            var f3 = Activator.CreateInstance(typeof(T3), dataNeeded) as BaseObject;
            var data = await f1.TransformData(receiveData);
            data = await f2.TransformData(data);
            data = await f3.TransformData(data);
            return data;

        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }

    public class BaseObjectInSerial<T1,T2> : BaseObject, ITransformer
        where T1: BaseObject
        where T2 : BaseObject
    {
        public BaseObjectInSerial(CtorDictionary dataNeeded):base(dataNeeded)
        {
            
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            //TODO : remove from release builds
            var v = new Verifier();
            var first = Activator.CreateInstance(typeof(T1), dataNeeded) as BaseObject;
            var second = Activator.CreateInstance(typeof(T2), dataNeeded) as BaseObject;

            var data = await first.TransformData(receiveData);
            //TODO : remove from release builds
            await v.TransformData(data);
            data = await second.TransformData(data);
            //TODO : remove from release builds
            await v.TransformData(data);
            return data;
            
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
