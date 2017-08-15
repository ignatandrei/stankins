using RazorCompile;
using StankinsInterfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SenderHTML
{
    public class SenderToHTML : ISend
    {
        public string ViewFileName { get; set; }
        public string OutputFile { get; set; }
        public SenderToHTML(string viewFileName,string outputFile)
        {
            this.ViewFileName = viewFileName;
            this.OutputFile = outputFile;
        }
        public IRow[] valuesToBeSent { set; protected get; }

        public async Task Send()
        {
            string content = File.ReadAllText(ViewFileName);

            IRazorRenderer c = new ConfigureRazor();
            
            var data= await c.RenderToString(content, valuesToBeSent);
            var buffer = Encoding.UTF8.GetBytes(data);

            using (var fs = new FileStream(OutputFile, FileMode.OpenOrCreate,
                FileAccess.Write, FileShare.None, buffer.Length, true))
            {
                await fs.WriteAsync(buffer, 0, buffer.Length);
            }
            
        }
    }
}
