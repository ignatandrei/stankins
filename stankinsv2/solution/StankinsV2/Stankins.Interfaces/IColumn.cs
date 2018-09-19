namespace Stankins.Interfaces
{
    public interface IColumn: IMetadataRow
    {
        int IDTable { get; set; }
    }
}
