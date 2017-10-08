using Transformers;
namespace ReceiverFileSystem
{
    public class FilterForFilesHierarchical : FilterForHierarchical
    {
        public FilterForFilesHierarchical() : base(new FilterComparableEqual(typeof(string), "file", "RowType"))
        {
            Name = $"filter hierarchical for files";
        }
    }
}
