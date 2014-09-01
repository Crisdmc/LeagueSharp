using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterYi.entity;
using MasterYi.enumerator;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;

namespace MasterActivator
{
    internal class MActivator
    {
        private Menu Config;
        private Obj_AI_Hero _player = ObjectManager.Player;
        TargetSelector ts = new TargetSelector(600, TargetSelector.TargetingMode.AutoPriority);

        // leagueoflegends.wikia.com/
        Item qss = new Item("Quicksilver Sash", "QSS", "qss", 3140, ItemTypeId.Purifier);
        Item mercurial = new Item("Mercurial Scimitar", "Mercurial", "mercurial", 3139, ItemTypeId.Purifier);
        Item bilgewater = new Item("Bilgewater Cutlass", "Bilgewater", "bilgewater", 3144, ItemTypeId.Offensive, 450);
        Item king = new Item("Blade of the Ruined King", "BoRKing", "king", 3153, ItemTypeId.Offensive, 450);
        Item youmus = new Item("Youmuu's Ghostblade", "Youmuu's", "youmus", 3142, ItemTypeId.Offensive);
        Item tiamat = new Item("Tiamat", "Tiamat", "tiamat", 3077, ItemTypeId.Offensive, 400);
        Item hydra = new Item("Ravenous Hydra", "Hydra", "hydra", 3074, ItemTypeId.Offensive, 400);
        Item dfg = new Item("Deathfire Grasp", "DFG", "dfg", 3128, ItemTypeId.Offensive, 750);
        Item divine = new Item("Sword of the Divine", "SoDivine", "divine", 3131, ItemTypeId.Offensive);
        Item hextech = new Item("Hextech Gunblade", "Hextech", "hextech", 3146, ItemTypeId.Offensive, 700);
        Item muramana = new Item("Muramana", "Muramana", "muramana", 3042, ItemTypeId.Offensive);
        //Item seraph = new Item("Seraph's Embrace", "Seraph's", "seraph", 3040, ItemTypeId.Deffensive);
        //Item banner = new Item("Banner of Command", "BoCommand", "banner", 3060); // falta range
        //Item mountain = new Item("Face of the Mountain", "FoMountain", "mountain", 3401); // falta range
        //Item frost = new Item("Frost Queen's Claim", "Frost Queen's", "frost", 3092, 850);
        //Item solari = new Item("Locket of the Iron Solari", "Solari", "solari", 3190, 600);
        //Item mikael = new Item("Mikael's Crucible", "Mikael's", "mikael", 3222); // falta range
        //Item talisman = new Item("Talisman of Ascension", "Talisman", "talisman", 3069, 600);
        //Item shadows = new Item("Twin Shadows", "Shadows", "shadows", 3023, 750); //2700
        //Item ohmwrecker = new Item("Ohmwrecker", "Ohmwrecker", "ohmwrecker", 3056, 775); // tower atk range Utility.UnderTurret
        Item hpPot = new Item("Health Potion", "HP Pot", "hpPot", 2003, ItemTypeId.HPRegenerator);
        Item manaPot = new Item("Mana Potion", "Mana Pot", "manaPot", 2004, ItemTypeId.ManaRegenerator);
        Item biscuit = new Item("Total Biscuit of Rejuvenation", "Biscuit", "biscuit", 2010, ItemTypeId.HPRegenerator);

        public MActivator()
        {
            // Boas vindas
            Game.PrintChat("<font color='#3BB9FF'>MasterActivator - by Crisdmc - </font>Loaded");

            try
            {
                Config = new Menu("MActivator", "masterActivator", true);

                Config.AddSubMenu(new Menu("Purifiers", "purifiers"));
                Config.SubMenu("purifiers").AddItem(new MenuItem("qss", "QSS")).SetValue(true);
                Config.SubMenu("purifiers").AddItem(new MenuItem("mercurial", "Mercurial")).SetValue(true);
                Config.SubMenu("purifiers").AddItem(new MenuItem("cleanse", "Cleanse")).SetValue(true);
                Config.SubMenu("purifiers").AddItem(new MenuItem("defJustOnCombo", "Just on combo")).SetValue(true);
                
                Config.AddSubMenu(new Menu("Purify", "purify"));
                Config.SubMenu("purify").AddItem(new MenuItem("blind", "Blind")).SetValue(true);
                Config.SubMenu("purify").AddItem(new MenuItem("charm", "Charm")).SetValue(true);
                Config.SubMenu("purify").AddItem(new MenuItem("fear", "Fear")).SetValue(true);
                Config.SubMenu("purify").AddItem(new MenuItem("flee", "Flee")).SetValue(true);
                Config.SubMenu("purify").AddItem(new MenuItem("snare", "Snare")).SetValue(true);
                Config.SubMenu("purify").AddItem(new MenuItem("taunt", "Taunt")).SetValue(true);
                Config.SubMenu("purify").AddItem(new MenuItem("suppression", "Suppression")).SetValue(true);
                Config.SubMenu("purify").AddItem(new MenuItem("stun", "Stun")).SetValue(true);
                Config.SubMenu("purify").AddItem(new MenuItem("polymorph", "Polymorph")).SetValue(false);

                Config.AddSubMenu(new Menu("Off. Itens", "offItens"));
                createMenuItem(youmus, 100, "offItens");
                createMenuItem(bilgewater, 60, "offItens");
                createMenuItem(king, 60, "offItens");
                createMenuItem(tiamat, 90, "offItens");
                createMenuItem(hydra, 90, "offItens");
                createMenuItem(dfg, 80, "offItens");
                createMenuItem(divine, 80, "offItens");
                createMenuItem(hextech, 80, "offItens");
                createMenuItem(muramana, 80, "offItens");

                Config.AddSubMenu(new Menu("Consumables", "consumables"));
                createMenuItem(hpPot, 55, "consumables");
                createMenuItem(manaPot, 55, "consumables", true);
                createMenuItem(biscuit, 55, "consumables");

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

            Game.OnGameUpdate += onGameUpdate;
        }
        
        private void onGameUpdate(EventArgs args)
        {
            if (Config.Item("enabled").GetValue<bool>())
            {
                var cleanseSlot = Utility.GetSpellSlot(_player, "summonerboost"); //w
                if (((Config.Item("defJustOnCombo").GetValue<bool>() && Config.Item("comboModeActive").GetValue<KeyBind>().Active) || 
                     (!Config.Item("defJustOnCombo").GetValue<bool>())) &&
                    ((Items.HasItem(qss.id) || Items.HasItem(mercurial.id)) || ( cleanseSlot != SpellSlot.Unknown)))
                {
                    if (Items.CanUseItem(qss.id) || Items.CanUseItem(mercurial.id) || _player.SummonerSpellbook.CanUseSpell(cleanseSlot) == SpellState.Ready)
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
                            if (Config.Item("cleanse").GetValue<bool>())
                            {
                                _player.SummonerSpellbook.CastSpell(cleanseSlot);
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
                    checkAndUse(hextech);

                    if (Config.Item(muramana.menuVariable).GetValue<bool>() && Items.HasItem(muramana.id))
                    {
                        if (!checkBuff("Muramana"))
                        {
                            checkAndUse(muramana);
                        }
                    }
                }

                if (!checkBuff("FlaskOfCrystalWater"))
                {
                    checkAndUse(manaPot);
                }
                if (!checkBuff("RegenerationPotion"))
                {
                    checkAndUse(hpPot);
                }
                if (!checkBuff("ItemMiniRegenPotion"))
                {
                    checkAndUse(biscuit);
                }
            }
        }

        private bool checkBuff(String name)
        {
            var searchedBuff = from buff in _player.Buffs
                              where buff.Name == name
                             select buff;

            return searchedBuff.Count() <= 0 ? false : true;
        }

        private void createMenuItem(Item item, int defaultValue, String parent, bool mana = false)
        {
            Config.SubMenu(parent).AddItem(new MenuItem(item.menuVariable, item.menuName)).SetValue(true);
            Config.SubMenu(parent).AddItem(new MenuItem(item.menuVariable + "UseOnPercent", "Use on " + (mana == false ? "%HP" : "%Mana"))).SetValue(new Slider(defaultValue, 0, 100));
        }

        private void checkAndUse(Item item)
        {
            try
            {
                if (Config.Item(item.menuVariable).GetValue<bool>() && Items.HasItem(item.id))
                {
                    if (Items.CanUseItem(item.id))
                    {
                        int actualHeroHpPercent = (int)((_player.Health / _player.MaxHealth) * 100);
                        int actualHeroManaPercent = (int)((_player.Mana / _player.MaxMana) * 100);

                        if(item.type == ItemTypeId.Offensive)
                        {
                            if (checkTarget(item.range))
                            {
                                int actualTargetHpPercent = (int)((ts.Target.Health / ts.Target.MaxHealth) * 100);
                                if (checkUsePercent(item, actualTargetHpPercent))
                                {
                                    useItem(item.id, item.range == 0 ? null : ts.Target);
                                }
                            }
                        }
                        else if (item.type == ItemTypeId.HPRegenerator)
                        {
                            if (checkUsePercent(item, actualHeroHpPercent))
                            {
                                useItem(item.id);
                            }
                        }
                        else if (item.type == ItemTypeId.ManaRegenerator)
                        {
                            if (checkUsePercent(item, actualHeroManaPercent))
                            {
                                useItem(item.id);
                            }
                        }
                    }
                }
            }
            catch
            {
                Game.PrintChat("MasterActivator presented a problem, and has been disabled!");
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

        private bool checkUsePercent(Item item, int actualPercent)
        {
            int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;
            return actualPercent <= usePercent ? true : false;
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
