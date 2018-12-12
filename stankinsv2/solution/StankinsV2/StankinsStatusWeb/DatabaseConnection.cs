using StankinsObjects;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class DatabaseConnection : CRONExecution, IToBaseObject
    {
        public CustomData CustomData { get; set; }
        public string ConnectionString { get; set; }
        public string TypeOfReceiver { get; set; }
        private BaseObject cache;
        public BaseObject baseObject()
        {
            if (cache == null)
            {
                var type = Type.GetType(TypeOfReceiver);
                var obj = Activator.CreateInstance(type, ConnectionString) as BaseObject;
                obj.Name = CustomData.Name;
                cache = obj;
            }
            return cache;
            
        }
        public async Task<DataTable> Execute()
        {

            var ret = await baseObject().TransformData(null);
            return ret.DataToBeSentFurther.Values.First();
        }
    }
}
