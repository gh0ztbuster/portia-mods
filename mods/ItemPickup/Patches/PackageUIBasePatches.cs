using System;
using System.Collections.Generic;
using Harmony;
using Pathea.UISystemNs;
using Pathea.UISystemNs.MainMenu.PackageUI;
using UModLib.Logging;

namespace ItemPickup.Patches
{
    [HarmonyPatch( typeof( PackageUIBase ) )]
    [HarmonyPatch( "DragBegin" )]
    internal class PackageUIBase_DragBeginPatches
    {
        private static void Prefix()
        {
            ULogger.LogTrace( "PackageUIBase::DragBegin" );
        }
    }

    [HarmonyPatch( typeof( PackageUIBase ) )]
    [HarmonyPatch( "DragEnd" )]
    internal class PackageUIBase_DragEndPatches
    {
        private static void Prefix()
        {
            ULogger.LogTrace( "PackageUIBase::DragEnd" );
        }
    }

    [HarmonyPatch( typeof( PackageUICtr ) )]
    [HarmonyPatch( "DragEnd" )]
    internal class PackageUICtr_DragEndPatches
    {
        private static void Prefix()
        {
            ULogger.LogTrace( "PackageUICtr::DragEnd (Main Menu)" );
        }
    }

    [HarmonyPatch( typeof( StoreageUIBase ) )]
    [HarmonyPatch( "DragBeginStoreage" )]
    internal class StorageUIBase_DragBeginStoragePatches
    {
        private static void Prefix()
        {
            ULogger.LogTrace( "StorageUIBase::DragBeginStoreage" );
        }
    }

    [HarmonyPatch( typeof( StoreageUIBase ) )]
    [HarmonyPatch( "DragEndStoreage" )]
    internal class StorageUIBase_DragEndStoreagePatches
    {
        private static void Prefix()
        {
            ULogger.LogTrace( "StorageUIBase::DragEndStoreage" );
        }
    }

    [HarmonyPatch( typeof( PlayerItemBarCtr ) )]
    [HarmonyPatch( "DragBegin" )]
    internal class PlayerItemBarCtr_DragBeginPatches
    {
        private static void Prefix()
        {
            ULogger.LogTrace( "PlayerItemBarCtr::DragBegin" );
        }
    }

    [HarmonyPatch( typeof( PlayerItemBarCtr ) )]
    [HarmonyPatch( "DragEnd" )]
    internal class PlayerItemBarCtr_DragEndPatches
    {
        private static void Prefix()
        {
            ULogger.LogTrace( "PlayerItemBarCtr::DragEnd" );
        }
    }

    [HarmonyPatch( typeof( PackageExchangeUICtr ) )]
    [HarmonyPatch( "DragBeginStoreage" )]
    internal class PackageExchangeUICtr_DragBeginStoreagePatches
    {
        private static void Prefix()
        {
            ULogger.LogTrace( "PackageExchangeUICtr::DragBeginStoreage" );
        }
    }

    [HarmonyPatch( typeof( PackageExchangeUICtr ) )]
    [HarmonyPatch( "DragEnd" )]
    internal class PackageExchangeUICtr_DragEndPatches
    {
        private static void Prefix()
        {
            ULogger.LogTrace( "PackageExchangeUICtr::DragEnd" );
        }
    }

    [HarmonyPatch( typeof( PackageExchangeUICtr ) )]
    [HarmonyPatch( "DragEndStoreage" )]
    internal class PackageExchangeUICtr_DragEndStoreagePatches
    {
        private static void Prefix()
        {
            ULogger.LogTrace( "PackageExchangeUICtr::DragEndStoreage" );
        }
    }


}
