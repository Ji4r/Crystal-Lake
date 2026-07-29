using UnityEngine;
using Zenject;

namespace MyProj
{
    public class GameRegistration : MonoInstaller
    {
        [SerializeField] private EntryPointGame entryPointGame;
        [SerializeField] private StatePlayersManager statePlayersManager;

        override public void InstallBindings()
        {
            Container.Bind<EntryPointGame>().FromInstance(entryPointGame).AsSingle();
            Container.Bind<StatePlayersManager>().FromInstance(statePlayersManager).AsSingle();
        }
    }
}
