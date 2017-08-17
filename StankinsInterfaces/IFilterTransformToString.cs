namespace StankinsInterfaces
{
    public interface IFilterTransformToString:IFilterTransformer
    {
        IRow[] valuesToBeSent { set; }
        string Result { get; }
    }
}
