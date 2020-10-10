namespace MiniRpcFactory.FunctionFactory.Contracts
{
    public sealed class FunctionFactoryInstance
    {
        internal FunctionFactory Instance { get; private set; }

        public FunctionFactoryInstance()
        {
            Instance = FunctionFactory.CreateFactoryInstance();
        }
    }
}
