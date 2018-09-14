namespace StankinsInterfaces
{
    public interface IFilterTransformToByteArray: IFilterTransformer
    {
        IRow[] valuesToBeSent { set; }
        byte[] Result { get; }
    }
}
