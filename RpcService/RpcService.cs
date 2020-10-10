using MiniRpcFactory.CommandFactory.Contracts;
using MiniRpcFactory.Logging;
using MiniRpcFactory.RpcService.Contracts;
using MiniRpcLib;
using MiniRpcLib.Func;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Logger = MiniRpcFactory.Logging.Logger;

namespace MiniRpcFactory.RpcService
{
    public sealed class RpcService
    {
        private static Lazy<RpcService> _rpcService { get; set; }
        private static RpcService _instance { get { return _rpcService.Value; } }
        private static MiniRpcInstance _miniRpcInstance { get; set; }
        private CommandFactoryInstance _commandFactory = new CommandFactoryInstance();
        private Dictionary<string, IRpcFunc<object, object>> _miniRpcCommands = new Dictionary<string, IRpcFunc<object, object>>();

        private RpcService()
        {
            _miniRpcInstance = MiniRpc.CreateInstance(new Guid().ToString());
        }

        private RpcService(string modGuid)
        {
            _miniRpcInstance = MiniRpc.CreateInstance(modGuid);
        }

        internal static RpcService CreateInstance()
        {
            if (!_rpcService.IsValueCreated)
            {
                _rpcService = new Lazy<RpcService>(() => new RpcService());
            }

            return _instance;
        }

        internal static RpcService CreateInstance(string modGuid)
        {
            if (!_rpcService.IsValueCreated)
            {
                _rpcService = new Lazy<RpcService>(() => new RpcService(modGuid));
            }

            return _instance;
        }

        public void RegisterCommand<RequestType, ResponseType>(string commandName, Type commandType, params object[] parameters)
        {
            if(_miniRpcCommands.ContainsKey(commandName))
            {
                return;
            }

            var commandConstructor = _commandFactory.Instance.GenerateCommand<RequestType, ResponseType>(commandType, parameters.Select(p => p.GetType()).ToArray());
            var command = commandConstructor(parameters);
            var commandFunction = command.GetCommandFunction();
            var commandTarget = command.GetTarget();
            var miniRpcCommand = _miniRpcInstance.RegisterFunc(commandTarget, commandFunction);

            _miniRpcCommands.Add(commandName, (IRpcFunc<object, object>)miniRpcCommand);
            command.MarkAsRegistered();
        }

        public IRpcFunc<RequestType, ResponseType> GetMiniRpcCommandById<RequestType, ResponseType>(string commandName)
        {
            if (!_miniRpcCommands.ContainsKey(commandName))
            {
                // TODO: Put a sensible error message in exception constructor
                throw new NotSupportedException();
            }

            var command = _miniRpcCommands[commandName];
            // TODO: Replace Activator throughout codebase with Expression
            return Activator.CreateInstance(command.GetType(), true) as IRpcFunc<RequestType, ResponseType>;
        }

        public void InvokeRpcCommand(string commandName, object requestParameter, Action<object> handleResponseAction)
        {
            var command = _miniRpcCommands[commandName];
            command.Invoke(requestParameter, handleResponseAction);
        }

        public void HandleRpcResult(RpcResult result)
        {
            if (result.Success)
            {
                HandleSuccessRpcResult(result);
            }
            else
            {
                HandleFailedRpcResult(result);
            }
        }

        public void HandleSuccessRpcResult(RpcResult result)
        {
            switch (result.Severity)
            {
                case LogSeverity.Info:
                    LogInfoMessage(result.Message);
                    break;
                default:
                    break;
            }
        }

        public void HandleFailedRpcResult(RpcResult result)
        {
            switch (result.Severity)
            {
                case LogSeverity.Info:
                    LogInfoMessage(result.Message);
                    break;
                case LogSeverity.Warning:
                    LogWarningMessage(result.Message);
                    break;
                case LogSeverity.Error:
                    LogErrorMessage(result.Message);
                    break;
                default:
                    break;
            }
        }

        private void LogInfoMessage(string message)
        {
            Logger.LogInfo(message);
        }

        private void LogWarningMessage(string message = "Debug", [CallerLineNumber] int lineNumber = 0)
        {
            string warning = $"Line {lineNumber}: {message}";
            Logger.LogWarning(warning);
        }

        private void LogErrorMessage(string message = "Error", [CallerLineNumber] int lineNumber = 0)
        {
            string warning = $"Line {lineNumber}: {message}";
            Logger.LogError(warning);
        }
    }
}
