using System.Collections;
using System.Collections.Generic;
using static MooWeOweYa.MooConfig;

namespace MooWeOweYa;
public static class Utils
{
    public static List<int[]> ProcessAIPartyList(List<int[]> list)
    {
        if (!ToggleMod.Value) 
            return list;
        
        if (DisableAll.Value)
        {
            list.Clear();
            return list;
        }
        // extremely goofy ifs
        if (!Mun.Value)
            list.RemoveAll(i => i[0] == 72);
        if (!GenEri.Value)
            list.RemoveAll(i => i[0] == 47);
        if (!Maki.Value)
            list.RemoveAll(i => i[0] == 46);
        if (!Levi.Value)
            list.RemoveAll(i => i[0] == 3);
        if (!Celia.Value)
            list.RemoveAll(i => i[0] == 49);
        if (!AntCaptain.Value)
            list.RemoveAll(i => i[0] == 95);
        if (!Madeleine.Value)
            list.RemoveAll(i => i[0] == 76);
        if (!Zasp.Value)
            list.RemoveAll(i => i[0] == 20);

        CustomPartners.CorePartners.AddCustom(list);
        
        return list;
    }

    public static IEnumerator EnemyDropFix()
    {
        var battle = MainManager.battle;
        battle.startdrop = true;
        while (battle.EnemyDropping())
            yield return null;
        battle.startdrop = false;
    }
}
