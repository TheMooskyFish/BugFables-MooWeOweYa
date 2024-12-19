using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MooWeOweYa.CustomPartners;

public class CorePartners
{
    private delegate IEnumerator Attack(EntityControl entity);
    private class CustomPartner(int animid, Attack attack)
    {
        public readonly int Animid = animid;
        public readonly Attack AttackCallback = attack;
    }
    private static readonly List<CustomPartner> s_partners = [
        new(73, MrTesterPartner.MrTester),
    ];

    public static bool Nodamage;
    public static List<int> Targetid;
    public static IEnumerator Handler(bool nodmg, List<int> id)
    {
        var aiparty = MainManager.battle.aiparty;
        Nodamage = nodmg;
        Targetid = id;
        var attack = s_partners.FirstOrDefault(i => i.Animid == aiparty.animid);
        if (attack is not null)
            yield return attack.AttackCallback(aiparty);
    }
    public static void AddCustom(List<int[]> list)
    {
        if (MooConfig.MrTester.Value)
            list.Add([73, 13]);
    }
}
