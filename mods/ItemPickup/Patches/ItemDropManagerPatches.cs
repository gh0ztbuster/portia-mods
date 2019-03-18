using System;
using System.Collections.Generic;
using Harmony;
using Pathea.ItemDropNs;
using Pathea.ItemSystem;
using UModLib.Logging;
using UnityEngine;

namespace ItemPickup.Patches
{
    //TODO: stop ignored items at creation:
    //  ItemDropManager.DropItemInternal()
    //  ItemDropManager.DropItemListToPlayerBag
    //      alternative: ItemBag.AddItem()
    [HarmonyPatch( typeof( ItemDropManager ) )]
    [HarmonyPatch( "DropItemInternal" )]
    [HarmonyPatch( new Type[] { typeof( Vector3 ),
                                typeof( ItemObject ),
                                typeof( bool ),
                                typeof( string ),
                                typeof( string ),
                                typeof( bool ),
                                typeof( float ),
                                typeof( bool ) } )]
    internal class ItemDropManagerPatches
    {
        private static bool Prefix( ItemObject item, ref ItemDrop __result )
        {
            if( IgnoreItems.allMatchedIDs.Contains( item.ItemDataId ) )
            {
                ULogger.LogTrace( "Dropped item ignored; ID={0}, name={1}",
                                  item.ItemDataId, item.ItemBase.Name );
                __result = ( ItemDrop )null;
                return false;
            }

            return true;
        }
    }

    //NOTE: ItemDropManager.DropItemListToPlayerBag() doesn't use DropItemInternal(), but
    //      it's being left unpatched for now as it is currently only used when gifts are
    //      sent, and that seems infrequent and outside the desired scope of this mod.

    internal class IgnoreItems
    {
        public static List<int> allMatchedIDs = new List<int>();
    }
}
