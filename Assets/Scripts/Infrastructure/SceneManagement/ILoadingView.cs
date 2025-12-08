using System.Threading.Tasks;

namespace Infrastructure.SceneManagement
{
    public interface ILoadingView
    {
        Task ShowAsync();
        Task HideAsync();
    }
}