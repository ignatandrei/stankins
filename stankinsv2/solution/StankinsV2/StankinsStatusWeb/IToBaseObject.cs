using StankinsObjects;
using System.Data;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public interface IToBaseObject
    {
        BaseObject baseObject();
        Task<DataTable> Execute();
    }
}
