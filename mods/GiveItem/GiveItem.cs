using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using UnityModManagerNet;

namespace GiveItem
{
    public static class GiveItem
    {
        #region Default Mod Properties
        public static bool enabled = false;
        public static UnityModManager.ModEntry.ModLogger Logger;
        #endregion

        #region Default Mod Entry Points
        public static bool Load( UnityModManager.ModEntry modEntry )
        {
            GiveItem.Logger = modEntry.Logger;
            modEntry.OnGUI = GiveItemGUI.OnGUI;

            HarmonyInstance harmony = HarmonyInstance.Create( modEntry.Info.Id );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );

            return true;
        }

        public static bool OnToggle( UnityModManager.ModEntry modEntry, bool value )
        {
            GiveItem.enabled = value;

            return true;
        }
        #endregion
    }
}
