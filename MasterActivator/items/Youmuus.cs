﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace MasterActivator.items
{
    class Youmuus : OffensiveItem
    {
        public Youmuus(string menuName)
        {
            data = LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade;
            this.menuName = menuName;
        }

        public override void use()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(data.Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                int actualTargetHpPercent = (int)((target.Health / target.MaxHealth) * 100);
                int usePercent = LeagueSharp.Common.Menu.GetMenu("MasterActivator", "masterActivator").Item(data.Id + "UseOnPercent").GetValue<Slider>().Value;

                if (actualTargetHpPercent <= usePercent)
                {
                    Items.UseItem(data.Id); //self = null
                }
            }
        }
    }
}
