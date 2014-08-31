using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;

namespace MasterYi
{
    class Activator
    {
        private Menu Config;
        private Obj_AI_Hero _player = ObjectManager.Player;

        int qss = 3140;
        int king = 3151;
        int mercurial = 3139;

        public Activator()
        {
            // Boas vindas
            Game.PrintChat("<font color='#3BB9FF'>LazyActivator - by Crisdmc - </font>Loaded");

            try
            {
                Config = new Menu("YiActivator", "YiActivator", true);

                Config.AddSubMenu(new Menu("Def. Itens", "defItens"));
                Config.SubMenu("defItens").AddItem(new MenuItem("qss", "QSS")).SetValue(true);
                Config.SubMenu("defItens").AddItem(new MenuItem("mercurial", "Mercurial")).SetValue(true);
                
                Config.AddSubMenu(new Menu("Clear", "clear"));
                Config.SubMenu("clear").AddItem(new MenuItem("blind", "Blind")).SetValue(true);
                Config.SubMenu("clear").AddItem(new MenuItem("charm", "Charm")).SetValue(true);
                Config.SubMenu("clear").AddItem(new MenuItem("fear", "Fear")).SetValue(true);
                Config.SubMenu("clear").AddItem(new MenuItem("flee", "Flee")).SetValue(true);
                Config.SubMenu("clear").AddItem(new MenuItem("snare", "Snare")).SetValue(true);
                Config.SubMenu("clear").AddItem(new MenuItem("taunt", "Taunt")).SetValue(true);
                Config.SubMenu("clear").AddItem(new MenuItem("suppression", "Suppression")).SetValue(true);
                Config.SubMenu("clear").AddItem(new MenuItem("stun", "Stun")).SetValue(true);
                Config.SubMenu("clear").AddItem(new MenuItem("polymorph", "Polymorph")).SetValue(false);

                Config.AddItem(new MenuItem("enabled", "Enabled")).SetValue(true);

                Config.AddToMainMenu();
            }
            catch
            {
                Game.PrintChat("LazyActivator error creating menu!");
            }

            Game.OnGameUpdate += OnGameUpdate;
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Config.Item("enabled").GetValue<bool>())
            {
                if (checkCC())
                {
                    if (Config.Item("qss").GetValue<bool>())
                    {
                        useItem(qss);
                    }
                    if (Config.Item("mercurial").GetValue<bool>())
                    {
                        useItem(mercurial);
                    }
                }
            }
        }

        private void useItem(int id)
        {
            if (Items.HasItem(id) && Items.CanUseItem(id))
            {
                Items.UseItem(id);
            }
        }

        private bool checkCC()
        {
            bool cc = false;

            if (Config.Item("blind").GetValue<bool>())
            {
                if (_player.HasBuffOfType(BuffType.Blind))
                {
                    cc = true;
                }
            }

            if (Config.Item("charm").GetValue<bool>())
            {
                if (_player.HasBuffOfType(BuffType.Charm))
                {
                    cc = true;
                }
            }

            if (Config.Item("fear").GetValue<bool>())
            {
                if (_player.HasBuffOfType(BuffType.Fear))
                {
                    cc = true;
                }
            }

            if (Config.Item("flee").GetValue<bool>())
            {
                if (_player.HasBuffOfType(BuffType.Flee))
                {
                    cc = true;
                }
            }

            if (Config.Item("snare").GetValue<bool>())
            {
                if (_player.HasBuffOfType(BuffType.Snare))
                {
                    cc = true;
                }
            }

            if (Config.Item("taunt").GetValue<bool>())
            {
                if (_player.HasBuffOfType(BuffType.Taunt))
                {
                    cc = true;
                }
            }

            if (Config.Item("suppression").GetValue<bool>())
            {
                if (_player.HasBuffOfType(BuffType.Suppression))
                {
                    cc = true;
                }
            }

            if (Config.Item("stun").GetValue<bool>())
            {
                if (_player.HasBuffOfType(BuffType.Stun))
                {
                    cc = true;
                }
            }

            if (Config.Item("polymorph").GetValue<bool>())
            {
                if (_player.HasBuffOfType(BuffType.Polymorph))
                {
                    cc = true;
                }
            }

            return cc;
        }
    }
}
