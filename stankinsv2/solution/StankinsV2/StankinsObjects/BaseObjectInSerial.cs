using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace StankinsObjects 
{
    class BaseObjectData
    {
        public BaseObjectData(Type t, CtorDictionary data)
        {
            this.Type=t;
            if(data!= null)
            {
                this.dataNeeded=new CtorDictionary( data);
            }
            

        }
        public Type Type { get; set; }
        public CtorDictionary dataNeeded { get; set; }=null;
    }
    public class BaseObjectInSerial : BaseObject, ITransformer
    {
        private List<BaseObjectData> Types { get; set; }
        public BaseObjectInSerial(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            Types = new  List<BaseObjectData>();
            this.Name = nameof(BaseObjectInSerial);
        }
        public void AddType(Type t, CtorDictionary data =null)
        {
             Types.Add(new BaseObjectData( t, data));
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var data = receiveData;
            var dataToSent = this.dataNeeded;
            foreach(var kv in Types)
            {
                var type=kv.Type;
                var vals=kv.dataNeeded;
                if(vals != null)
                {
                    foreach (var val in vals)
                    {
                        dataToSent[val.Key]=val.Value;
                    }
                }
                var constructType = Activator.CreateInstance(type, dataToSent) as BaseObject;
                data = await constructType.TransformData(data);
                dataToSent = constructType.dataNeeded;

            }
            return data;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
    public class BaseObjectInSerial<T1, T2, T3, T4,T5> : BaseObjectInSerial<T1, BaseObjectInSerial<T2,T3,T4,T5>>, ITransformer
        where T1 : BaseObject
        where T2 : BaseObject
        where T3 : BaseObject
        where T4 : BaseObject
        where T5: BaseObject
    {
        public BaseObjectInSerial(CtorDictionary dataNeeded) : base(dataNeeded)
        {

        }

        

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
    public class BaseObjectInSerial<T1, T2, T3,T4> : BaseObjectInSerial<T1, BaseObjectInSerial<T2,T3,T4>>, ITransformer
        where T1 : BaseObject
        where T2 : BaseObject
        where T3 : BaseObject
        where T4 : BaseObject
    {
        public BaseObjectInSerial(CtorDictionary dataNeeded) : base(dataNeeded)
        {

        }

        

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
    public class BaseObjectInSerial<T1, T2, T3> : BaseObjectInSerial<T1, BaseObjectInSerial<T2,T3>>, ITransformer
        where T1 : BaseObject
        where T2 : BaseObject
        where T3:BaseObject
    {
        public BaseObjectInSerial(CtorDictionary dataNeeded) : base(dataNeeded)
        {

        }

        //public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        //{
           
        //    var f1 = Activator.CreateInstance(typeof(T1), dataNeeded) as BaseObject;
          
        //    var data = await f1.TransformData(receiveData);
        //    var f2 = Activator.CreateInstance(typeof(T2), f1.dataNeeded) as BaseObject;
        //    data = await f2.TransformData(data);
        //    var f3 = Activator.CreateInstance(typeof(T3), f2.dataNeeded) as BaseObject;
        //    data = await f3.TransformData(data);
        //    return data;

        //}

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }

    public class BaseObjectInSerial<T1,T2> : BaseObjectInSerial, ITransformer
        where T1: BaseObject
        where T2 : BaseObject
    {
        public BaseObjectInSerial(CtorDictionary dataNeeded):base(dataNeeded)
        {
            base.AddType(typeof(T1));
            base.AddType(typeof(T2));
        }

        //public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        //{
           
        //    var first = Activator.CreateInstance(typeof(T1), dataNeeded) as BaseObject;          
        //    var data = await first.TransformData(receiveData);
        //    var second = Activator.CreateInstance(typeof(T2),first.dataNeeded) as BaseObject;
        //    data = await second.TransformData(data);
        //    return data;
            
        //}

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
