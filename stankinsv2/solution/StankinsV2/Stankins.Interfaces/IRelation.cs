namespace Stankins.Interfaces
{
    public interface IRelation
    {
        long IdTableParent { get; set; }
        long IdTableChild { get; set; }
        string ColumnParent { get; set; }
        string ColumnChild { get; set; }
    }
}
