using System;
using System.Collections.Generic;
using System.Linq;
using GiveItem.Patches;
using Pathea;
using Pathea.AudioNs;
using Pathea.InputSolutionNs;
using Pathea.ModuleNs;
using Pathea.ScenarioNs;
using Pathea.UISystemNs;
using UnityModManagerNet;
using UnityEngine;
using UnityEngine.EventSystems;

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
                if( ( Event.current.type == EventType.KeyDown          )
                 && ( ( Event.current.keyCode == KeyCode.Return      )
                   || ( Event.current.keyCode == KeyCode.KeypadEnter ) ) )
                    submit = true;

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
        public const string GUI_TXT_LABEL = "Enter Search Term";

        public static bool CanBeActive { get; private set; } = true;
        public static bool IsActive { get; private set; } = false;

        private const int GUI_OPEN_SFX_ID = 68;
        private static ColorConfigUI searchUI = null;
        private static SearchUICfg searchUICfg;
        private static GameObject lastSelect;

        private static bool initGUI()
        {
            // if GUI is called too early, fail quietly
            if( UIStateComm.Instance == null )
                return false;

            if( searchUI == null )
                searchUI = GameUtils
                            .AddChild( UIStateComm.Instance.UiRoot, "Prefabs/ColorConfigUI", AssetType.UiSystem )
                            .GetComponent<ColorConfigUI>();

            if( searchUI == null )
            {
                GiveItem.Logger.Error( "Can't instantiate a ColorConfigUI child, mod disabled" );
                CanBeActive = false;
                return false;
            }

            if( searchUICfg == null )
            {
                searchUICfg = new SearchUICfg();
            }

            return true;
        }

        public static void OpenGUI()
        {
            if( IsActive || !initGUI() )
                return;

            IsActive = true;
            GiveItem.Logger.Log( "GiveItemGUI2.OpenGUI() !!!" );

            lastSelect = EventSystem.current.currentSelectedGameObject;
            Module<InputSolutionModule>.Self.Push( SolutionType.ColorConfig );
            searchUICfg.SearchTerm = GUI_TXT_LABEL;
            searchUI.gameObject.SetActive( true );
            searchUI.SetTarget( searchUICfg, new Action( OnSearch ), false );
            Module<AudioModule>.Self.PlayEffect2D( GUI_OPEN_SFX_ID, false, true, false );
        }

        public static void OnSearch()
        {
            Module<InputSolutionModule>.Self.Pop();
            EventSystem.current.SetSelectedGameObject( lastSelect );

            if( searchUICfg.SearchTerm == GUI_TXT_LABEL )
            {
                GiveItem.Logger.Log( "User canceled search" );
                CloseGUI();
                return;
            }

            GiveItem.Logger.Log( $"User searched for: \"{searchUICfg.SearchTerm}\"" );
            CloseGUI();
        }

        public static void CloseGUI()
        {
            if( !IsActive )
                return;

            IsActive = false;
            searchUI.gameObject.SetActive( false );

            if( searchUI.enabled )
            {
                GiveItem.Logger.Log( "User is closing GUI, cancel search UI" );
            }
            else
                GiveItem.Logger.Log( "User is closing GUI, search UI already disabled" );
        }

        sealed class SearchUICfg : IColorConfig
        {
            public string SearchTerm { get; set; }

            bool IColorConfig.HasNameEdit => true;

            Color IColorConfig.GetColor( int index ) => Color.white;

            int IColorConfig.GetColorCount() => 0;

            int IColorConfig.GetDyeCost() => 0;

            Renderer[] IColorConfig.GetExtraRenders() => null;

            MatColorConfig IColorConfig.GetMatColorConfig( int index ) => null;

            string IColorConfig.GetName() => this.SearchTerm;

            Vector3 IColorConfig.GetPreviewScale() => Vector3.one;

            Renderer IColorConfig.GetRender() => (Renderer)null;

            void IColorConfig.ResetSetting() {}

            void IColorConfig.SetColor(Color color, int index) {}

            void IColorConfig.SetName( string name ) => this.SearchTerm = name;
        }
    }
}
