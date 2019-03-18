using System.Collections.Generic;
using Harmony;
using Pathea.InputSolutionNs;
using UnityEngine;
using UnityModManagerNet;

namespace GiveItem.Patches
{
    [HarmonyPatch( typeof( UnityModManager.UI ) )]
    [HarmonyPatch( "BlockGameUI" )]
    internal class UnityModManagerUIPatches
    {
        private static bool? escMouse = null;
        
        private static void Postfix( bool value )
        {
            if( !value )
            {
                if( escMouse == true )
                {
                    // restore mouse
                    Pathea.ModuleNs.Module<InputSolutionModule>.Self?.Pop();
                    GiveItem.Logger.Log( "Popped InputSolution" );
                    escMouse = false;
                }
            }
            else
            {
                // capture mouse
                Pathea.ModuleNs.Module<InputSolutionModule>.Self?.Push( SolutionType.UIBase );
                GiveItem.Logger.Log( "Pushed InputSolution" );
                escMouse = true;
            }
        }
    }
}
