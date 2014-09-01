using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterYi.entity;

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
        TargetSelector ts = new TargetSelector(600, TargetSelector.TargetingMode.AutoPriority);

        // leagueoflegends.wikia.com/
        Item qss = new Item("Quicksilver Sash", "QSS", "qss", 3140);
        Item mercurial = new Item("Mercurial Scimitar", "Mercurial", "mercurial", 3139);
        Item bilgewater = new Item("Bilgewater Cutlass", "Bilgewater", "bilgewater", 3144, 450);
        Item king = new Item("Blade of the Ruined King", "BoRKing", "king", 3153, 450);
        Item youmus = new Item("Youmuu's Ghostblade", "Youmuu's", "youmus", 3142);
        Item tiamat = new Item("Tiamat", "Tiamat", "tiamat", 3077, 400);
        Item hydra = new Item("Ravenous Hydra", "Hydra", "hydra", 3074, 400);
        Item dfg = new Item("Deathfire Grasp", "DFG", "dfg", 3128, 750);
        Item divine = new Item("Sword of the Divine", "SoDivine", "divine", 3131);
        Item hextech = new Item("Hextech Gunblade", "Hextech", "hextech", 3146, 700);
        Item muramana = new Item("Muramana", "Muramana", "muramana", 3042);
        Item seraph = new Item("Seraph's Embrace", "Seraph's", "seraph", 3040);
        Item banner = new Item("Banner of Command", "BoCommand", "banner", 3060); // falta range
        Item reaver = new Item("Essence Reaver", "Reaver", "reaver", 3508);
        Item mountain = new Item("Face of the Mountain", "FoMountain", "mountain", 3401); // falta range
        Item frost = new Item("Frost Queen's Claim", "Frost Queen's", "frost", 3092, 850);
        Item solari = new Item("Locket of the Iron Solari", "Solari", "solari", 3190, 600);
        Item mikael = new Item("Mikael's Crucible", "Mikael's", "mikael", 3222); // falta range
        Item talisman = new Item("Talisman of Ascension", "Talisman", "talisman", 3069, 600);
        Item shadows = new Item("Twin Shadows", "Shadows", "shadows", 3023, 750); //2700
        Item ohmwrecker = new Item("Ohmwrecker", "Ohmwrecker", "ohmwrecker", 3056, 775); // tower atk range

        public Activator()
        {
            // Boas vindas
            Game.PrintChat("<font color='#3BB9FF'>YiActivator - by Crisdmc - </font>Loaded");

            try
            {
                Config = new Menu("YiActivator", "YiActivator", true);

                Config.AddSubMenu(new Menu("Def. Itens", "defItens"));
                Config.SubMenu("defItens").AddItem(new MenuItem("qss", "QSS")).SetValue(true);
                Config.SubMenu("defItens").AddItem(new MenuItem("mercurial", "Mercurial")).SetValue(true);
                Config.SubMenu("defItens").AddItem(new MenuItem("defJustOnCombo", "Just on combo")).SetValue(true);
                
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

                Config.AddSubMenu(new Menu("Off. Itens", "offItens"));
                createMenuOffItem(youmus, 100);
                createMenuOffItem(bilgewater, 60);
                createMenuOffItem(king, 60);
                createMenuOffItem(tiamat, 90);
                createMenuOffItem(hydra, 90);
                createMenuOffItem(dfg, 80);
                createMenuOffItem(divine, 80);

                // Combo mode
                Config.AddSubMenu(new Menu("Combo Mode", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("comboModeActive", "Active")).SetValue(new KeyBind(32, KeyBindType.Press, true));

                // Target selector
                Config.AddSubMenu(new Menu("Target Selector", "targetSelector"));
                Config.SubMenu("targetSelector").AddItem(new MenuItem("targetMode", "")).SetValue(new StringList(new[] { "LowHP", "MostAD", "MostAP", "Closest", "NearMouse", "AutoPriority", "LessAttack", "LessCast" }, 0));
                
                Config.AddItem(new MenuItem("enabled", "Enabled")).SetValue(true);

                Config.AddToMainMenu();
            }
            catch
            {
                Game.PrintChat("YiActivator error creating menu!");
            }

            Game.OnGameUpdate += OnGameUpdate;
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (Config.Item("enabled").GetValue<bool>())
            {
                if (((Config.Item("defJustOnCombo").GetValue<bool>() && Config.Item("comboModeActive").GetValue<KeyBind>().Active) || 
                     (!Config.Item("defJustOnCombo").GetValue<bool>())) &&
                    (Items.HasItem(qss.id) || Items.HasItem(mercurial.id)))
                {
                    if (Items.CanUseItem(qss.id) || Items.CanUseItem(mercurial.id))
                    {
                        if (checkCC())
                        {
                            if (Config.Item(qss.menuVariable).GetValue<bool>())
                            {
                                useItem(qss.id);
                            }
                            if (Config.Item(mercurial.menuVariable).GetValue<bool>())
                            {
                                useItem(mercurial.id);
                            }
                        }
                    }
                }

                if (Config.Item("comboModeActive").GetValue<KeyBind>().Active)
                {
                    checkAndUse(youmus);
                    checkAndUse(bilgewater);
                    checkAndUse(king);
                    checkAndUse(tiamat);
                    checkAndUse(hydra);
                    checkAndUse(dfg);
                    checkAndUse(divine);
                }
            }
        }

        private void createMenuOffItem(Item item, int defaultHP)
        {
            Config.SubMenu("offItens").AddItem(new MenuItem(item.menuVariable, item.menuName)).SetValue(true);
            Config.SubMenu("offItens").AddItem(new MenuItem(item.menuVariable + "UseOnHp", "Use on %HP")).SetValue(new Slider(defaultHP, 0, 100));
        }

        private void checkAndUse(Item item)
        {
            try
            {
                if (Config.Item(item.menuVariable).GetValue<bool>() && Items.HasItem(item.id))
                {
                    if (Items.CanUseItem(item.id))
                    {
                        if (checkTarget(item.range))
                        {
                            int hpPercent = Config.Item(item.menuVariable + "UseOnHp").GetValue<Slider>().Value;
                            if (((int)((ts.Target.Health / ts.Target.MaxHealth) * 100)) <= hpPercent)
                            {
                                useItem(item.id, item.range == 0 ? null : ts.Target);
                            }
                        }
                    }
                }
            }
            catch
            {
                Game.PrintChat("YiActivator presented a problem, and has been disabled!");
                Config.Item("enabled").SetValue<bool>(false);
            }
        }

        private void useItem(int id, Obj_AI_Hero target = null)
        {
            if (Items.HasItem(id) && Items.CanUseItem(id))
            {
                Items.UseItem(id, target);
            }
        }

        private bool checkTarget(float range)
        {
            if (range == 0)
            {
                range = _player.AttackRange + 125;
            }

            int targetModeSelectedIndex = Config.Item("targetMode").GetValue<StringList>().SelectedIndex;
            TargetSelector.TargetingMode targetModeSelected = new TargetSelector.TargetingMode();

            foreach (TargetSelector.TargetingMode targetMode in Enum.GetValues(typeof(TargetSelector.TargetingMode)))
            {
                int index = (int)targetMode;
                if (index == targetModeSelectedIndex)
                {
                    targetModeSelected = targetMode;
                }
            }
            
            ts.SetRange(range);
            ts.SetTargetingMode(targetModeSelected);

            return ts.Target != null ? true : false;
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
