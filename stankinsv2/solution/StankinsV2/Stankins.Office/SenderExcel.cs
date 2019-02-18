using StankinsCommon;

using StankinsObjects;

namespace Stankins.Office
{
    public class SenderExcel : BaseObjectInSerial<SenderOutputExcel,SenderOutputToFolder>
    {
        public SenderExcel(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(SenderExcel);

        }
        public SenderExcel(string fileName) : this(new CtorDictionary()
        {

            { nameof(fileName),fileName},
            {"addKey",false }
        })
        {
            this.Name = nameof(SenderExcel);
        }

        


    }
}
