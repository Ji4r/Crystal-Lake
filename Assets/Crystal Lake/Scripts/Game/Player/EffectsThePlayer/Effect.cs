using System.Threading.Tasks;

namespace MyProj
{
    public abstract class Effect
    {
        public virtual Task EnableEffect() => Task.CompletedTask;
        public virtual Task DissableEffect() => Task.CompletedTask;
        public virtual Task AddEffectTime() => Task.CompletedTask;
    }
}
