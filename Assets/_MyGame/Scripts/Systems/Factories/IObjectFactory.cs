using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _MyGame.Scripts.Systems.Factories
{
    public interface IObjectFactory<T> where T : MonoBehaviour
    {
        UniTask<T> CreateObject(Vector2 position, Vector2 direction, CancellationToken token);
    }
}
