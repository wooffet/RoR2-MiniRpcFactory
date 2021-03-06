﻿using BepInEx;
using MiniRpcFactory.Actions;
using MiniRpcFactory.Commands.Contracts;
using MiniRpcFactory.Functions;
using MiniRpcFactory.Logging;
using MiniRpcFactory.RpcService.Contracts;
using RoR2;
using UnityEngine;

namespace MiniRpcFactory.Demo
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency(MiniRpcFactory.Dependency)]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class MiniRpcFactoryDemo
    {
        public RpcServiceInstance RpcServiceInstance = new RpcServiceInstance(ModGuid);

        private const string ModVer = "1.0";
        private const string ModName = "MiniRpcFactoryDemo";
        private const string ModGuid = "com.wooffet.minirpcfactorydemo";

        private RpcServiceInstance _rpc = new RpcServiceInstance(ModGuid);


        public MiniRpcFactoryDemo()
        {
            var rpcService = _rpc.Instance;

            rpcService.RegisterCommand<string, bool>(nameof(ExampleCommand), typeof(ExampleCommand), CommandTarget.Client);

            var classInput = 99;
            rpcService.RegisterCommand<string, bool>(nameof(ExampleCommandWithClassProperties), typeof(ExampleCommandWithClassProperties), CommandTarget.Client, classInput);

            rpcService.RegisterCommand<string, RpcResult>(nameof(ExampleCommandReturnRpcResult), typeof(ExampleCommandReturnRpcResult), CommandTarget.Client);

            rpcService.RegisterCommand<string>(nameof(ExampleCommandAction), typeof(ExampleCommandAction), CommandTarget.Client);

            RegisterGameEvents();
        }

        private void RegisterGameEvents()
        {
            On.RoR2.UI.HUD.Awake += (orig, self) =>
            {
                orig(self);

                var rpcService = _rpc.Instance;

                var exampleCommandRequestValue = "Example Command Request Value";
                rpcService.InvokeRpcCommand(nameof(ExampleCommand), exampleCommandRequestValue, (result) => Debug.Log($"Result: {result}"));

                var exampleCommandWithClassPropertiesResponseValue = "Example Command With Class Propertied Request Value";
                rpcService.InvokeRpcCommand(nameof(ExampleCommandWithClassProperties), exampleCommandWithClassPropertiesResponseValue, (result) => Debug.Log($"Result: {result}"));

                var exampleCommandReturnRpcResultRequestValue = "Example Command Return Rpc Result Request Value";
                rpcService.InvokeRpcCommand(nameof(ExampleCommandReturnRpcResult), exampleCommandReturnRpcResultRequestValue, (result) => rpcService.HandleRpcResult(result as RpcResult));

                var exampleCommandActionRequestValue = "Example Command Action Request Value";
                rpcService.InvokeRpcCommand(nameof(ExampleCommandAction), exampleCommandActionRequestValue);
            };
        }
    }

    public class ExampleCommand: FunctionCommand<string, bool>
    {
        public ExampleCommand(CommandTarget target) : base(target)
        {
            SetCommandFunction((user, request) => FunctionToRun(new NetworkUser(), request));
        }

        protected override bool FunctionToRun(NetworkUser networkUser, string request)
        {
            Debug.Log($"Request value: {request}");
            return true;
        }
    }

    public class ExampleCommandWithClassProperties : FunctionCommand<string, bool>
    {
        public int ExampleClassProperty { get; set; }

        public ExampleCommandWithClassProperties(CommandTarget target, int classInput) : base(target)
        {
            ExampleClassProperty = classInput;
            SetCommandFunction((user, request) => FunctionToRun(new NetworkUser(), (ExampleClassProperty == 99 ? ExampleClassProperty.ToString() : request)));
        }

        protected override bool FunctionToRun(NetworkUser networkUser, string request)
        {
            Debug.Log($"Request value: {request} ExampleClassProperty value: {ExampleClassProperty}");
            return true;
        }
    }

    public class ExampleCommandReturnRpcResult : FunctionCommand<string, RpcResult>
    {
        public ExampleCommandReturnRpcResult(CommandTarget target) : base(target)
        {
            SetCommandFunction((user, request) => FunctionToRun(new NetworkUser(), request));
        }

        protected override RpcResult FunctionToRun(NetworkUser networkUser, string request)
        {
            Debug.Log($"Request value: {request}");
            return new RpcResult(true, "ExampleCommandReturnRpcResult has run successfully!", LogSeverity.Info);
        }
    }

    public class ExampleCommandAction : ActionCommand<string>
    {
        public ExampleCommandAction(CommandTarget target) : base(target)
        {
            SetCommandAction((user, request) => ExampleAction(new NetworkUser(), request));
        }

        public void ExampleAction(NetworkUser networkUser, string request)
        {
            Debug.Log($"ExampleCommandAction has run ExampleAction with request value: {request}");
        }
    }
}
