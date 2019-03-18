using System;
using System.IO;
using System.Reflection;
using Harmony;
using LitJson;

namespace MiniMapRotate
{
    public class MiniMapRotate
    {
        #region Default Mod Properties
        public static MiniMapRotateConfig Config { get; set; }
        public static string ModDirectoryPath
        {
            get
            {
                var uri = new UriBuilder( Assembly.GetExecutingAssembly().CodeBase );
                return Path.GetDirectoryName( Uri.UnescapeDataString( uri.Path ) );
            }
        }
        public static string GameDirectoryPath => Path.Combine( ModDirectoryPath, @"\..\Portia_Data\" );
        #endregion

        #region Default Mod Entry Point
        public static void Load( JsonData modCfg )
        {
            if( modCfg != null )
            {
                Config = LitJson.JsonMapper.ToObject<MiniMapRotateConfig>( modCfg.ToJson() );
            }
            if( Config == null )
            {
                Config = new MiniMapRotateConfig();
            }

            Assembly modAssembly = Assembly.GetExecutingAssembly();
            HarmonyInstance.Create( modAssembly.GetName().Name ).PatchAll( modAssembly );
        }
        #endregion
    }
}
