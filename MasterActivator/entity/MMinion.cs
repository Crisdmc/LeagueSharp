using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterActivator.enumerator;

namespace MasterActivator.entity
{
    class MMinion
    {
        public String name { get; set; }
        public String menuName { get; set; }
        public float preX { get; set; }
        public float width { get; set; }

        public MMinion(String name, String menuName, float preX, float width)
        {
            this.name = name;
            this.menuName = menuName;
            this.preX = preX;
            this.width = width;
        }
    }
}
