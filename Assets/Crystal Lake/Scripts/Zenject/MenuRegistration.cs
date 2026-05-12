using UnityEngine;
using Zenject;

namespace MyProj
{
    public class MenuRegistration : MonoInstaller
    {
        [SerializeField] private LobbyManager lobbyManager;

        public override void InstallBindings()
        {
            Container.Bind<LobbyManager>()
                .FromInstance(lobbyManager)
                .AsSingle()
                .NonLazy();
        }
    }
}
