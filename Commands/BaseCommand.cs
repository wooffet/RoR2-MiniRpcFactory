namespace MiniRpcFactory.Commands
{
    public class BaseCommand
    {
        internal bool IsRegistered { get; private set; }

        internal void MarkCommandAsRegistered()
        {
            IsRegistered = true;
        }

        internal void MarkCommandAsUnregistered()
        {   
            IsRegistered = false;
        }

        internal bool IsCommandRegistered()
        {
            return IsRegistered;
        }
    }
}
