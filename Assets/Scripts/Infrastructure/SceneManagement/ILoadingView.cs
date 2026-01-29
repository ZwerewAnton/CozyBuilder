using Cysharp.Threading.Tasks;

namespace Infrastructure.SceneManagement
{
    public interface ILoadingView
    {
        UniTask ShowAsync();
        UniTask HideAsync();
    }
}