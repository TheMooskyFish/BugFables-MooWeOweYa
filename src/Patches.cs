using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MooWeOweYa.CustomPartners;
using UnityEngine;

namespace MooWeOweYa;

[HarmonyPatch(typeof(BattleControl))]
internal class Patches
{
    private static int s_holoPos;

    [HarmonyPatch(nameof(BattleControl.StartBattle), MethodType.Enumerator), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> HologramPatch(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions)
            .MatchForward(true,
                new CodeMatch(OpCodes.Ldsfld),
                new CodeMatch(OpCodes.Ldfld),
                new CodeMatch(OpCodes.Ldc_I4_1),
                new CodeMatch(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == "hologram")
            ).Advance(-1).Set(OpCodes.Call, AccessTools.PropertyGetter(typeof(MooConfig), "Hologram"));
        s_holoPos = codeMatcher.Pos;
        return codeMatcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(BattleControl.StartBattle), MethodType.Enumerator), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> PartnersPatch(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions)
            .GoTo(s_holoPos)
            .MatchBack(true, 
                new CodeMatch(OpCodes.Callvirt), 
                new CodeMatch(OpCodes.Ldloc_S), 
                new CodeMatch(OpCodes.Callvirt))
            .Insert(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.ProcessAIPartyList)))
        );
        return codeMatcher.InstructionEnumeration();
    }
    
    [HarmonyPatch(typeof(BattleControl), nameof(BattleControl.AIAttack), MethodType.Enumerator)]
    private static class AIAttackPatch
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
        {
            var codeMatcher = new CodeMatcher(instructions, ilGen);
            var nodamage = instructions.First(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name.StartsWith("<nodamage"));
            var targetid = instructions.First(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name.StartsWith("<targetid"));
            codeMatcher.MatchForward(false,
                new CodeMatch(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name.StartsWith("<aid")),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name.StartsWith("<aid")),
                new CodeMatch(OpCodes.Stloc_2),
                new CodeMatch(OpCodes.Ldloc_2)
            ).Advance(1).InjectYield(
                new CodeInstruction(OpCodes.Ldarg_0),
                nodamage,
                new CodeInstruction(OpCodes.Ldarg_0),
                targetid,
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CorePartners), nameof(CorePartners.Handler)))
            ).InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(CorePartners), nameof(CorePartners.Nodamage))),
                new CodeInstruction(OpCodes.Stfld, nodamage.operand),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(CorePartners), nameof(CorePartners.Targetid))),
                new CodeInstruction(OpCodes.Stfld, targetid.operand)
            );

            if (Application.version != "1.2.0")
                return codeMatcher.InstructionEnumeration();
            
            codeMatcher.End();
            codeMatcher.MatchBack(false,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(nodamage),
                new CodeMatch(OpCodes.Brtrue)
            ).Nopify(0).InjectYield(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utils), nameof(Utils.EnemyDropFix))))
            .Insert(new CodeInstruction(OpCodes.Ldarg_0));
            
            return codeMatcher.InstructionEnumeration();
        }
    }
}
