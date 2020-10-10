using MiniRpcFactory.ActionFactory.Contracts;
using MiniRpcFactory.FunctionFactory.Contracts;
using MiniRpcFactory.Logging;
using MiniRpcFactory.RpcService.Contracts;
using MiniRpcLib;
using MiniRpcLib.Action;
using MiniRpcLib.Func;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private FunctionFactoryInstance _commandFactory = new FunctionFactoryInstance();
        private ActionFactoryInstance _actionFactory = new ActionFactoryInstance();

        private Dictionary<string, IRpcFunc<object, object>> _miniRpcFunctionCommands = new Dictionary<string, IRpcFunc<object, object>>();
        private Dictionary<string, IRpcAction<object>> _miniRpcActionCommands = new Dictionary<string, IRpcAction<object>>();

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
            if (_miniRpcFunctionCommands.ContainsKey(commandName))
            {
                return;
            }

            var commandConstructor = _commandFactory.Instance.GenerateCommand<RequestType, ResponseType>(commandType, parameters.Select(p => p.GetType()).ToArray());
            var command = commandConstructor(parameters);
            var commandFunction = command.GetCommandFunction();
            var commandTarget = command.GetTarget();
            var miniRpcCommand = _miniRpcInstance.RegisterFunc(commandTarget, commandFunction);

            _miniRpcFunctionCommands.Add(commandName, (IRpcFunc<object, object>)miniRpcCommand);
            command.MarkAsRegistered();
        }

        public IRpcFunc<RequestType, ResponseType> GetMiniRpcCommandById<RequestType, ResponseType>(string commandName)
        {
            if (!_miniRpcFunctionCommands.ContainsKey(commandName))
            {
                // TODO: Put a sensible error message in exception constructor
                throw new NotSupportedException();
            }

            var command = _miniRpcFunctionCommands[commandName];
            // TODO: Replace Activator throughout codebase with Expression
            return Activator.CreateInstance(command.GetType(), true) as IRpcFunc<RequestType, ResponseType>;
        }

        public void RegisterCommand<RequestType>(string commandName, Type commandType, params object[] parameters)
        {
            if (_miniRpcActionCommands.ContainsKey(commandName))
            {
                return;
            }

            var commandConstructor = _actionFactory.Instance.GenerateCommand<RequestType>(commandType, parameters.Select(p => p.GetType()).ToArray());
            var command = commandConstructor(parameters);
            var commandAction = command.GetCommandAction();
            var commandTarget = command.GetTarget();
            var miniRpcCommand = _miniRpcInstance.RegisterAction(commandTarget, commandAction);

            _miniRpcActionCommands.Add(commandName, (IRpcAction<object>)miniRpcCommand);
            command.MarkAsRegistered();
        }

        public void InvokeRpcCommand(string commandName, object requestParameter, Action<object> handleResponseAction)
        {
            var command = _miniRpcFunctionCommands[commandName];
            command.Invoke(requestParameter, handleResponseAction);
        }

        public void InvokeRpcCommand(string commandName, object requestParameter)
        {
            var command = _miniRpcActionCommands[commandName];
            command.Invoke(requestParameter);
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
