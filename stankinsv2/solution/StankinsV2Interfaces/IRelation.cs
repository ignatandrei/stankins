namespace StankinsV2Interfaces
{
    public interface IRelation
    {
        long IdColumnParent { get; set; }
        long IdColumnChild { get; set; }
    }
}
