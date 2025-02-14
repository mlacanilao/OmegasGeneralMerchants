using BepInEx;
using HarmonyLib;

namespace OmegasGeneralMerchants
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.omegasgeneralmerchants";
        internal const string Name = "Omegas General Merchants";
        internal const string Version = "1.0.2.0";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    internal class OmegasGeneralMerchants : BaseUnityPlugin
    {
        internal static OmegasGeneralMerchants Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
        
        internal static void Log(object payload)
        {
            Instance?.Logger.LogInfo(data: payload);
        }
    }
}