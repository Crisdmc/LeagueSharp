using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterYi.enumerator;

namespace MasterYi.entity
{
    class Item
    {
        public String name { get; set; }
        public String menuName { get; set; }
        public String menuVariable { get; set; }
        public int id { get; set; }
        public float range { get; set; }
        public ItemTypeId type { get; set; }

        public Item(String name, String menuName, String menuVariable, int id, ItemTypeId type, float range = 0)
        {
            this.name = name;
            this.menuVariable = menuVariable;
            this.menuName = menuName;
            this.id = id;
            this.range = range;
            this.type = type;
        }
    }
}
