namespace Stankins.AzureDevOps
{
    public class Checkout : Step
    {
        public Checkout(string value)
        {
            Name = "checkout";
            Value = value;
        }
    }

}
