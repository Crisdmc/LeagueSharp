using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using MasterActivator.entity;
using MasterActivator.enumerator;
using SharpDX;

namespace MasterActivator
{
    internal class MActivator
    {
        private Menu Config;
        private Obj_AI_Hero _player;
        private int playerHit;
        private bool gotHit  = false;
        TargetSelector ts = new TargetSelector(600, TargetSelector.TargetingMode.AutoPriority);

        // leagueoflegends.wikia.com/
        MItem qss = new MItem("Quicksilver Sash", "QSS", "qss", 3140, ItemTypeId.Purifier);
        MItem mercurial = new MItem("Mercurial Scimitar", "Mercurial", "mercurial", 3139, ItemTypeId.Purifier);
        MItem bilgewater = new MItem("Bilgewater Cutlass", "Bilgewater", "bilge", 3144, ItemTypeId.Offensive, 450);
        MItem king = new MItem("Blade of the Ruined King", "BoRKing", "king", 3153, ItemTypeId.Offensive, 450);
        MItem youmus = new MItem("Youmuu's Ghostblade", "Youmuu's", "youmus", 3142, ItemTypeId.Offensive);
        MItem tiamat = new MItem("Tiamat", "Tiamat", "tiamat", 3077, ItemTypeId.Offensive, 400);
        MItem hydra = new MItem("Ravenous Hydra", "Hydra", "hydra", 3074, ItemTypeId.Offensive, 400);
        MItem dfg = new MItem("Deathfire Grasp", "DFG", "dfg", 3128, ItemTypeId.Offensive, 750);
        MItem divine = new MItem("Sword of the Divine", "SoDivine", "divine", 3131, ItemTypeId.Offensive);
        MItem hextech = new MItem("Hextech Gunblade", "Hextech", "hextech", 3146, ItemTypeId.Offensive, 700);
        MItem muramana = new MItem("Muramana", "Muramana", "muramana", 3042, ItemTypeId.Buff);
        MItem seraph = new MItem("Seraph's Embrace", "Seraph's", "seraph", 3040, ItemTypeId.Deffensive);
        MItem zhonya = new MItem("Zhonya's Hourglass", "Zhonya's", "zhonya", 3157, ItemTypeId.Deffensive);
        //Item banner = new Item("Randuin's Omen", "Randuin's", "randuin", 3143, 500);
        //Item banner = new Item("Banner of Command", "BoCommand", "banner", 3060); // falta range
        MItem mountain = new MItem("Face of the Mountain", "FoMountain", "mountain", 3401, ItemTypeId.Deffensive, 700); // falta range
        //Item frost = new Item("Frost Queen's Claim", "Frost Queen's", "frost", 3092, 850);
        MItem solari = new MItem("Locket of the Iron Solari", "Solari", "solari", 3190, ItemTypeId.Deffensive, 600);
        MItem mikael = new MItem("Mikael's Crucible", "Mikael's", "mikael", 3222, ItemTypeId.Purifier, 750);
        //Item talisman = new Item("Talisman of Ascension", "Talisman", "talisman", 3069, 600);
        //Item shadows = new Item("Twin Shadows", "Shadows", "shadows", 3023, 750); //2700
        //Item ohmwrecker = new Item("Ohmwrecker", "Ohmwrecker", "ohmwrecker", 3056, 775); // tower atk range Utility.UnderTurret
        MItem hpPot = new MItem("Health Potion", "HP Pot", "hpPot", 2003, ItemTypeId.HPRegenerator);
        MItem manaPot = new MItem("Mana Potion", "Mana Pot", "manaPot", 2004, ItemTypeId.ManaRegenerator);
        MItem biscuit = new MItem("Total Biscuit of Rejuvenation", "Biscuit", "biscuit", 2010, ItemTypeId.HPRegenerator);

        // Heal prioritizes the allied champion closest to the cursor at the time the ability is cast.
        // If no allied champions are near the cursor, Heal will target the most wounded allied champion in range.
        MItem heal = new MItem("Heal", "Heal", "SummonerHeal", 0, ItemTypeId.DeffensiveSpell, 700); // 300? www.gamefaqs.com/pc/954437-league-of-legends/wiki/3-1-summoner-spells
        MItem barrier = new MItem("Barrier", "Barrier", "SummonerBarrier", 0, ItemTypeId.DeffensiveSpell);
        MItem cleanse = new MItem("Cleanse", "Cleanse", "SummonerBoost", 0, ItemTypeId.PurifierSpell);
        MItem clarity = new MItem("Clarity", "Clarity", "SummonerMana", 0, ItemTypeId.ManaRegeneratorSpell, 600);
        MItem ignite = new MItem("Ignite", "Ignite", "SummonerDot", 0, ItemTypeId.OffensiveSpell, 600);
        MItem smite = new MItem("Smite", "Active", "SummonerSmite", 0, ItemTypeId.OffensiveSpell, 750);
        //SummonerExhaust  

        public MActivator()
        {
            CustomEvents.Game.OnGameLoad += onLoad;
        }

        private void onLoad(EventArgs args)
        {
            // Boas vindas
            Game.PrintChat("<font color='#3BB9FF'>MasterActivator - by Crisdmc - </font>Loaded");

            try
            {
                _player = ObjectManager.Player;
                createMenu();

                LeagueSharp.Drawing.OnDraw += onDraw;
                Game.OnGameUpdate += onGameUpdate;
                Game.OnGameProcessPacket += onGameProcessPacket;
            }
            catch
            {
                Game.PrintChat("MasterActivator error creating menu!");
            }
        }

        private void onDraw(EventArgs args)
        {
            try
            {
                if (Config.Item("dSmite").GetValue<bool>())
                {
                    string[] jungleMinions = { "AncientGolem", "GreatWraith", "Wraith", "LizardElder", "Golem", "Worm", "Dragon", "GiantWolf"};

                    var minions = MinionManager.GetMinions(_player.Position, 1500, MinionTypes.All, MinionTeam.Neutral);
                    if (minions.Count() > 0)
                    {
                        foreach (Obj_AI_Base minion in minions)
                        {
                            if (minion.IsHPBarRendered && !minion.IsDead &&
                               (jungleMinions.Any(name => minion.Name.StartsWith(name) && Config.Item(name).GetValue<bool>() && Config.Item("justAS").GetValue<bool>()) ||
                                jungleMinions.Any(name => minion.Name.StartsWith(name) && !Config.Item("justAS").GetValue<bool>())))
                            {
                                Vector2 hpBarPos = minion.HPBarPosition;
                                hpBarPos.X += 45;
                                hpBarPos.Y += 18;
                                int smiteDmg = getSmiteDmg();
                                var damagePercent = smiteDmg / minion.MaxHealth;
                                float hpXPos = hpBarPos.X + (63 * damagePercent);

                                Drawing.DrawLine(hpXPos, hpBarPos.Y, hpXPos, hpBarPos.Y + 5, 2, smiteDmg > minion.Health ? System.Drawing.Color.Lime : System.Drawing.Color.WhiteSmoke);
                            }
                        }
                    }
                }
            }
            catch
            {
                Game.PrintChat("Problem with MasterActivator(Drawing).");
            }
        }
        
        private void onGameProcessPacket(GamePacketEventArgs args)
        {
            byte[] packet = args.PacketData;

            // Added hit detection usesing packets yay
            // Added the checks in checkAndUse && teamCheckAndUse 
            if (packet[0] == Packet.S2C.Damage.Header)
            {
                Packet.S2C.Damage.Struct damage = Packet.S2C.Damage.Decoded(args.PacketData);
                var source = damage.SourceNetworkId;
                var target = damage.TargetNetworkId;

                playerHit = target;
                gotHit = true;
                
            }
        }
        
        private void onGameUpdate(EventArgs args)
        {
            if (Config.Item("enabled").GetValue<bool>())
            {
                try
                {
                    checkAndUse(clarity);

                    checkAndUse(cleanse);
                    checkAndUse(qss);
                    checkAndUse(mercurial);

                    checkAndUse(manaPot, "FlaskOfCrystalWater");
                    checkAndUse(hpPot, "RegenerationPotion");
                    checkAndUse(biscuit, "ItemMiniRegenPotion");

                    // TODO: get heal debuff name, verify a way to use when receiving damage.
                    int enemysInRange = Utility.CountEnemysInRange(700);
                    if (enemysInRange >= 1)
                    {
                        checkAndUse(barrier);
                        checkAndUse(seraph);
                        checkAndUse(zhonya);
                    }

                    teamCheckAndUse(heal);
                    teamCheckAndUse(solari, "", true);
                    teamCheckAndUse(mountain);
                    teamCheckAndUse(mikael);

                    checkAndUse(smite);

                    if (Config.Item("comboModeActive").GetValue<KeyBind>().Active)
                    {
                        combo();
                    }
                }
                catch
                {
                    Game.PrintChat("MasterActivator presented a problem, and has been disabled!");
                    Config.Item("enabled").SetValue<bool>(false);
                }
            }
        }

        private void combo()
        {
            checkAndUse(ignite);
            checkAndUse(youmus);
            checkAndUse(bilgewater);
            checkAndUse(king);
            checkAndUse(tiamat);
            checkAndUse(hydra);
            checkAndUse(dfg);
            checkAndUse(divine);
            checkAndUse(hextech);
            checkAndUse(muramana);
        }

        private bool checkBuff(String name)
        {
            var searchedBuff = from buff in _player.Buffs
                              where buff.Name == name
                             select buff;

            return searchedBuff.Count() <= 0 ? false : true;
        }

        private void createMenuItem(MItem item, int defaultValue, String parent, bool mana = false, bool useOn = true)
        {
            Config.SubMenu(parent).AddItem(new MenuItem(item.menuVariable, item.menuName)).SetValue(true);
            if (useOn)
            {
                Config.SubMenu(parent).AddItem(new MenuItem(item.menuVariable + "UseOnPercent", "Use on " + (mana == false ? "%HP" : "%Mana"))).SetValue(new Slider(defaultValue, 0, 100));
            }
        }

        private void teamCheckAndUse(MItem item, String buff = "", bool self = false)
        {
            if (Config.Item(item.menuVariable).GetValue<bool>())
            {
                if (item.type == ItemTypeId.DeffensiveSpell || item.type == ItemTypeId.ManaRegeneratorSpell || item.type == ItemTypeId.PurifierSpell)
                {
                    var spellSlot = Utility.GetSpellSlot(_player, item.menuVariable);
                    if (spellSlot != SpellSlot.Unknown)
                    {
                        var activeAllyHeros = getActiveAllyHeros(item);
                        if (activeAllyHeros.Count() > 0)
                        {
                            int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;

                            foreach (Obj_AI_Hero hero in activeAllyHeros)
                            {
                                int enemyInRange = Utility.CountEnemysInRange(700, hero);
                                if (enemyInRange >= 1)
                                {
                                    int actualHeroHpPercent = (int)((hero.Health / hero.MaxHealth) * 100);
                                    int actualHeroManaPercent = (int)((_player.Mana / _player.MaxMana) * 100);

                                    if ((item.type == ItemTypeId.DeffensiveSpell && actualHeroHpPercent <= usePercent && playerHit == hero.NetworkId && gotHit) ||
                                         (item.type == ItemTypeId.ManaRegeneratorSpell && actualHeroManaPercent <= usePercent))
                                    {
                                        _player.SummonerSpellbook.CastSpell(spellSlot);
                                        gotHit = false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Items.HasItem(item.id))
                    {
                        if (Items.CanUseItem(item.id))
                        {
                            var activeAllyHeros = getActiveAllyHeros(item);
                            if (activeAllyHeros.Count() > 0)
                            {
                                foreach (Obj_AI_Hero hero in activeAllyHeros)
                                {
                                    if (item.type == ItemTypeId.Purifier)
                                    {
                                        if ((Config.Item("defJustOnCombo").GetValue<bool>() && Config.Item("comboModeActive").GetValue<KeyBind>().Active) ||
                                        (!Config.Item("defJustOnCombo").GetValue<bool>()))
                                        {
                                            if (checkCC(hero))
                                            {
                                                useItem(item.id, hero);
                                            }
                                        }
                                    }
                                    else if (item.type == ItemTypeId.Deffensive)
                                    {
                                        int enemyInRange = Utility.CountEnemysInRange(700, hero);
                                        if (enemyInRange >= 1)
                                        {
                                            int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;
                                            int actualHeroHpPercent = (int)((hero.Health / hero.MaxHealth) * 100);
                                            if (actualHeroHpPercent <= usePercent && gotHit)
                                            {
                                                if (self && playerHit == _player.NetworkId)
                                                {
                                                    useItem(item.id);
                                                    gotHit = false;
                                                }
                                                else if ( playerHit == hero.NetworkId)
                                                {
                                                    useItem(item.id, hero);
                                                    gotHit = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<Obj_AI_Hero> getActiveAllyHeros(MItem item)
        {
            var activeAllyHeros = from hero in ObjectManager.Get<Obj_AI_Hero>()
                                 where hero.IsAlly == true &&
                                       Config.Item(hero.SkinName).GetValue<bool>() &&
                                       hero.Distance(_player) <= item.range &&
                                       !hero.IsDead
                                select hero;

            return activeAllyHeros;
        }

        private void checkAndUse(MItem item, String buff = "")
        {
            if (Config.Item(item.menuVariable).GetValue<bool>())
            {
                int actualHeroHpPercent = (int)((_player.Health / _player.MaxHealth) * 100);
                int actualHeroManaPercent = (int)((_player.Mana / _player.MaxMana) * 100);

                if ( item.type == ItemTypeId.DeffensiveSpell || item.type == ItemTypeId.ManaRegeneratorSpell || item.type == ItemTypeId.PurifierSpell || item.type == ItemTypeId.OffensiveSpell)
                {
                    var spellSlot = Utility.GetSpellSlot(_player, item.menuVariable);
                    if (spellSlot != SpellSlot.Unknown)
                    {
                        if (_player.SummonerSpellbook.CanUseSpell(spellSlot) == SpellState.Ready)
                        {
                            if (item.type == ItemTypeId.DeffensiveSpell)
                            {
                                int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;
                                if (actualHeroHpPercent <= usePercent && playerHit == _player.NetworkId && gotHit)
                                {
                                    _player.SummonerSpellbook.CastSpell(spellSlot);
                                    gotHit = false;
                                }
                            }
                            else if (item.type == ItemTypeId.ManaRegeneratorSpell)
                            {
                                int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;
                                if (actualHeroManaPercent <= usePercent)
                                {
                                    _player.SummonerSpellbook.CastSpell(spellSlot);
                                }
                            }
                            else if (item.type == ItemTypeId.PurifierSpell)
                            {
                                if ((Config.Item("defJustOnCombo").GetValue<bool>() && Config.Item("comboModeActive").GetValue<KeyBind>().Active) ||
                                    (!Config.Item("defJustOnCombo").GetValue<bool>()))
                                {
                                    if (checkCC(_player))
                                    {
                                        _player.SummonerSpellbook.CastSpell(spellSlot);
                                    }
                                }
                            }
                            else if (item.type == ItemTypeId.OffensiveSpell)
                            {
                                if (item == ignite)
                                {
                                    ts.SetRange(item.range);
                                    ts.SetTargetingMode(TargetSelector.TargetingMode.LowHP);
                                    Obj_AI_Hero target = ts.Target;
                                    if (target != null)
                                    {
                                        
                                        var aaspeed = _player.AttackSpeedMod;
                                        float aadmg = 0;

                                        // attack speed checks
                                        if (aaspeed < 0.8f)
                                            aadmg = _player.FlatPhysicalDamageMod * 3;
                                        else if (aaspeed > 1f && aaspeed < 1.3f)
                                            aadmg = _player.FlatPhysicalDamageMod * 5;
                                        else if (aaspeed > 1.3f && aaspeed < 1.5f)
                                            aadmg = _player.FlatPhysicalDamageMod * 7;
                                        else if (aaspeed > 1.5f && aaspeed < 1.7f)
                                            aadmg = _player.FlatPhysicalDamageMod * 9;
                                        else if (aaspeed > 2.0f)
                                            aadmg = _player.FlatPhysicalDamageMod * 11;

                                        // Will calculate for base hp regen, currenthp, etc
                                        float dmg = (_player.Level * 20) + 50;
                                        float regenpersec = (target.FlatHPRegenMod + (target.HPRegenRate * target.Level));
                                        float dmgafter = (dmg - ((regenpersec * 5) / 2));

                                        float aaleft = (dmgafter + target.Health / _player.FlatPhysicalDamageMod);
                                        //var pScreen = Drawing.WorldToScreen(target.Position);

                                        if (target.Health < (dmgafter + aadmg) && _player.Distance(target) <= item.range)
                                        {
                                            bool overIgnite = Config.Item("overIgnite").GetValue<bool>();
                                            if ((!overIgnite && !target.HasBuff("summonerdot")) || overIgnite)
                                            {
                                                _player.SummonerSpellbook.CastSpell(spellSlot, target);
                                                //Drawing.DrawText(pScreen[0], pScreen[1], System.Drawing.Color.Crimson, "Kill in " + aaleft);
                                            }
                                        
                                        }

                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        string[] jungleMinions = { "AncientGolem", "GreatWraith", "Wraith", "LizardElder", "Golem", "Worm", "Dragon", "GiantWolf" };

                                        var minions = MinionManager.GetMinions(_player.Position, item.range, MinionTypes.All, MinionTeam.Neutral);
                                        if (minions.Count() > 0)
                                        {
                                            int smiteDmg = getSmiteDmg();
                                            foreach (Obj_AI_Base minion in minions)
                                            {
                                                if (minion.Health <= smiteDmg && jungleMinions.Any(name => minion.Name.StartsWith(name) && Config.Item(name).GetValue<bool>()))
                                                {
                                                    _player.SummonerSpellbook.CastSpell(spellSlot, minion);
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        Game.PrintChat("Problem with MasterActivator(Smite).");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Items.HasItem(item.id))
                    {
                        if (Items.CanUseItem(item.id))
                        {
                            if (item.type == ItemTypeId.Offensive)
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
                            else if (item.type == ItemTypeId.HPRegenerator || item.type == ItemTypeId.Deffensive)
                            {
                                if (checkUsePercent(item, actualHeroHpPercent))
                                {
                                    if ((buff != "" && !checkBuff(buff)) || buff == "")
                                    {
                                        useItem(item.id);
                                    }
                                }
                            }
                            else if (item.type == ItemTypeId.ManaRegenerator)
                            {
                                if (checkUsePercent(item, actualHeroManaPercent))
                                {
                                    if ((buff != "" && !checkBuff(buff)) || buff == "")
                                    {
                                        useItem(item.id);
                                    }
                                }
                            }
                            else if (item.type == ItemTypeId.Buff)
                            {
                                if (!checkBuff(item.name))
                                {
                                    useItem(item.id);
                                }
                            }
                            else if (item.type == ItemTypeId.Purifier)
                            {
                                if ((Config.Item("defJustOnCombo").GetValue<bool>() && Config.Item("comboModeActive").GetValue<KeyBind>().Active) ||
                                    (!Config.Item("defJustOnCombo").GetValue<bool>()))
                                {
                                    if (checkCC(_player))
                                    {
                                        useItem(item.id);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void useItem(int id, Obj_AI_Hero target = null)
        {
            if (Items.HasItem(id) && Items.CanUseItem(id))
            {
                Items.UseItem(id, target);
            }
        }

        private int getSmiteDmg()
        {
            int level = _player.Level;
            int index = _player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }
        

        private bool checkUsePercent(MItem item, int actualPercent)
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

        private void createMenu()
        {
            Config = new Menu("MActivator", "masterActivator", true);

            Config.AddSubMenu(new Menu("Purifiers", "purifiers"));
            createMenuItem(qss, 100, "purifiers", false, false);
            createMenuItem(mercurial, 100, "purifiers", false, false);
            createMenuItem(cleanse, 100, "purifiers", false, false);
            createMenuItem(mikael, 100, "purifiers", false, false);
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
            Config.SubMenu("purify").AddItem(new MenuItem("silence", "Silence")).SetValue(false);

            Config.AddSubMenu(new Menu("Smite", "smiteCfg"));
            createMenuItem(smite, 100, "smiteCfg", false, false);
            Config.SubMenu("smiteCfg").AddItem(new MenuItem("AncientGolem", "Blue")).SetValue(true);
            Config.SubMenu("smiteCfg").AddItem(new MenuItem("LizardElder", "Red")).SetValue(true);
            Config.SubMenu("smiteCfg").AddItem(new MenuItem("Dragon", "Dragon")).SetValue(true);
            Config.SubMenu("smiteCfg").AddItem(new MenuItem("Worm", "Baron")).SetValue(true);
            Config.SubMenu("smiteCfg").AddItem(new MenuItem("GreatWraith", "GreatWraith")).SetValue(false);
            Config.SubMenu("smiteCfg").AddItem(new MenuItem("Wraith", "Wraith")).SetValue(false);
            Config.SubMenu("smiteCfg").AddItem(new MenuItem("Golem", "Golem")).SetValue(false);
            Config.SubMenu("smiteCfg").AddItem(new MenuItem("GiantWolf", "GiantWolf")).SetValue(false);
            Config.SubMenu("smiteCfg").AddItem(new MenuItem("dSmite", "Draw")).SetValue(true);
            Config.SubMenu("smiteCfg").AddItem(new MenuItem("justAS", "Just ON")).SetValue(false);

            Config.AddSubMenu(new Menu("Offensive", "offensive"));
            createMenuItem(ignite, 100, "offensive", false, false);
            Config.SubMenu("offensive").AddItem(new MenuItem("overIgnite", "Over Ignite")).SetValue(false);
            createMenuItem(youmus, 100, "offensive");
            createMenuItem(bilgewater, 60, "offensive");
            createMenuItem(king, 60, "offensive");
            createMenuItem(tiamat, 90, "offensive");
            createMenuItem(hydra, 90, "offensive");
            createMenuItem(dfg, 80, "offensive");
            createMenuItem(divine, 80, "offensive");
            createMenuItem(hextech, 80, "offensive");
            createMenuItem(muramana, 80, "offensive");

            Config.AddSubMenu(new Menu("Deffensive", "deffensive"));
            createMenuItem(barrier, 35, "deffensive");
            createMenuItem(seraph, 45, "deffensive");
            createMenuItem(zhonya, 35, "deffensive");
            createMenuItem(solari, 45, "deffensive");
            createMenuItem(mountain, 45, "deffensive");

            Config.AddSubMenu(new Menu("Regenerators", "regenerators"));
            createMenuItem(heal, 35, "regenerators");
            //Config.SubMenu("regenerators").AddItem(new MenuItem("useWithHealDebuff", "With debuff")).SetValue(true);
            createMenuItem(clarity, 25, "regenerators", true);
            createMenuItem(hpPot, 55, "regenerators");
            createMenuItem(manaPot, 55, "regenerators", true);
            createMenuItem(biscuit, 55, "regenerators");

            Config.AddSubMenu(new Menu("Team Use", "teamUseOn"));

            var allyHeros = from hero in ObjectManager.Get<Obj_AI_Hero>()
                            where hero.IsAlly == true
                            select hero.SkinName;

            foreach (String allyHero in allyHeros)
            {
                Config.SubMenu("teamUseOn").AddItem(new MenuItem(allyHero, allyHero)).SetValue(true);
            }

            // Combo mode
            Config.AddSubMenu(new Menu("Combo Mode", "combo"));
            Config.SubMenu("combo").AddItem(new MenuItem("comboModeActive", "Active")).SetValue(new KeyBind(32, KeyBindType.Press, true));

            // Target selector
            Config.AddSubMenu(new Menu("Target Selector", "targetSelector"));
            Config.SubMenu("targetSelector").AddItem(new MenuItem("targetMode", "")).SetValue(new StringList(new[] { "LowHP", "MostAD", "MostAP", "Closest", "NearMouse", "AutoPriority", "LessAttack", "LessCast" }, 0));

            Config.AddItem(new MenuItem("enabled", "Enabled")).SetValue(true);

            Config.AddToMainMenu();
        }

        private bool checkCC(Obj_AI_Hero hero)
        {
            bool cc = false;

            if (Config.Item("blind").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Blind))
                {
                    cc = true;
                }
            }

            if (Config.Item("charm").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Charm))
                {
                    cc = true;
                }
            }

            if (Config.Item("fear").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Fear))
                {
                    cc = true;
                }
            }

            if (Config.Item("flee").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Flee))
                {
                    cc = true;
                }
            }

            if (Config.Item("snare").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Snare))
                {
                    cc = true;
                }
            }

            if (Config.Item("taunt").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Taunt))
                {
                    cc = true;
                }
            }

            if (Config.Item("suppression").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Suppression))
                {
                    cc = true;
                }
            }

            if (Config.Item("stun").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Stun))
                {
                    cc = true;
                }
            }

            if (Config.Item("polymorph").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Polymorph))
                {
                    cc = true;
                }
            }

            if (Config.Item("silence").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.Silence))
                {
                    cc = true;
                }
            }

            return cc;
        }
    }
}
