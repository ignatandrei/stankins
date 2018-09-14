using StankinsInterfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SenderAction
{
    public class SenderStringBuilder : SenderActionGenericRow
    {
        public StringBuilder StringWithData;
        public SenderStringBuilder() : base()
        {
            StringWithData = new StringBuilder();
            writeAfter = (rowNumber, s) =>
            {
                if (rowNumber < valuesToBeSent.Length - 1)
                    StringWithData.AppendLine();
            };
            writeValues = (rowNumber,s) => StringWithData.Append(s);
            
        }
    }
    public class SenderActionConsole : SenderActionGenericRow
    {
        public SenderActionConsole() : base()
        {
            writeAfter = (rowNumber, t) =>
            {
                if (rowNumber < valuesToBeSent.Length - 1)
                    Console.WriteLine("");
            };
            writeValues = (rowNumber,s) => Console.Write(s);
        }
    }
    public abstract class SenderActionGenericRow : SenderActionOrdered
    {
        
        public SenderActionGenericRow() : base()
        {

        }
        protected Action<int,string> writeValues;
        protected Action<int,IRow> writeBefore;
        protected Action<int, IRow> writeAfter;
        public void Generate()
        {
            ActionToRow = (rowNumber,r) =>
            {
                writeBefore?.Invoke(rowNumber, r);
                foreach (var item in r.Values)
                {
                    writeValues?.Invoke(rowNumber, $"{item.Key} =>{item.Value}");
                }
                writeAfter?.Invoke(rowNumber,r);

            };
        }
        public override Task Send()
        {
            if(ActionToRow== null)
                Generate();

            return base.Send();
        }

    }
    
}
