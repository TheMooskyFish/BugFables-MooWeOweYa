using System.Collections;
using UnityEngine;

namespace MooWeOweYa.CustomPartners;

public static class MrTesterPartner
{
    public static IEnumerator MrTester(EntityControl entity)
    {
        entity.rigid.interpolation = RigidbodyInterpolation.Interpolate;
        var helperboostdmg = MainManager.BadgeHowManyEquipped(90);
        var oldpos = entity.transform.position;
        var battle = MainManager.battle;
        var available = battle.GetTargetList([BattleControl.BattlePosition.Ground]);
        if (available.Length == 0)
            yield break;
        CorePartners.Targetid = [available[Random.Range(0, available.Length)]];
        battle.DoDamage(null, ref battle.enemydata[CorePartners.Targetid[0]], 1 + helperboostdmg, null);
        yield return new WaitForSeconds(0.35f);
    }
}
