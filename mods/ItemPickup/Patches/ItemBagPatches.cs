using System;
using System.Collections.Generic;
using Harmony;
using Pathea.ItemSystem;
using UModLib.Logging;

namespace ItemPickup.Patches
{
    [HarmonyPatch( typeof( ItemBag ) )]
    [HarmonyPatch( "AddItem" )]
    [HarmonyPatch( new Type[] { typeof( ItemObject ), typeof( AddItemMode ) } )]
    internal class ItemBagAddItemPatches
    {
        private static void Prefix( ItemObject item, ref AddItemMode addItemMode )
        {
            if( ItemPickup.Config.PreferBagFirst )
            {
                if( addItemMode == AddItemMode.Default )
                    addItemMode = AddItemMode.ForceBag;
            }
        }
    }

    [HarmonyPatch( typeof( ItemBag ) )]
    [HarmonyPatch( "TryAddItem" )]
    [HarmonyPatch( new Type[] { typeof( ItemObject ), typeof( AddItemMode ) } )]
    internal class ItemBagTryAddItemPatches
    {
        private static void Prefix( ref AddItemMode addItemMode )
        {
            if( ItemPickup.Config.PreferBagFirst )
            {
                if( addItemMode == AddItemMode.Default )
                    addItemMode = AddItemMode.ForceBag;
            }
        }
    }
}
