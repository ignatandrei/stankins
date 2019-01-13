namespace Stankins.AzureDevOps
{
    public class Script : MultipleCommands
    {
        public Script(string value)
        {
            Name = "script";
            Value = value;
        }
    }

}
