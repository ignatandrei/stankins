using Newtonsoft.Json;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenderJSON
{
    public class Sender_JSON : ISend
    {


        public IRow[] valuesToBeSent { private get; set; }
        public string JSONFileName { get; set; }
        public Sender_JSON(string fileName)
        {
            this.JSONFileName = fileName;
        }


        public async Task Send()
        {

             
            File.WriteAllText(JSONFileName, data);

        }
    }
}
