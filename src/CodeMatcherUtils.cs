using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MooWeOweYa;

internal static class CodeMatcherUtils
{
    public static CodeMatcher Nopify(this CodeMatcher codeMatcher, int num)
    {
        foreach (var _ in codeMatcher.InstructionsWithOffsets(0, num))
        {
            codeMatcher.SetAndAdvance(OpCodes.Nop, null);
        }
        return codeMatcher;
    }

    public static CodeMatcher GoTo(this CodeMatcher codeMatcher, int pos)
    {
        AccessTools.PropertySetter(typeof(CodeMatcher), "Pos").Invoke(codeMatcher, [pos]);
        return codeMatcher;
    }

    public static void SetAt(this CodeMatcher codeMatcher, int pos, CodeInstruction inst)
    {
        var oldPos = codeMatcher.Pos;
        codeMatcher.GoTo(pos)
            .SetInstruction(inst)
            .GoTo(oldPos);
    }
    private class InstructionLabels
    {
        public int _switchPos;
        public List<Label> _list = [];
    }
    private static InstructionLabels ExtractLabels(CodeMatcher codeMatcher)
    {
        codeMatcher = codeMatcher.Clone();
        InstructionLabels labels = new();
        codeMatcher.Start().MatchForward(false, new CodeMatch(OpCodes.Switch));
        labels._switchPos = codeMatcher.Pos;
        labels._list = (codeMatcher.Operand as Label[])!.ToList();
        return labels;
    }
        
    public static CodeMatcher InjectYield(this CodeMatcher codeMatcher, params CodeInstruction[] instructions)
    {
        var current = codeMatcher.Instructions().First(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name.EndsWith("_current"));
        var state = codeMatcher.Instructions().First(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name.EndsWith("_state"));
        var labels = ExtractLabels(codeMatcher);
        codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(instructions)
            .InsertAndAdvance(current)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4, labels._list.Count),
                state,
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Ret),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_M1),
                state
            ).Advance(-3).CreateLabel(out var label);
        labels._list.Add(label);
        codeMatcher.SetAt(labels._switchPos, new CodeInstruction(OpCodes.Switch, labels._list.ToArray()));
        codeMatcher.Advance(3);
        return codeMatcher;
    }
}
