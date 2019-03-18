using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GiveItem.Patches;
using Harmony;
using Pathea;
using Pathea.ScenarioNs;
using UnityModManagerNet;
using UnityEngine;

namespace GiveItem
{
    public static class GiveItem
    {
        #region Default Mod Properties
        public static bool enabled = false;
        public static UnityModManager.ModEntry.ModLogger Logger;
        #endregion

        //TODO: move into GiveItemGUI
        // GUI constants
        private const float FilterUpdtMinTime = 0.5f;
        private const int QtyMax = 100;
        private static readonly GUILayoutOption LblWidth = GUILayout.Width( 300 );
        private static readonly GUILayoutOption ItemWidth = GUILayout.Width( 500 );

        //TODO: move into GiveItemGUI
        // GUI data
        private static Vector2 scrollView = Vector2.zero;
        private static int qty = 1;
        private static string name = String.Empty;
        private static string filtName = String.Empty;
        private static IEnumerable<KeyValuePair<int, string>> items;

        #region Default Mod Entry Points
        public static bool Load( UnityModManager.ModEntry modEntry )
        {
            GiveItem.Logger = modEntry.Logger;

            var harmony = HarmonyInstance.Create( modEntry.Info.Id );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );

            modEntry.OnGUI = GiveItem.OnGUI;

            return true;
        }

        public static bool OnToggle( UnityModManager.ModEntry modEntry, bool value )
        {
            GiveItem.enabled = value;

            return true;
        }
        #endregion

        //TODO: move into GiveItemGUI
        public static void OnGUI( UnityModManager.ModEntry modEntry )
        {
            if( ( Pathea.ModuleNs.Module<ScenarioModule>.Self?.Loading != false                                    )
             || ( Pathea.ModuleNs.Module<ScenarioModule>.Self?.CurrentScenarioName?.StartsWith( "Start" ) != false ) )
            {
                GUILayout.Label( "ERROR: You must be loaded into a game to use this menu." );
                return;
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label( "Quantity: ", LblWidth );

                qty = ( int )Math.Round( Mathf.Clamp( float.Parse( GUILayout.TextField( qty.ToString(), GUILayout.Width( 50 ) ) ), 1.0f, ( float )QtyMax ) );
                qty = Mathf.RoundToInt( GUILayout.HorizontalSlider( qty, 1.0f, 100.0f, GUILayout.Width( 200 ) ) );
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label( "Item Name: ", LblWidth );

                name = GUILayout.TextField( name, GUILayout.Width( 200 ) ).Trim();
                if( ( GUILayout.Button( "Search", GUILayout.Width( 100 ) ) )
                 || ( Input.GetKeyUp( KeyCode.Return )                     )
                 || ( Input.GetKeyUp( KeyCode.KeypadEnter )                ) )
                {
                    filtName = name;
                    if( filtName == String.Empty )
                        items = ItemDataMgrPatches.ItemDB.AsEnumerable();
                    else if( filtName.Substring( 0, 2 ) == "##" && filtName.Length > 2 )
                    {
                        try
                        {
                            items = ItemDataMgrPatches.ItemDB.Where( item => item.Key == int.Parse( filtName.Substring( 2, filtName.Length - 1 ) ) );
                        }
                        catch
                        {
                            items = ItemDataMgrPatches.ItemDB.Where( item => item.Value?.Equals( filtName, StringComparison.InvariantCultureIgnoreCase ) == true );
                        }
                    }
                    else if( filtName[0] == '*' && filtName[filtName.Length - 1] == '*' )
                    {
                        items = ItemDataMgrPatches.ItemDB.Where( item => item.Value?.IndexOf( filtName.Substring( 1, filtName.Length - 2 ), StringComparison.InvariantCultureIgnoreCase ) >= 0 );
                        GiveItem.Logger.Log( "Filter=IndexOf, Term=\"" + filtName.Substring( 1, filtName.Length - 2 ) + "\"" );
                    }
                    else if( filtName[0] != '*' && filtName[filtName.Length - 1] == '*' )
                    {
                        items = ItemDataMgrPatches.ItemDB.Where( item => item.Value?.StartsWith( filtName.Substring( 0, filtName.Length - 1 ), StringComparison.InvariantCultureIgnoreCase ) == true );
                        GiveItem.Logger.Log( "Filter=StartsWith, Term=\"" + filtName.Substring( 0, filtName.Length - 1 ) + "\"" );
                    }
                    else if( filtName[0] == '*' && filtName[filtName.Length - 1] != '*' )
                    {
                        items = ItemDataMgrPatches.ItemDB.Where( item => item.Value?.EndsWith( filtName.Substring( 1 ), StringComparison.InvariantCultureIgnoreCase ) == true );
                        GiveItem.Logger.Log( "Filter=EndsWith, Term=\"" + filtName.Substring( 1 ) + "\"" );
                    }
                    else
                    {
                        items = ItemDataMgrPatches.ItemDB.Where( item => item.Value?.Equals( filtName, StringComparison.InvariantCultureIgnoreCase ) == true );
                        GiveItem.Logger.Log( "Filter=Equals, Term=\"" + filtName + "\"" );
                    }
                }
            }
            GUILayout.EndHorizontal();

            if( items != null )
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label( "Click to add: ", LblWidth );

                    GiveItem.scrollView = GUILayout.BeginScrollView( GiveItem.scrollView, GUILayout.ExpandWidth( false ) );
                    {
                        foreach( KeyValuePair<int, string> item in items )
                            if( GUILayout.Button( item.Value, ItemWidth ) )
                            {
                                modEntry.Logger.Log( $"Give item, ID={item.Key}, Name={item.Value}" );
                                Pathea.ModuleNs.Module<Player>.Self.bag.AddItem( item.Key, qty, true );
                            }
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    int numItems = items.Count();

                    GUILayout.Label( "Results: ", LblWidth );
                    GUILayout.Label( $"{numItems} item{(numItems != 1 ? "s" : "")} found" );
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
