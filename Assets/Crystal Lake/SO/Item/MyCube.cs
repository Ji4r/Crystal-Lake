using UnityEngine;

namespace MyProj
{
    public class MyCube : Item, IUseProp
    {
        public bool IsDisposable => false;

        public void Use(Camera gameCamera)
        {
            throw new System.NotImplementedException();
        }
    }
}
