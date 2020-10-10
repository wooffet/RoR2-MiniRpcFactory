namespace MiniRpcFactory.CommandFactory.Contracts
{
    public sealed class CommandFactoryInstance
    {
        internal CommandFactory Instance { get; private set; }

        public CommandFactoryInstance()
        {
            Instance = CommandFactory.CreateFactoryInstance();
        }
    }
}
