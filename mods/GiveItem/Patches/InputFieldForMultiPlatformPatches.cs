using System.Collections.Generic;
using Harmony;
using Pathea.UISystemNs;
using TMPro;
using UnityEngine;

namespace GiveItem.Patches
{
    [HarmonyPatch( typeof( InputFieldForMultiPlatform ) )]
    [HarmonyPatch( "EndEditFromInputField" )]
    internal class IFFMP_EndEditFromInputField_Patches
    {
        public static readonly Color guiLblColor = new Color32( 0x80, 0x80, 0x80, 0xFF );
        private static Color? prevColor = null;

        private static void Prefix( TextMeshProUGUI ___buttonText )
        {
            if( !GiveItemGUI2.IsActive )
                return;

            GiveItem.Logger.Log( $"GUI Text input end; button text=\"{___buttonText.text}\"" );

            if( ___buttonText.text == string.Empty )
                ___buttonText.text = GiveItemGUI2.GUI_TXT_LABEL;

            if( ___buttonText.text == GiveItemGUI2.GUI_TXT_LABEL )
            {
                if( prevColor == null )
                    prevColor = ___buttonText.color;

                ___buttonText.color = guiLblColor;
            }
            else
            {
                if( prevColor != null && ___buttonText.color == guiLblColor )
                    ___buttonText.color = ( Color )prevColor;
            }
        }
    }
}
