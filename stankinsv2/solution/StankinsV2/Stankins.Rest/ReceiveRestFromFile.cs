using System.Text;
using System.Threading.Tasks;
using StankinsCommon;

namespace Stankins.Rest
{
    public class ReceiveRestFromFile: ReceiveRest
    {
        protected string adress;
        public ReceiveRestFromFile(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiveRestFromFile);
            adress = GetMyDataOrThrow<string>(nameof(adress));

        }
       
        public ReceiveRestFromFile(string adress) : this(new CtorDictionary()
        {
            {nameof(adress),adress },
          

        })
        {

        }
        public override async Task<string> GetData()
        {
            var file = new ReadFileToString
            {
                FileEnconding = Encoding.UTF8,
                FileToRead = this.adress
            };
            var data = await file.LoadData();
            return data;

        }
    }
}
