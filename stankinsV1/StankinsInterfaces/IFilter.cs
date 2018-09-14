namespace StankinsInterfaces
{
    public interface IFilter: IFilterTransformer
    {
        IRow[] valuesRead { get; set; }
        IRow[] valuesTransformed { get; set; }
    }
}