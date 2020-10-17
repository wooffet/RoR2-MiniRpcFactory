using MiniRpcFactory.Commands.Contracts;
using MiniRpcFactory.Logging;
using MiniRpcFactory.RpcService.Contracts;
using MiniRpcLib;
using RoR2;
using System;
using Logger = MiniRpcFactory.Logging.Logger;

namespace MiniRpcFactory.Functions
{
    // TODO: Re-introduce ICommand interface?
    public class FunctionCommand<RequestType, ResponseType> : BaseCommand
    {
        private Target _target { get; set; }
        private Func<NetworkUser, RequestType, ResponseType> _commandFunction { get; set; }

        public FunctionCommand(CommandTarget target)
        {
            switch (target)
            {
                case CommandTarget.Server:
                    _target = Target.Server;
                    break;
                case CommandTarget.Client:
                default:
                    _target = Target.Client;
                    break;
            }
        }

        protected virtual ResponseType FunctionToRun(NetworkUser networkUser, RequestType request)
        {
            Logger.LogError($"No function supplied by child class of Command for request type: {request.GetType().Name}");
            return (ResponseType)Activator.CreateInstance(typeof(RpcResult), false, $"No function supplied child class of Command for request type: {nameof(RequestType)} response type {nameof(ResponseType)}", LogSeverity.Error);
        }

        protected void SetCommandFunction(Func<NetworkUser, RequestType, ResponseType> functionToRun)
        {
            _commandFunction = functionToRun;
        }

        public Target GetTarget()
        {
            return _target;
        }

        public Func<NetworkUser, RequestType, ResponseType> GetCommandFunction()
        {
            if (_commandFunction is null)
            {
                _commandFunction = FunctionToRun;
            }
            return _commandFunction;
        }

        public bool IsInstanceRegistered()
        {
            return IsCommandRegistered();
        }

        public void MarkAsRegistered()
        {
            MarkCommandAsRegistered();
        }

        public void MarkAsUnregistered()
        {
            MarkCommandAsUnregistered();
        }
    }
}
