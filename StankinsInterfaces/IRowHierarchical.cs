namespace StankinsInterfaces
{
    public interface IRowReceiveHierarchicalParent : IRowReceive
    {
        long ID { get; set; }
        IRowReceiveHierarchicalParent Parent { get; set; }
    }
}