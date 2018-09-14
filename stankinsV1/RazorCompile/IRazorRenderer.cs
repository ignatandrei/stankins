using System.Threading.Tasks;

namespace RazorCompile
{
    public interface IRazorRenderer
    {
        Task<string> RenderToString<TModel>(string contentView, TModel model);
    }
}