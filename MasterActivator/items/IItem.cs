using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterActivator.items
{
    interface IItem
    {
        LeagueSharp.Common.Data.ItemData.Item data { get; set; }
        string menuName { get; set; }
        void onGameUpdate(EventArgs args);
    }
}
