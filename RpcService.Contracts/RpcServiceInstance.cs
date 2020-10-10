namespace MiniRpcFactory.RpcService.Contracts
{
    public sealed class RpcServiceInstance
    {
        public RpcService Instance { get; private set; }

        public RpcServiceInstance()
        {
            Instance = RpcService.CreateInstance();
        }

        public RpcServiceInstance(string modGuid)
        {
            Instance = RpcService.CreateInstance(modGuid);
        }
    }
}
