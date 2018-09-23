using StankinsCommon;

namespace Stankins.File
{
    public class ReceiverCSVText : ReceiveCSV<ReceiverStreamingText>
    {
        public ReceiverCSVText(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiverCSVText);
            this.Text = GetMyDataOrThrow<string>(nameof(Text));

        }
        public ReceiverCSVText(string text,  bool firstLineHasColumnNames, char columnSeparator, char lineSeparator) : this(new CtorDictionary()
            {
                {nameof(text),text},
                {nameof(columnSeparator),columnSeparator },
                {nameof(lineSeparator),lineSeparator},
                {nameof(firstLineHasColumnNames),firstLineHasColumnNames }

            })
        {
            
        }

        public string Text { get; }

        public override ReceiverStreamingText Create()
        {
            return new ReceiverStreamingText(Text, LineSeparator);
        }

        public override string NameTable()
        {
            return "Table1";
        }
    }
}