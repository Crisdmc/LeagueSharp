using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace MasterActivator.items
{
    class Frost : OffensiveItem
    {
        public Frost(string menuName)
        {
            data = LeagueSharp.Common.Data.ItemData.Frost_Queens_Claim;
            this.menuName = menuName;
        }

        public override void use()
        {
 	        Obj_AI_Hero target = TargetSelector.GetTarget(data.Range, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                int actualTargetHpPercent = (int)((target.Health / target.MaxHealth) * 100);
                int usePercent = LeagueSharp.Common.Menu.GetMenu("MasterActivator", "masterActivator").Item(data.Id + "UseOnPercent").GetValue<Slider>().Value;

                if (actualTargetHpPercent <= usePercent)
                {
                    // FIX-ME: In frost case, we must check the affected area, not just ppl in range(item).
                    if (Utility.CountEnemiesInRange(ObjectManager.Player, data.Range) >= LeagueSharp.Common.Menu.GetMenu("MasterActivator", "masterActivator").Item(data.Id + "UseXUnits").GetValue<Slider>().Value)
                    {
                        Items.UseItem(data.Id, target);
                    }
                }
            }
        }
    }
}
