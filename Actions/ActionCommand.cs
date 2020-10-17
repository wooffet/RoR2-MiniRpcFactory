using MiniRpcFactory.Commands.Contracts;
using MiniRpcLib;
using RoR2;
using System;
using Logger = MiniRpcFactory.Logging.Logger;

namespace MiniRpcFactory.Actions
{
    // TODO: Re-introduce ICommand interface?
    public class ActionCommand<RequestType> : BaseCommand
    {
        private Target _target { get; set; }

        public ActionCommand(CommandTarget target)
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

        private Action<NetworkUser, RequestType> _actionToRun;

        protected void SetCommandAction(Action<NetworkUser, RequestType> actionToRun)
        {
            _actionToRun = actionToRun;
        }

        public Action<NetworkUser, RequestType> GetCommandAction()
        {
            if (_actionToRun is null)
            {
                _actionToRun = (networkUser, requestType) => DefaultAction(requestType);
            }
            return _actionToRun;
        }

        private void DefaultAction(RequestType requestType)
        {
            Logger.LogError($"No action supplied by child class of Command for request type: {requestType.GetType().Name}");
        }

        public Target GetTarget()
        {
            return _target;
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
