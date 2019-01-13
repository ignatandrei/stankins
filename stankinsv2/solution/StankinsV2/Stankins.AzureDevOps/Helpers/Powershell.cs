namespace Stankins.AzureDevOps
{
    public class Powershell : MultipleCommands
    {
        public Powershell(string value)
        {
            Name = "powershell";
            Value = value;
        }
    }

}
