using Zenject;
using UnityEngine.SceneManagement;

namespace MyProj
{
    public class ProjectStarter : IInitializable
    {
        public void Initialize()
        {
            SceneManager.LoadScene(SceneName.MENU);
        }
    }
}
