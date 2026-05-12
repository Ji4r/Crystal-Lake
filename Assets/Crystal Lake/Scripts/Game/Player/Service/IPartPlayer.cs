namespace MyProj
{
    public interface IPartPlayer { }

    public interface ILocalOnly : IPartPlayer
    {
        public void LocalDissable();
    }
}
