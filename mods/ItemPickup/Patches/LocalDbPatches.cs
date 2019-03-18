using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using UModLib.Logging;

namespace ItemPickup.Patches
{
    [HarmonyPatch( typeof( LocalDb ) )]
    [HarmonyPatch( "Load" )]
    internal class LocalDbLoadPatches
    {
        static bool loaded = false;
        static readonly List<string> validLangs = new List<string> { "English", "Chinese", "German", "T_Chinese", "Italian", "Spanish", "Japanese", "Russian" };

        private static void Postfix()
        {
            var items = ItemPickup.Config.ItemIgnore.Items;
            if( items.Count == 0 || loaded )
                return;

            // Yeah yeah, I know, escaping is bad and I should be using query parameters.
            // I get it. I really do. And normally I would, if I weren't dealing with stupid
            // needless abstraction layers and crap while patching someone else's product.
            // But that's what I'm doing.
            // So I'm not using them.
            // I hope that hurts you deep down inside, Mr. OMGWHYISNTHEUSINGPARAMETERSOMG.
            var method = ItemPickup.Config.ItemIgnore.MatchMethod.ToLower();
            Func<string, string> escape = s => s.Replace( "'", "''" );
            Func<string, string> nameQueryVal;
            switch( method )
            {
                case "exact":
                    nameQueryVal = name => escape( name );
                    break;

                case "contains":
                    nameQueryVal = name => $"%{escape( name )}%";
                    break;

                case "startswith":
                    nameQueryVal = name => escape( name ) + "%";
                    break;

                case "endswith":
                    nameQueryVal = name => "%" + escape( name );
                    break;

                default:
                    ULogger.LogWarn( "Invalid MatchMethod value \"{0}\", defaulting to exact", method );
                    method = "exact";
                    nameQueryVal = name => escape( name );
                    break;
            }
            string nameQueryOp = ( method != "exact" ? "LIKE" : "=" );

            var lang = ItemPickup.Config.ItemIgnore.Language;
            if( !validLangs.Contains( lang, StringComparer.OrdinalIgnoreCase ) )
            {
                ULogger.LogWarn( "Invalid Language value \"{0}\", defaulting to English", lang );
                lang = "English";
            }

            ULogger.LogInfo( "Setting up {0} item ignore filters...", items.Count );

            foreach( var item in items )
            {
                ULogger.LogTrace( "Processing ignore string \"{0}\"", nameQueryVal( item ) );

                var query = $@"
                    SELECT
                        t.ID Trans_Id, t.{lang}, p.Props_Id, p.Item_Type
                    FROM
                        Translation_hint t
                        INNER JOIN Props_total_table p
                            ON t.ID = p.Props_Name
                    WHERE
                        t.{lang} {nameQueryOp} '{nameQueryVal( item )}'";
                SqliteDataReader reader = LocalDb.cur.ExecuteQuery( query );

                int num = 0;
                while( reader.Read() )
                {
                    var trans_id  = reader.GetInt32( 0 );
                    var item_name = reader.GetString( 1 );
                    var item_id   = reader.GetInt32( 2 );
                    var item_type = reader.GetString( 3 );

                    ULogger.LogTrace( "Item matched; name=\"{0}\", type=\"{1}\", id={2}, tran_id={3}",
                                      item_name, item_type, item_id, trans_id );
                    IgnoreItems.allMatchedIDs.Add( item_id );
                    num++;
                }

                if( num == 0 )
                    ULogger.LogInfo( "Ignore item value \"{0}\" didn't match any items (check spelling?)", item );
            }

            ULogger.LogInfo( "Done setting up! There were {0} items matched and will be ignored.",
                             IgnoreItems.allMatchedIDs.Count );
        }
    }
}
