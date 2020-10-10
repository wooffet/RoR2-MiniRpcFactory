using BepInEx;
using MiniRpcFactory.Commands;
using MiniRpcFactory.Commands.Contracts;
using MiniRpcFactory.Functions;
using MiniRpcFactory.Logging;
using MiniRpcFactory.RpcService.Contracts;
using RoR2;
using System;
using UnityEngine;

namespace MiniRpcFactory.Demo
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency(MiniRpcFactory.Dependency)]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class MiniRpcFactoryDemo
    {
        public RpcServiceInstance RpcService = new RpcServiceInstance(ModGuid);

        private const string ModVer = "1.0";
        private const string ModName = "MiniRpcFactoryDemo";
        private const string ModGuid = "com.wooffet.minirpcfactorydemo";

        public MiniRpcFactoryDemo()
        {
            
        }
    }

    public class ExampleCommand: FunctionCommand<string, bool>
    {
        public ExampleCommand(CommandTarget target) : base(target)
        {

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

        }

        protected override RpcResult FunctionToRun(NetworkUser networkUser, string request)
        {
            Debug.Log($"Request value: {request}");
            return new RpcResult(true, "ExampleCommandReturnRpcResult has run successfully!", LogSeverity.Info);
        }
    }

    public class ExampleCommandAction : FunctionCommand<string, object>
    {
        public ExampleCommandAction(CommandTarget target) : base(target)
        {
            SetCommandAction(ExampleAction);
        }

        public void ExampleAction(string request)
        {
            Debug.Log($"ExampleCommandAction has run ExampleAction with request value: {request}");
        }
    }
}
