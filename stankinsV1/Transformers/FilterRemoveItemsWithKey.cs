namespace Transformers
{
    public class FilterRemoveItemsWithKey : FilterAddRemoveItemsWithKey
    {

        public FilterRemoveItemsWithKey(string key, FilterType filterType):base(key,filterType)
        {
            this.Name = $"remove items that have at least one key matching {FilterType} {key}";
        }
        public override bool Result(bool exists)
        {
            return !exists;
        }
    }
}
