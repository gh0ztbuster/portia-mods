using System.Collections.Generic;
using System.Reflection.Emit;
using Harmony;
using UModLib.Logging;

namespace CraftMaxDefault.Patches
{
    [HarmonyPatch( typeof( AutomataMachineMenuCtr ) )]
    [HarmonyPatch( "StartAutomata" )]
    internal class AutomataMachineMenuCtrPatches
    {
        enum SearchState
        {
            INITIAL,

            THIS = INITIAL,
            CURITEM,
            ITEMID,
            MIN_0,
            LOCAL_VAR,
            PARAM_0,
            CALL_MAX,
            DFLT_1,
            TXTMGR_ID,

            END
        }

        // Target code (based on v1.1.130102):
        //      UIUtils.ShowNumberSelectMinMax(this.curItem.itemId, 0, Mathf.Max(a, 0), 1, TextMgr.GetStr(100401, -1), new Action<int>(this.StartAutomataResult), (Action) (() => {}), true, (int) totalTime, itemNameStr);
        // Replacement (transpiled) code:
        //      UIUtils.ShowNumberSelectMinMax(this.curItem.itemId, 0, Mathf.Max(a, 0), Mathf.Max(a, 0), TextMgr.GetStr(100401, -1), new Action<int>(this.StartAutomataResult), (Action) (() => {}), true, (int) totalTime, itemNameStr);
        private static IEnumerable<CodeInstruction> Transpiler( IEnumerable<CodeInstruction> instructions )
        {
            SearchState state = SearchState.INITIAL;
            List<CodeInstruction> saved_inst = new List<CodeInstruction>();

            foreach( CodeInstruction inst in instructions )
            {
                if( state != SearchState.DFLT_1 && state != SearchState.TXTMGR_ID )
                    yield return inst;

                switch( state )
                {
                    case SearchState.THIS:
                        if( inst.opcode == OpCodes.Ldarg_0 )
                        {
                            saved_inst.Clear();
                            state++;
                        }
                        break;

                    case SearchState.CURITEM:
                        if( inst.operand?.ToString()?.Contains( "curItem" ) == true )
                        {
                            state++;
                        }
                        else
                        {
                            ULogger.LogTrace( "Fallback from {0} state; opcode={1}", state.ToString( "G" ), inst.opcode.ToString() );
                            state = SearchState.INITIAL;
                        }
                        break;

                    case SearchState.ITEMID:
                        if( inst.operand?.ToString()?.Contains( "itemId" ) == true )
                        {
                            state++;
                        }
                        else
                        {
                            ULogger.LogTrace( "Fallback from {0} state; opcode={1}", state.ToString( "G" ), inst.opcode.ToString() );
                            state = SearchState.INITIAL;
                        }
                        break;

                    case SearchState.MIN_0:
                        if( inst.opcode == OpCodes.Ldc_I4_0 )
                        {
                            state++;
                        }
                        else
                        {
                            ULogger.LogTrace( "Fallback from {0} state; opcode={1}", state.ToString( "G" ), inst.opcode.ToString() );
                            state = SearchState.INITIAL;
                        }
                        break;

                    case SearchState.LOCAL_VAR:
                    case SearchState.PARAM_0:
                        if( inst.opcode.ToString().StartsWith( "Ldloc", true, null ) || inst.opcode == OpCodes.Ldc_I4_0 )
                        {
                            saved_inst.Add( inst );
                            state++;
                        }
                        else
                        {
                            ULogger.LogTrace( "Fallback from {0} state; opcode={1}", state.ToString( "G" ), inst.opcode.ToString() );
                            state = SearchState.INITIAL;
                        }
                        break;

                    case SearchState.CALL_MAX:
                        if( ( inst.opcode == OpCodes.Call || inst.opcode == OpCodes.Callvirt )
                         && ( inst.operand?.ToString()?.Contains( " Max(" ) == true          ) )
                        {
                            saved_inst.Add( inst );
                            state++;
                        }
                        else
                        {
                            ULogger.LogTrace( "Fallback from {0} state; opcode={1}, operand={2}",
                                              state.ToString( "G" ),
                                              inst.opcode.ToString(),
                                              inst.operand?.ToString() );
                            state = SearchState.INITIAL;
                        }
                        break;

                    case SearchState.DFLT_1:
                        if( inst.opcode == OpCodes.Ldc_I4_1 )
                        {
                            state++;
                        }
                        else
                        {
                            ULogger.LogTrace( "Fallback from {0} state; opcode={1}", state.ToString( "G" ), inst.opcode.ToString() );
                            yield return inst;
                            state = SearchState.INITIAL;
                        }
                        break;

                    case SearchState.TXTMGR_ID:
                        var opVal = ( inst.operand as int? );
                        if( opVal == 100401 )
                        {
                            foreach( CodeInstruction newinst in saved_inst )
                                yield return newinst;

                            state++;
                        }
                        else
                        {
                            ULogger.LogTrace( "Fallback from {0} state; operand={1}, opVal={1}",
                                              state.ToString( "G" ),
                                              inst.operand.ToString(),
                                              opVal );
                            state = SearchState.INITIAL;
                        }

                        yield return inst;
                        break;
                }
            }

            if( state != SearchState.END )
                ULogger.LogError( "Unable to patch default crafting number; game or plugin version is outdated!" );
            else
                ULogger.LogTrace( "Successfully patched" );
        }
    }
}
