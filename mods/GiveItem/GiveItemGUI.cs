using System;
using System.Collections.Generic;
using System.Linq;
using GiveItem.Patches;
using Pathea;
using Pathea.ModuleNs;
using Pathea.ScenarioNs;
using UnityModManagerNet;
using UnityEngine;

namespace GiveItem
{
    internal static class GiveItemGUI
    {
        private const float FilterUpdtMinTime = 0.5f;
        private const int QtyMax = 100;
        private static readonly GUILayoutOption LblWidth = GUILayout.Width( 300 );
        private static readonly GUILayoutOption ItemWidth = GUILayout.Width( 500 );

        private static Vector2 scrollView = Vector2.zero;
        private static int qty = 1;
        private static string name = String.Empty;
        private static string filtName = String.Empty;
        private static IEnumerable<KeyValuePair<int, string>> items;

        public static void OnGUI( UnityModManager.ModEntry modEntry )
        {
            if( ( Module<ScenarioModule>.Self?.Loading != false                                    )
             || ( Module<ScenarioModule>.Self?.CurrentScenarioName?.StartsWith( "Start" ) != false ) )
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

                bool submit = false;
                if( ( Event.current.type == EventType.KeyUp          )
                 && ( ( Event.current.keyCode == KeyCode.Return      )
                   || ( Event.current.keyCode == KeyCode.KeypadEnter ) ) )
				{
				    Event.current.Use();
                    submit = true;
				}

                name = GUILayout.TextField( name, GUILayout.Width( 200 ) );
                if( GUILayout.Button( "Search", GUILayout.Width( 100 ) ) || submit )
                {
                    name = name.Trim();

                    if( filtName != name )
                    {
                        filtName = name;

                        if( ( filtName == String.Empty                   )
                         || ( filtName.Length == 1 && filtName[0] == '*' ) )
                        {
                            items = ItemDataMgrPatches.ItemDB.AsEnumerable();
                            GiveItem.Logger.Log( "Filter=None" );
                        }
                        else if( filtName.Length > 2 && filtName.Substring( 0, 2 ) == "##" )
                        {
                            try
                            {
                                int itemId = int.Parse( filtName.Substring( 2, filtName.Length - 2 ) );
                                items = ItemDataMgrPatches.ItemDB.Where( item => item.Key == itemId );
                                GiveItem.Logger.Log( $"Filter=ItemId, Term={itemId}" );
                            }
                            catch
                            {
                                items = ItemDataMgrPatches.ItemDB.Where( item => item.Value?.Equals( filtName, StringComparison.InvariantCultureIgnoreCase ) == true );
                                GiveItem.Logger.Log( "Filter=Equals, Term=\"" + filtName + "\"" );
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
            }
            GUILayout.EndHorizontal();

            if( items != null )
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label( "Click to add: ", LblWidth );

                    scrollView = GUILayout.BeginScrollView( scrollView, GUILayout.ExpandWidth( false ) );
                    {
                        foreach( KeyValuePair<int, string> item in items )
                            if( GUILayout.Button( item.Value, ItemWidth ) )
                            {
                                modEntry.Logger.Log( $"Give item, ID={item.Key}, Name={item.Value}" );
                                Module<Player>.Self.bag.AddItem( item.Key, qty, true );
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

    //TODO: Prototype new in-game GUI here.
    //TODO: Remove the UMM's OnGui() logic above and replace with this.
    internal static class GiveItemGUI2
    {

    }
}
