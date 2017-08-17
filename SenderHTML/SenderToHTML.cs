using RazorCompile;
using SenderToFile;
using StankinsInterfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SenderHTML
{
    public class Sender_HTML : SenderMediaToFile
    {
        public string ViewFileName { get; set; }
        
        public Sender_HTML(string viewFileName,string fileName) 
            : base(new MediaTransformRazor(viewFileName), fileName)
        {
            this.ViewFileName = viewFileName;
        
        }
        
    }
}
