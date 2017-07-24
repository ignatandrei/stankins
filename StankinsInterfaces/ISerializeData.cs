namespace StankinsInterfaces
{
    public interface ISerializeData
    {
        object GetValue(string key);
        void SetValue(string key, object value);
    }
}