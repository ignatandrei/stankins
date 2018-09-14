using Transformers;
namespace ReceiverFileSystem
{
    public class FilterForFoldersHierarchical : FilterForHierarchical
    {
        public FilterForFoldersHierarchical() : base(new FilterComparableEqual(typeof(string), "folder", "RowType"))
        {
            Name = $"filter hierarchical for folders";
        }
    }
}
