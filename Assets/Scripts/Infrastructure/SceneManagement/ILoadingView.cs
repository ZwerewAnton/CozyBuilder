using System.Threading;
using Cysharp.Threading.Tasks;

namespace Infrastructure.SceneManagement
{
    public interface ILoadingView
    {
        UniTask ShowAsync(CancellationToken token = default);
        UniTask HideAsync(CancellationToken token = default);
    }
}