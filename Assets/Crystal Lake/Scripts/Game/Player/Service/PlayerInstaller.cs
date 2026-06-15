using Mirror;
using Zenject;

namespace MyProj
{
    public class PlayerInstaller : NetworkBehaviour
    {
        bool injected;

        public override void OnStartClient()
        {
            if (injected)
                return;

            injected = true;

            ProjectContext.Instance.Container.InjectGameObject(gameObject);
        }
    }
}
