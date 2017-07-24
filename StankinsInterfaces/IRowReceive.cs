namespace StankinsInterfaces
{
    public interface IRowReceive:IRow
    {
        ICommonData CommonData { get; }
        string ReceiverName { get; }
        string AdditionalDetails { get; }
    }
}