namespace Shared.Remoting.Interfaces
{
    public interface IClient
    {
        void Notify(params string[] msgs);
    }
}