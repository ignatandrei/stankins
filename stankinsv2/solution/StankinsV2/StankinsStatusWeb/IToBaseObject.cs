using StankinsObjects;
using System.Data;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public interface IToBaseObjectExecutable
    {
        BaseObject baseObject();
        Task<DataTable> Execute();
    }
}
