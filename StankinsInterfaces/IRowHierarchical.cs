namespace StankinsInterfaces
{
    public interface IRowReceiveHierarchical : IRowReceive
    {
        long ID { get; set; }
        IRowReceiveHierarchical Parent { get; set; }
    }
}