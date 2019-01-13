namespace Stankins.AzureDevOps
{
    public abstract class Step
    {
        public string Name { get; protected set; }
        public string Value { get; protected set; }
        public string DisplayName { get; set; }
    }

}
