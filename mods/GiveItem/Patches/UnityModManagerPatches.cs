using System.Collections.Generic;
using Harmony;
using Pathea;
using Pathea.AudioNs;
using Pathea.InputSolutionNs;
using Pathea.ModuleNs;
using Pathea.UISystemNs;
using UnityModManagerNet;

namespace GiveItem.Patches
{
    [HarmonyPatch( typeof( UnityModManager.UI ) )]
    [HarmonyPatch( "BlockGameUI" )]
    internal static class UnityModManagerUIPatches
    {
        private static readonly BoolTrue trueLogic = new BoolTrue();
        private static bool? escMouse = null;
        private static bool prevVis;
        
        private static void Postfix( bool value )
        {
            if( !value )
            {
                if( escMouse == true )
                {
                    // restore mouse, resume audio effects, resume game
                    GiveItem.Logger.Log( "Restoring mouse state, removing time pauser" );
                    UIStateComm.Instance?.SetCursor( prevVis );
                    Module<InputSolutionModule>.Self?.RemoverDisable( trueLogic );
                    AudioPlayer.Self?.ResumeEffect2D();
                    AudioPlayer.Self?.ResumeEffect3D();
                    UIStateComm.Instance?.ResumeGame( trueLogic );
                    escMouse = false;
                }
            }
            else
            {
                prevVis = UIStateComm.Instance?.cursorVisiable != false;

                // capture mouse, pause audio effects, pause game
                GiveItem.Logger.Log( "Overriding mouse state, adding time pauser" );
                UIStateComm.Instance?.SetCursor( true );
                Module<InputSolutionModule>.Self?.AddDisable( trueLogic );
                AudioPlayer.Self?.PauseEffect2D();
                AudioPlayer.Self?.PauseEffect3D();
                UIStateComm.Instance?.PauseGame( trueLogic );
                escMouse = true;
            }
        }
    }
}
