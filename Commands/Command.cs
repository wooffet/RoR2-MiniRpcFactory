using MiniRpcFactory.Commands.Contracts;
using MiniRpcFactory.Logging;
using MiniRpcFactory.RpcService.Contracts;
using MiniRpcLib;
using RoR2;
using System;
using Logger = MiniRpcFactory.Logging.Logger;

namespace MiniRpcFactory.Commands
{
    // TODO: Re-introduce ICommand interface?
    public class Command<RequestType, ResponseType> : BaseCommand
    {
        private Target _target { get; set; }
        private Func<NetworkUser, RequestType, ResponseType> _commandFunction { get; set; }

        public Command(CommandTarget target)
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

        #region Actions
        // TODO: split off into action factory, interface and class
        private Action<RequestType> _actionToRun;

        protected void SetCommandAction(Action<RequestType> actionToRun)
        {
            _actionToRun = actionToRun;
        }

        public Action<RequestType> GetCommandAction()
        {
            if (_actionToRun is null)
            {
                _actionToRun = (RequestType requestType) => { Logger.LogError($"No action supplied by child class of Command for request type: {requestType.GetType().Name}"); };
            }
            return _actionToRun;
        }
        #endregion
    }
}
