using MiniRpcFactory.Commands;
using MiniRpcFactory.Commands.Contracts;
using MiniRpcFactory.Functions;
using MiniRpcFactory.Logging;
using MiniRpcFactory.RpcService.Contracts;
using MiniRpcLib;
using RoR2;
using System;

namespace ArtifactOfDoom.Functions
{
    public class IsArtifactActiveCommand : FunctionCommand<bool, RpcResult>
    {
        public bool IsActive { get; set; }

        public IsArtifactActiveCommand(CommandTarget target, bool isActive) : base(target)
        {
            IsActive = isActive;
            // TODO: Potentially move into base constuctor so this is not missed by users creating their own commands
            SetCommandFunction((user, request) => FunctionToRun(new NetworkUser(), IsActive));
        }

        protected override RpcResult FunctionToRun(NetworkUser networkUser, bool request)
        {
            try
            {
                IsActive = request;

                return new RpcResult(true, "UI aware Artifact of Doom is active!", LogSeverity.Info); ;
            }
            catch (Exception e)
            {
                return new RpcResult(false, $"IsArtifactActiveCommand Error: {e}", LogSeverity.Error);
            }
        }
    }
}
