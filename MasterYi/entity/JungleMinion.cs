using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;

namespace MasterYi.entity
{
    class JungleMinion
    {
        public String name { get; set; }
        public bool isDead { get; set; }
        public GameObject minion { get; set; }


        public JungleMinion(String name)
        {
            this.name = name;
        }
    }
}
