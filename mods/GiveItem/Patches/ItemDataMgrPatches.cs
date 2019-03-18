using System.Collections.Generic;
using Harmony;
using Pathea.ItemSystem;
using UnityEngine;

namespace GiveItem.Patches
{
    [HarmonyPatch( typeof( ItemDataMgr ) )]
    [HarmonyPatch( "OnLoad" )]
    internal class ItemDataMgrPatches
    {
        public static Dictionary<int, string> ItemDB { get; private set; }

        private static void Postfix( List<ItemBaseConfData> ___itemBaseList )
        {
            GiveItem.Logger.Log( "Loading items DB" );

            ItemDataMgrPatches.ItemDB = new Dictionary<int, string>();

            foreach( var item in ___itemBaseList )
                ItemDataMgrPatches.ItemDB.Add( item.ID, TextMgr.GetStr( item.NameID ) );

            GiveItem.Logger.Log( $"Loaded {ItemDataMgrPatches.ItemDB.Count} items!" );
        }
    }
}
