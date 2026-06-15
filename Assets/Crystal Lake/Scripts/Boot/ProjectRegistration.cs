using Zenject;
using UnityEngine;

namespace MyProj
{
    public class ProjectRegistration : MonoInstaller
    {
        [SerializeField] private SteamManager steam;
        [SerializeField] private MyNetworkManager networkManager;
        [SerializeField] private VoiceSettings voiceSettings;

        public override void InstallBindings()
        {
            Container.Bind<SteamManager>().FromInstance(steam).AsSingle();
            Container.Bind<MyNetworkManager>().FromInstance(networkManager).AsSingle();   
            Container.Bind<VoiceSettings>().FromInstance(voiceSettings).AsSingle();

            Container.Bind<IInitializable>()
                .To<ProjectStarter>()
                .AsSingle()
                .NonLazy();
        }
    }
}
