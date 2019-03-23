using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using UnityEngine;
using UnityModManagerNet;

namespace GiveItem
{
    public static class GiveItem
    {
        public static bool Enabled { get; private set; } = false;
        public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
        public static KeyCode GUIKey { get; private set; } = KeyCode.F9;
        public static bool GUIKeyCtrl { get; private set; } = true;


        public static bool Load( UnityModManager.ModEntry modEntry )
        {
            Logger = modEntry.Logger;
            modEntry.OnGUI = GiveItemGUI.OnGUI;
            modEntry.OnUpdate = OnUpdate;

            HarmonyInstance harmony = HarmonyInstance.Create( modEntry.Info.Id );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );

            return GiveItemGUI2.CanBeActive;
        }

        public static bool OnToggle( UnityModManager.ModEntry modEntry, bool value )
        {
            //TODO: does UMM call OnUpdate() if we're disabled? if not... pointless to store the state
            Enabled = value;

            // "!Enabled || CanBeActive" instead here?
            return GiveItemGUI2.CanBeActive;
        }

        public static void OnUpdate( UnityModManager.ModEntry modEntry, float deltaTime )
        {
            if( !GiveItemGUI2.CanBeActive )
            {
                //TODO: some way to force the red circle status in UMM?
                GiveItem.Logger.Log( "OnUpdate() but CanBeActive false, disabling update proc" );
                modEntry.OnUpdate = null;
                return;
            }

            //TODO: need to switch to checking Event for the key events?
            //      would allow "consuming" (event.Use()) the event to try to prevent its propagation
            if( GiveItemGUI2.IsActive )
            {
                if( Input.GetKeyUp( KeyCode.Escape ) )
                {
                    GiveItemGUI2.CloseGUI();
                }
            }
            else
            {
                if( ( ( !GUIKeyCtrl                          )
                   || ( Input.GetKey( KeyCode.LeftControl )  )
                   || ( Input.GetKey( KeyCode.RightControl ) ) )
                 && ( Input.GetKeyUp( GUIKey )                 ) )
                {
                    GiveItemGUI2.OpenGUI();
                }
            }
        }
    }
}
