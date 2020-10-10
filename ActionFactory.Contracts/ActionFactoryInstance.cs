namespace MiniRpcFactory.ActionFactory.Contracts
{
    public sealed class ActionFactoryInstance
    {
        internal ActionFactory Instance { get; private set; }

        public ActionFactoryInstance()
        {
            Instance = ActionFactory.CreateFactoryInstance();
        }
    }
}
