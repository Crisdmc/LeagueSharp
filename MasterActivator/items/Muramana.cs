using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace MasterActivator.items
{
    class Muramana : OffensiveItem
    {
        public Muramana(string menuName)
        {
            data = LeagueSharp.Common.Data.ItemData.Muramana;
            this.menuName = menuName;
        }

        public override void use()
        {
 	        Obj_AI_Hero target = TargetSelector.GetTarget(data.Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (!ObjectManager.Player.HasBuff(data.Name))
                {
                    Items.UseItem(data.Id);
                }
            }
            else
            {
                if (ObjectManager.Player.HasBuff(data.Name))
                {
                    Items.UseItem(data.Id);
                }
            }
        }
    }
}