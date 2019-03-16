using Stankins.Rest;
using StankinsCommon;
using System.Threading.Tasks;

namespace Stankins.Jenkins
{
    public class JenkinsJson : ReceiveRestFromFile
    {
        private readonly string url;

        public JenkinsJson(string url) : this(new CtorDictionary()
        {
            {nameof(url),url },
            {nameof(adress), url +"/api/json" }
            })
        {
            
        }
        public JenkinsJson(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            url = GetMyDataOrThrow<string>(nameof(url));
            Name = nameof(JenkinsJson);
        }
        
    }
}
