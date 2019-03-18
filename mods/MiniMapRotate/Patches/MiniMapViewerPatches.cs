using System.Collections.Generic;
using Harmony;
using Pathea.CameraSystemNs;
using Pathea.UISystemNs;
using UModLib.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace MiniMapRotate.Patches
{
    [HarmonyPatch( typeof( MiniMapViewer ) )]
    [HarmonyPatch( "PlayerPosInMiniMap" )]
    internal class MiniMapViewerPatches
    {
        private static void Postfix( Image ___playerIcon, bool ___rotateMap )
        {
            if( ___playerIcon == null )
            {
                ULogger.LogTrace( "playerIcon is null, skipping" );
                return;
            }
            if( CameraManager.Instance.SourceCamera == null )
            {
                ULogger.LogTrace( "SourceCamera is null, skipping" );
                return;
            }

            if( !___rotateMap )
            {
                float camRotation = CameraManager.Instance.SourceTransform.eulerAngles.y;
                ___playerIcon.rectTransform.localEulerAngles = new Vector3( 0.0f, 0.0f, -camRotation );
            }
        }
    }
}
