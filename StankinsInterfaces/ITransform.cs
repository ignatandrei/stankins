namespace StankinsInterfaces
{
    public interface ITransform: IFilterTransformer
    {
        IRow[] valuesRead { get; set; }
        IRow[] valuesTransformed{ get; set; }
    }
}