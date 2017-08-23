namespace StankinsInterfaces
{
    public interface IRowReceiveHierarchical : IRowReceive
    {
        IRow Parent { get; set; }
    }
}