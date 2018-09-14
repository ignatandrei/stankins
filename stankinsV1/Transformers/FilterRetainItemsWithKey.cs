namespace Transformers
{
    public class FilterRetainItemsWithKey : FilterAddRemoveItemsWithKey
    {

        public FilterRetainItemsWithKey(string key, FilterType filterType) : base(key, filterType)
        {
            this.Name = $"retain items that have at least one key matching {FilterType} {key}";
        }
        public override bool Result(bool exists)
        {
            return exists;
        }
    }
}
