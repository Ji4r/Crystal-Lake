using UnityEngine;

namespace MyProj
{
    public interface IUseProp 
    {
        public bool IsDisposable { get; }
        public void Use(Camera gameCamera);
    }
}
