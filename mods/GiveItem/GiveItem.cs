using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using UnityModManagerNet;

namespace GiveItem
{
    public static class GiveItem
    {
        public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

        public static bool Load( UnityModManager.ModEntry modEntry )
        {
            Logger = modEntry.Logger;
            modEntry.OnGUI = GiveItemGUI.OnGUI;
            modEntry.OnToggle = OnToggle;

            HarmonyInstance harmony = HarmonyInstance.Create( modEntry.Info.Id );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );

            return true;
        }

        public static bool OnToggle( UnityModManager.ModEntry modEntry, bool value )
        {
            if( !value )
                modEntry.OnGUI = null;
            else
                modEntry.OnGUI = GiveItemGUI.OnGUI;

            return true;
        }
    }
}
