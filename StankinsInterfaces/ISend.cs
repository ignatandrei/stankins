using System.Threading.Tasks;

namespace StankinsInterfaces
{
    public interface ISend
    {
        IRow[] valuesToBeSent{ set; }
        Task Send();
        //TODO: just export some fields, not all...
        //string[] FieldNames { set; }
    }
}