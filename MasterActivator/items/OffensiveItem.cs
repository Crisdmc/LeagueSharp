using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;

namespace MasterActivator.items
{
    public abstract class OffensiveItem : IItem
    {
        public LeagueSharp.Common.Data.ItemData.Item data { get; set; }
        public string menuName { get; set; }

        public void onGameUpdate(EventArgs args)
        {
            try
            {
                if (LeagueSharp.Common.Menu.GetMenu("MasterActivator", "masterActivator").Item("enabled").GetValue<KeyBind>().Active)
                {
                    if (LeagueSharp.Common.Menu.GetMenu("MasterActivator", "masterActivator").Item("comboModeActive").GetValue<KeyBind>().Active)
                    {
                        if (LeagueSharp.Common.Menu.GetMenu("MasterActivator", "masterActivator").Item(data.Id.ToString()).GetValue<bool>())
                        {
                            if (Items.HasItem(data.Id)) //double checked
                            {
                                if (Items.CanUseItem(data.Id))
                                {
                                    use();
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                Game.PrintChat("Problem with MasterActivator - " + data.Name);
            }
        }

        public abstract void use();
    }
}
