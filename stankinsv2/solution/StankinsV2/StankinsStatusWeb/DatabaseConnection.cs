using StankinsObjects;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class DatabaseConnection : CRONExecution
    {
        
        public string ConnectionString { get; set; }
        public string TypeOfReceiver { get; set; }
        private BaseObject cache;
        public override BaseObject baseObject()
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
    }
}
