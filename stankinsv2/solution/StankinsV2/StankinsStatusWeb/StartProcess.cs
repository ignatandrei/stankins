using Stankins.Alive;
using StankinsObjects;

namespace StankinsStatusWeb
{
    public class StartProcess : CRONExecution, IToBaseObjectExecutable
    {
        public string FileName { get; set; }
        public string Parameters { get; set; }
        private ReceiverProcessAlive cache;
        public override BaseObject baseObject()
        {
            if (cache == null)
                cache = new ReceiverProcessAlive(FileName,Parameters)
                {
                    Name = CustomData.Name
                };

            return cache;
        }



    }
}
