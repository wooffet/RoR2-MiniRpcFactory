using BepInEx;
using MiniRpcLib;

namespace MiniRpcFactory
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency(MiniRpcPlugin.Dependency)]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class MiniRpcFactory : BaseUnityPlugin
    {
        public const string Dependency = ModGuid;

        private const string ModVer = "1.0";
        private const string ModName = "MiniRpcFactory";
        private const string ModGuid = "com.wooffet.minirpcfactory";

        public MiniRpcFactory()
        {
            Logger.LogInfo("Initialising MiniRpcFactory...");
            InitialiseLogger();
            Logger.LogInfo("MiniRpcFactory Loaded!");
        }

        private void InitialiseLogger()
        {
            Logging.Logger.LogInfo = Logger.LogInfo;
            Logging.Logger.LogDebug = Logger.LogDebug;
            Logging.Logger.LogWarning = Logger.LogWarning;
            Logging.Logger.LogError = Logger.LogError;
        }
    }
}
