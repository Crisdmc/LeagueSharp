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
        private Obj_AI_Hero target;
        
        // leagueoflegends.wikia.com/
        MItem qss = new MItem("Quicksilver Sash", "QSS", "qss", 3140, ItemTypeId.Purifier);
        MItem mercurial = new MItem("ItemMercurial", "Mercurial", "mercurial", 3139, ItemTypeId.Purifier);
        MItem bilgewater = new MItem("BilgewaterCutlass", "Bilgewater", "bilge", 3144, ItemTypeId.Offensive, 450);
        MItem king = new MItem("ItemSwordOfFeastAndFamine", "BoRKing", "king", 3153, ItemTypeId.Offensive, 450);
        MItem youmus = new MItem("Youmuu's Ghostblade", "Youmuu's", "youmus", 3142, ItemTypeId.Offensive);
        MItem tiamat = new MItem("ItemTiamatCleave", "Tiamat", "tiamat", 3077, ItemTypeId.Offensive, 400);
        MItem hydra = new MItem("Ravenous Hydra", "Hydra", "hydra", 3074, ItemTypeId.Offensive, 400);
        MItem dfg = new MItem("DeathfireGrasp", "DFG", "dfg", 3128, ItemTypeId.Offensive, 750);
        MItem divine = new MItem("ItemSoTD", "SoDivine", "divine", 3131, ItemTypeId.Offensive); //Sword of the Divine
        MItem hextech = new MItem("Hextech Gunblade", "Hextech", "hextech", 3146, ItemTypeId.Offensive, 700);
        MItem muramana = new MItem("Muramana", "Muramana", "muramana", 3042, ItemTypeId.Buff);
        MItem seraph = new MItem("ItemSeraphsEmbrace", "Seraph's", "seraph", 3040, ItemTypeId.Deffensive);
        MItem zhonya = new MItem("Zhonya's Hourglass", "Zhonya's", "zhonya", 3157, ItemTypeId.Deffensive);
        MItem randuin = new MItem("RanduinsOmen", "Randuin's", "randuin", 3143, ItemTypeId.OffensiveAOE, 500);
        //Item banner = new Item("Banner of Command", "BoCommand", "banner", 3060); // falta range
        MItem mountain = new MItem("Face of the Mountain", "FoMountain", "mountain", 3401, ItemTypeId.Deffensive, 700); // falta range
        MItem frost = new MItem("Frost Queen's Claim", "Frost Queen's", "frost", 3092, ItemTypeId.OffensiveAOE, 850);
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
        //MItem exhaust = new MItem("Exhaust", "Exhaust", "SummonerExhaust", 0, ItemTypeId.OffensiveSpell, ???);
        MItem barrier = new MItem("Barrier", "Barrier", "SummonerBarrier", 0, ItemTypeId.DeffensiveSpell);
        MItem cleanse = new MItem("Cleanse", "Cleanse", "SummonerBoost", 0, ItemTypeId.PurifierSpell);
        MItem clarity = new MItem("Clarity", "Clarity", "SummonerMana", 0, ItemTypeId.ManaRegeneratorSpell, 600);
        MItem ignite = new MItem("Ignite", "Ignite", "SummonerDot", 0, ItemTypeId.OffensiveSpell, 600);
        MItem smite = new MItem("Smite", "Smite", "SummonerSmite", 0, ItemTypeId.OffensiveSpell, 750);
        MItem smiteAOE = new MItem("SmiteAOE", "smite AOE", "itemsmiteaoe", 0, ItemTypeId.OffensiveSpell, 750);
        MItem smiteDuel = new MItem("SmiteDuel", "smite Duel", "s5_summonersmiteduel", 0, ItemTypeId.OffensiveSpell, 750);
        MItem smiteQuick = new MItem("SmiteQuick", "smite Quick", "s5_summonersmitequick", 0, ItemTypeId.OffensiveSpell, 750);
        MItem smiteGanker = new MItem("SmiteGanker", "smite Ganker", "s5_summonersmiteplayerganker", 0, ItemTypeId.OffensiveSpell, 750);

        // Auto shields 
        MItem blackshield = new MItem("BlackShield", "Black Shield", "bShield", 0, ItemTypeId.TeamAbility, 750, SpellSlot.E); //Morgana
        MItem unbreakable = new MItem("BraumE", "Unbreakable", "unbreak", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.E);
        MItem palecascade = new MItem("DianaOrbs", "Pale Cascade", "cascade", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);
        MItem bulwark = new MItem("GalioBulwark", "Bulwark", "bulwark", 0, ItemTypeId.TeamAbility, 800, SpellSlot.W);
        MItem courage = new MItem("GarenW", "Courage", "courage", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);
        MItem eyeofstorm = new MItem("EyeOfTheStorm", "Eye of the Storm", "storm", 0, ItemTypeId.Ability, 800, SpellSlot.E); //Janna
        MItem inspire = new MItem("KarmaSolKimShield", "Inspire", "inspire", 0, ItemTypeId.TeamAbility, 800, SpellSlot.E);
        MItem helppix = new MItem("LuluE", "Help Pix!", "pix", 0, ItemTypeId.TeamAbility, 650, SpellSlot.E);
        MItem prismaticbarrier = new MItem("LuxPrismaticWave", "Prismatic Barrier", "pBarrier", 0, ItemTypeId.TeamAbility, 1075, SpellSlot.W);
        MItem titanswraith = new MItem("NautilusPiercingGaze", "Titans Wraith", "tWraith", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);
        MItem commandprotect = new MItem("OrianaRedactCommand", "Command Protect", "cProt", 0, ItemTypeId.TeamAbility, 1100, SpellSlot.E);
        MItem feint = new MItem("ShenFeint", "Feint", "feint", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W); // ?
        MItem spellshield = new MItem("SivirE", "SpellShield", "sShield", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.E);
        MItem nocturneShield = new MItem("NocturneShroudOfDarkness", "Noct. Shield", "nShield", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);
        MItem yasuoShield = new MItem("YasuoWMovingWall", "Yasuo Shield", "yShield", 0, ItemTypeId.TeamAbilityAOE, 400, SpellSlot.W);
        MItem fioraRiposte = new MItem("FioraRiposte", "Fiora Riposte", "fRiposte", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);// S2 fiora
        MItem tryndaUlt = new MItem("UndyIngrage", "Trynda Ult.", "tIngrage", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.R);// Trynda mia ult
        MItem nasusUlt = new MItem("NasusR", "Nasus Ult.", "nasusR", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.R);
        MItem renekUlt = new MItem("RenektonReignOfTheTyrant", "Renek Ult.", "renekR", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.R); //Renek nek
        MItem leonaW = new MItem("LeonaSolarBarrier", "Leona Barrier", "leonaW", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);
        MItem annieE = new MItem("MoltenShield", "Annie Barrier", "annieE", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.E); // Annie
        MItem vladW = new MItem("VladimirSanguinePool", "Vlad. Pool", "vladW", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W); // nigga VladImir W
        MItem wukongW = new MItem("MonkeyKingDecoy", "Wu. Decoy", "wuW", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);

        //jaxcounterstrike E Jax 20%?
        //judicatorintervention R Kayle 30%?
        // lee W, blindmonkwone(targ), blindmonkwtwo(self-l-steal/mv) 700 range?
        //lissandrar Liss R 20%?
        //obduracy malp W
        //defensiveballcurl rammus W
        // rivenfeint rivelina E
        //rumbleshield rumble W crap 
        //shenfeint W
        // skarnerexoskeleton skarner W
        //yorickravenous Yorik E, taget enemy
        //udyrturtlestance Udyr runner guy W
        // urgotterrorcapacitoractive2 Urgot W never played
        // chronoshift Zilean R NAHH
        

        //poppyparagonofdemacia poppy W def
        //gp heal
        //alistouro heal
        // master meditate
        //primalsurge nida E range 600
        // sonaariaofperseverance sona W range 1000
        // soraka wish R global - astralblessing W 550 !self
        // imbue taric Q

        // Jungle Minions
        MMinion blue = new MMinion("SRU_Blue", "Blue", 6, 143);
        MMinion red = new MMinion("SRU_Red", "Red", 6, 143);
        MMinion dragon = new MMinion("SRU_Dragon", "Dragon", 6, 143);
        MMinion baron = new MMinion("SRU_Baron", "Baron", -18, 192);
        MMinion wolf = new MMinion("SRU_Murkwolf", "Murkwolf", 41, 74);
        MMinion razor = new MMinion("SRU_Razorbeak", "Razor", 39, 74); // Ghosts
        MMinion krug = new MMinion("SRU_Krug", "Krug", 38, 80);
        MMinion crab = new MMinion("Sru_Crab", "Crab", 43, 62);
        MMinion gromp = new MMinion("SRU_Gromp", "Gromp", 32, 87); // Ghost

        MMinion tVilemaw = new MMinion("TT_Spiderboss", "Vilemaw", 45, 67);
        MMinion tWraith = new MMinion("TT_NWraith", "Wraith", 45, 67);
        MMinion tGolem = new MMinion("TT_NGolem", "Golem", 45, 67);
        MMinion tWolf = new MMinion("TT_NWolf", "Wolf", 45, 67);

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
                Obj_AI_Base.OnProcessSpellCast += onProcessSpellCast;
            }
            catch
            {
                Game.PrintChat("MasterActivator error creating menu!");
            }
        }

        private void onProcessSpellCast(Obj_AI_Base attacker, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                double incDmg = 0;
                SpellSlot spellSlot = SpellSlot.Unknown;
                GameObject spellTarget = args.Target;

                if (Config.Item("enabled").GetValue<KeyBind>().Active)
                {
                    if (Config.Item("predict").GetValue<bool>())
                    {
                        if (spellTarget != null) // Check (spell w/o target) AOE etc?
                        {
                            // Self target && attacker.IsEnemy 
                            //if (attacker.Type == GameObjectType.obj_AI_Hero && attacker.NetworkId == spellTarget.NetworkId)
                            //{
                                //Console.WriteLine("Target Name2-> " + spellTarget.Name + "  Spell->" + args.SData.Name + "   SpellTT->" + args.SData.SpellTotalTime);
                            //}

                            //Config.Item(hero.SkinName).GetValue<bool>()
                            // 750 from greater range(mikael).
                            if (attacker.Type == GameObjectType.obj_AI_Hero && attacker.IsEnemy && spellTarget.Type == GameObjectType.obj_AI_Hero && (spellTarget.IsMe || (spellTarget.IsAlly && _player.Distance(spellTarget.Position) <= 750)))
                            {
                                Obj_AI_Hero attackerHero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == attacker.NetworkId);
                                Obj_AI_Hero attackedHero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == spellTarget.NetworkId);

                                SpellDataInst spellA = attacker.Spellbook.Spells.FirstOrDefault(hero=> args.SData.Name.Contains(hero.SData.Name));
                               
                                spellSlot = spellA == null ? SpellSlot.Unknown : spellA.Slot;
                                SpellSlot igniteSlot = Utility.GetSpellSlot(attackerHero, ignite.menuVariable);

                                if (igniteSlot != SpellSlot.Unknown && spellSlot == igniteSlot)
                                {
                                    incDmg = Damage.GetSummonerSpellDamage(attackerHero, attackedHero, Damage.SummonerSpell.Ignite);
                                }

                                else if (spellSlot == SpellSlot.Item1 || spellSlot == SpellSlot.Item2 || spellSlot == SpellSlot.Item3 || spellSlot == SpellSlot.Item4 || spellSlot == SpellSlot.Item5 || spellSlot == SpellSlot.Item6)
                                {
                                    if (args.SData.Name == king.name)
                                    {
                                        incDmg = Damage.GetItemDamage(attackerHero, attackedHero, Damage.DamageItems.Botrk);
                                    }
                                    else if (args.SData.Name == bilgewater.name)
                                    {
                                        incDmg = Damage.GetItemDamage(attackerHero, attackedHero, Damage.DamageItems.Bilgewater);
                                    }
                                    else if (args.SData.Name == dfg.name)
                                    {
                                        incDmg = Damage.GetItemDamage(attackerHero, attackedHero, Damage.DamageItems.Dfg);
                                    }
                                    else if (args.SData.Name == hydra.name)
                                    {
                                        incDmg = Damage.GetItemDamage(attackerHero, attackedHero, Damage.DamageItems.Hydra);
                                    }
                                }
                                else if (spellSlot == SpellSlot.Unknown)
                                {
                                    incDmg = Damage.GetAutoAttackDamage(attackerHero, attackedHero, true);
                                }
                                else
                                {
                                    incDmg = Damage.GetSpellDamage(attackerHero, attackedHero, spellSlot);
                                }

                                //Console.WriteLine("Slot->" + spellSlot + "  inc-> " + incDmg + " Spell-> " + args.SData.Name);// 44 = sivir w, 49 = YasuoBasicAttack3, 50 YassuoCritAttack, 45 = LeonaShieldOfDaybreakAttack
                            }
                            else if (attacker.Type == GameObjectType.obj_AI_Turret && attacker.IsEnemy && spellTarget.Type == GameObjectType.obj_AI_Hero && (spellTarget.IsAlly && _player.Distance(spellTarget.Position) <= 750))
                            {
                                // TODO: Get multiplier/real dmg
                                incDmg = attacker.BaseAttackDamage;
                                Obj_AI_Hero attackedHero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == spellTarget.NetworkId);
                            }
                        }
                        // w/o target
                        else
                        {
                            // Self target && attacker.IsEnemy 
                            if (attacker.Type == GameObjectType.obj_AI_Hero && attacker.IsEnemy)
                            {
                                float range1 = args.SData.CastRangeDisplayOverride.FirstOrDefault(s => s > 0);
                                float range2 = args.SData.CastRange.FirstOrDefault();
                                float range = range1 != null ? range1 : range2;

                                //drawPos2 = args.Start.Extend(args.End, range);
                                if (args.Start.Distance(_player.Position) <= range)
                                {
                                    // ponto fake
                                    Vector3 fakePoint = args.Start.Extend(args.End, args.Start.Distance(_player.Position));

                                    if (_player.Position.Distance(fakePoint) <= 30)
                                    {
                                        SpellDataInst spellA = attacker.Spellbook.Spells.FirstOrDefault(hero => args.SData.Name.Contains(hero.SData.Name));
                                        spellSlot = spellA == null ? SpellSlot.Unknown : spellA.Slot;

                                        spellTarget = _player;
                                    }
                                }
                            }
                        }
                    }

                    if (incDmg > 0 || spellSlot != SpellSlot.Unknown)
                    {
                        if (spellTarget.Team == _player.Team)
                        {
                            Obj_AI_Hero attackedHero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == spellTarget.NetworkId);

                            teamCheckAndUse(heal, Config.Item("useWithHealDebuff").GetValue<bool>() ? "" : "summonerhealcheck", false, incDmg);
                            teamCheckAndUse(solari, "", true, incDmg);
                            teamCheckAndUse(mountain, "", false, incDmg);
                            checkAndUseShield(incDmg, attacker, attackedHero, spellSlot);

                            if (spellTarget.IsMe)
                            {
                                checkAndUse(zhonya, "", incDmg);
                                checkAndUse(barrier, "", incDmg);
                                checkAndUse(seraph, "", incDmg);
                            }
                        }
                    }
                }
            }
            catch
            {
                Game.PrintChat("Problem with MasterActivator(Receiving dmg sys.).");
            }
        }

        private void onDraw(EventArgs args)
        {
            try
            {
                if (Config.Item("dSmite").GetValue<bool>())
                {
                    MMinion[] jungleMinions;
                    if (Utility.Map.GetMap().Type.Equals(Utility.Map.MapType.TwistedTreeline))
                    {
                        jungleMinions = new MMinion[] { tVilemaw, tWraith, tWolf, tGolem };
                    }
                    else
                    {
                        jungleMinions = new MMinion[] { blue, red, razor, baron, krug, wolf, dragon, gromp, crab };
                    }

                    var minions = MinionManager.GetMinions(_player.Position, 1500, MinionTypes.All, MinionTeam.Neutral);
                    if (minions.Count() > 0)
                    {
                        foreach (Obj_AI_Base minion in minions)
                        {
                            if (minion.IsHPBarRendered && !minion.IsDead)
                            {
                                foreach (MMinion jMinion in jungleMinions)
                                {
                                    if (minion.Name.StartsWith(jMinion.name) && ((minion.Name.Length - jMinion.name.Length) <= 6) && Config.Item(jMinion.name).GetValue<bool>() && Config.Item("justAS").GetValue<bool>() ||
                                    minion.Name.StartsWith(jMinion.name) && ((minion.Name.Length - jMinion.name.Length) <= 6) && !Config.Item("justAS").GetValue<bool>())
                                    {
                                        Vector2 hpBarPos = minion.HPBarPosition;
                                        hpBarPos.X += jMinion.preX;
                                        hpBarPos.Y += 18;
                                        int smiteDmg = getSmiteDmg();
                                        var damagePercent = smiteDmg / minion.MaxHealth;
                                        float hpXPos = hpBarPos.X + (jMinion.width * damagePercent);

                                        Drawing.DrawLine(hpXPos, hpBarPos.Y, hpXPos, hpBarPos.Y + 5, 2, smiteDmg > minion.Health ? System.Drawing.Color.Lime : System.Drawing.Color.WhiteSmoke);
                                    }
                                }
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

        private void onGameUpdate(EventArgs args)
        {
            if (Config.Item("enabled").GetValue<KeyBind>().Active)
            {
                try
                {

                    checkAndUse(clarity);
                    teamCheckAndUse(mikael);
                    if (!_player.InFountain() && !Config.Item("justPredHeal").GetValue<bool>())
                    {
                        teamCheckAndUse(heal, Config.Item("useWithHealDebuff").GetValue<bool>() ? "" : "summonerhealcheck");
                    }
                    
                    checkAndUse(cleanse);
                    checkAndUse(qss);
                    checkAndUse(mercurial);

                    checkAndUse(manaPot, "FlaskOfCrystalWater");
                    checkAndUse(hpPot, "RegenerationPotion");
                    checkAndUse(biscuit, "ItemMiniRegenPotion");

                    if (!Config.Item("justPred").GetValue<bool>() || !Config.Item("predict").GetValue<bool>())
                    {
                        checkAndUse(zhonya);
                        checkAndUse(barrier);
                        checkAndUse(seraph);
                        teamCheckAndUse(solari, "", true);
                        teamCheckAndUse(mountain);
                        checkAndUseShield();
                    }

                    checkAndUse(smite);
                    checkAndUse(smiteAOE);
                    checkAndUse(smiteDuel);
                    checkAndUse(smiteGanker);
                    checkAndUse(smiteQuick);

                    if (Config.Item("comboModeActive").GetValue<KeyBind>().Active)
                    {
                        combo();
                    }
                }
                catch
                {
                    Game.PrintChat("MasterActivator presented a problem, and has been disabled!");
                    Config.Item("enabled").SetValue<KeyBind>(new KeyBind('L', KeyBindType.Toggle, false)); // Check
                }
            }
        }

        private void combo()
        {
            checkAndUse(ignite);
            checkAndUse(youmus);
            checkAndUse(bilgewater);
            checkAndUse(king);
            checkAndUse(tiamat, "", 0, true);
            checkAndUse(hydra, "", 0, true);
            checkAndUse(dfg);
            checkAndUse(divine);
            checkAndUse(hextech);
            checkAndUse(muramana);
            checkAndUse(frost);
            checkAndUse(randuin);
        }

        // And about ignore HP% check?
        private void justUseAgainstCheck(MItem item, double incDmg, Obj_AI_Base attacker = null, Obj_AI_Base attacked = null, SpellSlot attackerSpellSlot = SpellSlot.Unknown)
        {
            // Se tem o spell
            if (Utility.GetSpellSlot(_player, item.name) != SpellSlot.Unknown)
            {
                if (attacker != null && attacked != null)
                {
                    // player
                    if (attacker.Type == GameObjectType.obj_AI_Hero)
                    {
                        // Se estiver habilitado para o determinado player
                        //Console.WriteLine(item.menuVariable + attacker.BaseSkinName);
                        if (Config.Item(item.menuVariable + attacker.BaseSkinName).GetValue<bool>())
                        {
                            //Console.WriteLine("Player habilitado->" + attacker.BaseSkinName);
                            if (attackerSpellSlot != SpellSlot.Unknown)
                            {
                                // Se a habilidade estiver habilitada
                                if (Config.Item(attackerSpellSlot + item.menuVariable + attacker.BaseSkinName).GetValue<bool>())
                                {
                                    if (item.type == ItemTypeId.Ability && attacked.IsMe)
                                    {
                                        checkAndUse(item, "", incDmg);
                                    }
                                    else if (item.type == ItemTypeId.TeamAbility || item.type == ItemTypeId.TeamAbilityAOE)
                                    {
                                        teamCheckAndUse(item, "", false, incDmg, attacked, attacker);
                                    }
                                }
                            }
                        }
                        //FIX-ME OR ..
                    }
                    // tower
                    else
                    {
                        if (Config.Item("tower" + item.menuVariable).GetValue<bool>())
                        {
                            if (item.type == ItemTypeId.Ability && attacked.IsMe)
                            {
                                checkAndUse(item, "", incDmg);
                            }
                            else if (item.type == ItemTypeId.TeamAbility || item.type == ItemTypeId.TeamAbilityAOE)
                            {
                                teamCheckAndUse(item, "", false, incDmg, attacked);
                            }
                        }
                    }
                }
                // OFF JustPred
                else 
                {
                    checkAndUse(item, "", incDmg);
                    teamCheckAndUse(item, "", false, incDmg, attacked);
                }
            }
        }

        private void checkAndUseShield(double incDmg = 0, Obj_AI_Base attacker = null, Obj_AI_Base attacked = null, SpellSlot attackerSpellSlot = SpellSlot.Unknown)
        {
            justUseAgainstCheck(titanswraith, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(blackshield, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(unbreakable, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(palecascade, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(bulwark, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(courage, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(eyeofstorm, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(inspire, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(helppix, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(prismaticbarrier, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(commandprotect, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(spellshield, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(nocturneShield, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(yasuoShield, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(fioraRiposte, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(tryndaUlt, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(nasusUlt, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(renekUlt, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(leonaW, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(annieE, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(vladW, incDmg, attacker, attacked, attackerSpellSlot);
            justUseAgainstCheck(wukongW, incDmg, attacker, attacked, attackerSpellSlot);
        }

        private bool checkBuff(String name)
        {
            var searchedBuff = from buff in _player.Buffs
                               where buff.Name == name
                               select buff;

            return searchedBuff.Count() <= 0 ? false : true;
        }

        private void createMenuItem(MItem item, String parent, int defaultValue = 0, bool mana = false, int minManaPct = 0)
        {
            if (item.type == ItemTypeId.Ability || item.type == ItemTypeId.TeamAbility || item.type == ItemTypeId.TeamAbilityAOE)
            {
                var abilitySlot = Utility.GetSpellSlot(_player, item.name);
                if (abilitySlot != SpellSlot.Unknown && abilitySlot == item.abilitySlot)
                {
                    var menu = new Menu(item.menuName, "menu" + item.menuVariable);
                    menu.AddItem(new MenuItem(item.menuVariable, "Enable").SetValue(true));
                    menu.AddItem(new MenuItem(item.menuVariable + "UseOnPercent", "Use on HP%")).SetValue(new Slider(defaultValue, 0, 100));
                    if (minManaPct > 0)
                    {
                        menu.AddItem(new MenuItem(item.menuVariable + "UseManaPct", "Min Mana%")).SetValue(new Slider(minManaPct, 0, 100));
                    }
                    var menuUseAgainst = new Menu("Filter", "UseAgainst");
                    menuUseAgainst.AddItem(new MenuItem("tower" + item.menuVariable, "Tower").SetValue(true));
                    var enemyHero = from hero in ObjectManager.Get<Obj_AI_Hero>()
                                   where hero.Team != _player.Team
                                  select hero;

                    if (enemyHero.Count() > 0)
                    {
                        foreach (Obj_AI_Hero hero in enemyHero)
                        {
                            var menuUseAgainstHero = new Menu(hero.BaseSkinName, "useAgainst" + hero.BaseSkinName);
                            menuUseAgainstHero.AddItem(new MenuItem(item.menuVariable + hero.BaseSkinName, "Enabled").SetValue(false));
                            menuUseAgainstHero.AddItem(new MenuItem(SpellSlot.Q + item.menuVariable + hero.BaseSkinName, "Q").SetValue(false));
                            menuUseAgainstHero.AddItem(new MenuItem(SpellSlot.W + item.menuVariable + hero.BaseSkinName, "W").SetValue(false));
                            menuUseAgainstHero.AddItem(new MenuItem(SpellSlot.E + item.menuVariable + hero.BaseSkinName, "E").SetValue(false));
                            menuUseAgainstHero.AddItem(new MenuItem(SpellSlot.R + item.menuVariable + hero.BaseSkinName, "R").SetValue(false));
                            menuUseAgainst.AddSubMenu(menuUseAgainstHero);
                            // Bring all, passives, summoners spells, etc;
                            /*if (hero.Spellbook.Spells.Count() > 0)
                            {
                                var menuUseAgainstHero = new Menu(hero.BaseSkinName, "useAgainst" + hero.BaseSkinName);
                                menuUseAgainstHero.AddItem(new MenuItem(item.menuVariable, "Enable").SetValue(true));
                                foreach(SpellDataInst spell in hero.Spellbook.Spells)
                                {
                                    menuUseAgainstHero.AddItem(new MenuItem("useAgainstSpell" + spell.Name, spell.Name).SetValue(true));
                                }
                                menuUseAgainst.AddSubMenu(menuUseAgainstHero);
                            }
                            else
                            {
                                Game.PrintChat("MasterActivator cant get " + hero.BaseSkinName + " spells!");
                            }*/
                        }
                    }
                    menu.AddSubMenu(menuUseAgainst);
                    Config.SubMenu(parent).AddSubMenu(menu);
                }
            }
            else
            {
                var menu = new Menu(item.menuName, "menu" + item.menuVariable);
                menu.AddItem(new MenuItem(item.menuVariable, "Enable").SetValue(true));

                if (defaultValue != 0)
                {
                    if (item.type == ItemTypeId.OffensiveAOE)
                    {
                        menu.AddItem(new MenuItem(item.menuVariable + "UseXUnits", "On X Units")).SetValue(new Slider(defaultValue, 1, 5));
                    }
                    else
                    {
                        menu.AddItem(new MenuItem(item.menuVariable + "UseOnPercent", "Use on " + (mana == false ? "%HP" : "%Mana"))).SetValue(new Slider(defaultValue, 0, 100));
                    }
                }
                Config.SubMenu(parent).AddSubMenu(menu);
            }
        }

        private void teamCheckAndUse(MItem item, String buff = "", bool self = false, double incDmg = 0, Obj_AI_Base attacked = null, Obj_AI_Base attacker = null)
        {
            if (Config.Item(item.menuVariable) != null)
            {
                // check if is configured to use
                if (Config.Item(item.menuVariable).GetValue<bool>())
                {
                    if (item.type == ItemTypeId.DeffensiveSpell || item.type == ItemTypeId.ManaRegeneratorSpell || item.type == ItemTypeId.PurifierSpell)
                    {
                        //Console.WriteLine("TCandU-> " + item.name);
                        var spellSlot = Utility.GetSpellSlot(_player, item.menuVariable);
                        if (spellSlot != SpellSlot.Unknown)
                        {
                            var activeAllyHeros = getActiveAllyHeros(item);
                            if (activeAllyHeros.Count() > 0)
                            {
                                int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;

                                foreach (Obj_AI_Hero hero in activeAllyHeros)
                                {
                                    //Console.WriteLine("Hero-> " + hero.SkinName);
                                    int enemyInRange = Utility.CountEnemiesInRange(hero, 700);
                                    if (enemyInRange >= 1)
                                    {
                                        int actualHeroHpPercent = (int)(((_player.Health - incDmg) / _player.MaxHealth) * 100); //after dmg not Actual ^^
                                        int actualHeroManaPercent = (int)((_player.Mana / _player.MaxMana) * 100);

                                        //Console.WriteLine("actHp% -> " + actualHeroHpPercent + "   useOn%-> " + usePercent + "  IncDMG-> " + incDmg);

                                        if ((item.type == ItemTypeId.DeffensiveSpell && actualHeroHpPercent <= usePercent) ||
                                            (item.type == ItemTypeId.ManaRegeneratorSpell && actualHeroManaPercent <= usePercent))
                                        {
                                            _player.Spellbook.CastSpell(spellSlot);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (item.type == ItemTypeId.TeamAbility || item.type == ItemTypeId.TeamAbilityAOE)
                    {
                        try
                        {
                            var spellSlot = Utility.GetSpellSlot(_player, item.name);
                            if (spellSlot != SpellSlot.Unknown)
                            {
                                var activeAllyHeros = getActiveAllyHeros(item);
                                if (activeAllyHeros.Count() > 0)
                                {
                                    if (_player.Spellbook.CanUseSpell(spellSlot) == SpellState.Ready)
                                    {
                                        int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;
                                        int manaPercent = Config.Item(item.menuVariable + "UseManaPct") != null ? Config.Item(item.menuVariable + "UseManaPct").GetValue<Slider>().Value : 0;
                                        foreach (Obj_AI_Hero hero in activeAllyHeros)
                                        {
                                            if (hero.NetworkId == attacked.NetworkId)
                                            {
                                                int actualHeroHpPercent = (int)(((hero.Health - incDmg) / hero.MaxHealth) * 100); //after dmg not Actual ^^
                                                int playerManaPercent = (int)((_player.Mana / _player.MaxMana) * 100);
                                                if (playerManaPercent >= manaPercent && actualHeroHpPercent <= usePercent)
                                                {
                                                    if (item.type == ItemTypeId.TeamAbility)
                                                    {
                                                        _player.Spellbook.CastSpell(item.abilitySlot, hero);
                                                    }
                                                    else
                                                    {
                                                        Vector3 pos = hero.Position;
                                                        // extend 20 to attacker direction THIS 20 COST RANGE
                                                        if (attacker != null)
                                                        {
                                                            if(_player.Distance(hero.Position.Extend(attacker.Position, 20), false) <= item.range)
                                                            {
                                                                pos = hero.Position.Extend(attacker.Position, 20);
                                                            }
                                                        }
                                                        _player.Spellbook.CastSpell(item.abilitySlot, pos);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Game.PrintChat("Problem with MasterActivator(AutoShield).");
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
                                            int enemyInRange = Utility.CountEnemiesInRange(hero, 700);
                                            if (enemyInRange >= 1)
                                            {
                                                int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;
                                                int actualHeroHpPercent = (int)((hero.Health / hero.MaxHealth) * 100);
                                                if (actualHeroHpPercent <= usePercent)
                                                {
                                                    if (self)
                                                    {
                                                        useItem(item.id);
                                                    }
                                                    else
                                                    {
                                                        useItem(item.id, hero);
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
        }

        private IEnumerable<Obj_AI_Hero> getActiveAllyHeros(MItem item)
        {
            var activeAllyHeros = from hero in ObjectManager.Get<Obj_AI_Hero>()
                                  where hero.Team == _player.Team &&
                                        Config.Item(hero.SkinName).GetValue<bool>() &&
                                        hero.Distance(_player, false) <= item.range &&
                                        !hero.IsDead
                                  select hero;

            return activeAllyHeros;
        }

        private void checkAndUse(MItem item, String buff = "", double incDamage = 0, bool self = false)
        {
            if (Config.Item(item.menuVariable) != null)
            {
                // check if is configured to use
                if (Config.Item(item.menuVariable).GetValue<bool>())
                {
                    int actualHeroHpPercent = (int)(((_player.Health - incDamage) / _player.MaxHealth) * 100); //after dmg not Actual ^^
                    int actualHeroManaPercent = (int)(_player.MaxMana > 0 ? ((_player.Mana / _player.MaxMana) * 100) : 0);

                    if (item.type == ItemTypeId.DeffensiveSpell || item.type == ItemTypeId.ManaRegeneratorSpell || item.type == ItemTypeId.PurifierSpell || item.type == ItemTypeId.OffensiveSpell)
                    {
                        var spellSlot = Utility.GetSpellSlot(_player, item.menuVariable);
                        if (spellSlot != SpellSlot.Unknown)
                        {
                            if (_player.Spellbook.CanUseSpell(spellSlot) == SpellState.Ready)
                            {
                                if (item.type == ItemTypeId.DeffensiveSpell)
                                {
                                    int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;
                                    if (actualHeroHpPercent <= usePercent)
                                    {
                                        _player.Spellbook.CastSpell(spellSlot);
                                    }
                                }
                                else if (item.type == ItemTypeId.ManaRegeneratorSpell)
                                {
                                    int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;
                                    if (actualHeroManaPercent <= usePercent && !_player.InFountain())
                                    {
                                        _player.Spellbook.CastSpell(spellSlot);
                                    }
                                }
                                else if (item.type == ItemTypeId.PurifierSpell)
                                {
                                    if ((Config.Item("defJustOnCombo").GetValue<bool>() && Config.Item("comboModeActive").GetValue<KeyBind>().Active) ||
                                        (!Config.Item("defJustOnCombo").GetValue<bool>()))
                                    {
                                        if (checkCC(_player))
                                        {
                                            _player.Spellbook.CastSpell(spellSlot);
                                        }
                                    }
                                }
                                else if (item.type == ItemTypeId.OffensiveSpell)
                                {
                                    if (item == ignite)
                                    {
                                        // TargetSelector.TargetingMode.LowHP FIX/Check
                                        Obj_AI_Hero target = TargetSelector.GetTarget(item.range, TargetSelector.DamageType.Physical); // Check about DamageType
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

                                            if (target.Health < (dmgafter + aadmg) && _player.Distance(target, false) <= item.range)
                                            {
                                                bool overIgnite = Config.Item("overIgnite").GetValue<bool>();
                                                if ((!overIgnite && !target.HasBuff("summonerdot")) || overIgnite)
                                                {
                                                    _player.Spellbook.CastSpell(spellSlot, target);
                                                    //Drawing.DrawText(pScreen[0], pScreen[1], System.Drawing.Color.Crimson, "Kill in " + aaleft);
                                                }

                                            }

                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            string[] jungleMinions;
                                            if (Utility.Map.GetMap().Type.Equals(Utility.Map.MapType.TwistedTreeline))
                                            {
                                                jungleMinions = new string[] { tVilemaw.name, tWraith.name, tGolem.name, tWolf.name };
                                            }
                                            else
                                            {
                                                jungleMinions = new string[] { blue.name, red.name, razor.name, baron.name, krug.name, wolf.name, dragon.name, gromp.name, crab.name };
                                            }

                                            var minions = MinionManager.GetMinions(_player.Position, item.range, MinionTypes.All, MinionTeam.Neutral);
                                            if (minions.Count() > 0)
                                            {
                                                int smiteDmg = getSmiteDmg();
                                                foreach (Obj_AI_Base minion in minions)
                                                {
                                                    if (minion.Health <= smiteDmg && jungleMinions.Any(name => minion.Name.StartsWith(name) && ((minion.Name.Length - name.Length) <= 6) && Config.Item(name).GetValue<bool>()))
                                                    {
                                                        _player.Spellbook.CastSpell(spellSlot, minion);
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
                    else if (item.type == ItemTypeId.Ability || item.type == ItemTypeId.TeamAbility)
                    {
                        try
                        {
                            var spellSlot = Utility.GetSpellSlot(_player, item.name);
                            if (spellSlot != SpellSlot.Unknown)
                            {
                                if (_player.Spellbook.CanUseSpell(spellSlot) == SpellState.Ready)
                                {
                                    int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;
                                    int manaPercent = Config.Item(item.menuVariable + "UseManaPct") != null ? Config.Item(item.menuVariable + "UseManaPct").GetValue<Slider>().Value : 0;
                                    //Console.WriteLine("ActualMana%-> " + actualHeroManaPercent + "  Mana%->" + manaPercent + "  Acthp%->" + actualHeroHpPercent + "   Use%->" + usePercent);
                                    
                                    if (actualHeroManaPercent >= manaPercent && actualHeroHpPercent <= usePercent)
                                    {
                                        _player.Spellbook.CastSpell(item.abilitySlot, _player);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Game.PrintChat("Problem with MasterActivator(AutoShield).");
                        }
                    }
                    else
                    {
                        if (Items.HasItem(item.id))
                        {
                            //Console.WriteLine("Tem item->" + item.id + item.name);
                            if (Items.CanUseItem(item.id))
                            {
                                if (item.type == ItemTypeId.Offensive)
                                {
                                    if (checkTarget(item.range))
                                    {
                                        int actualTargetHpPercent = (int)((target.Health / target.MaxHealth) * 100);
                                        if (checkUsePercent(item, actualTargetHpPercent))
                                        {
                                            useItem(item.id, (item.range == 0 || self) ? null : target);
                                        }
                                    }
                                }
                                else if (item.type == ItemTypeId.OffensiveAOE)
                                {
                                    if (checkTarget(item.range))
                                    {
                                        // FIX-ME: In frost case, we must check the affected area, not just ppl in range(item).
                                        if (Utility.CountEnemiesInRange(_player, (int)item.range) >= Config.Item(item.menuVariable + "UseXUnits").GetValue<Slider>().Value)
                                        {
                                            useItem(item.id, item.range == 0 ? null : target);
                                        }
                                    }
                                }
                                else if (item.type == ItemTypeId.HPRegenerator)
                                {
                                    if (checkUsePercent(item, actualHeroHpPercent) && !_player.InFountain() && !Utility.IsRecalling(_player))
                                    {
                                        if ((buff != "" && !checkBuff(buff)) || buff == "")
                                        {
                                            useItem(item.id);
                                        }
                                    }
                                }
                                else if (item.type == ItemTypeId.Deffensive)
                                {
                                    if (checkUsePercent(item, actualHeroHpPercent) && !_player.InFountain() && (Config.Item("useRecalling").GetValue<bool>() || !Utility.IsRecalling(_player)))
                                    {
                                        if ((buff != "" && !checkBuff(buff)) || buff == "")
                                        {
                                            useItem(item.id);
                                        }
                                    }
                                }
                                else if (item.type == ItemTypeId.ManaRegenerator)
                                {
                                    if (checkUsePercent(item, actualHeroManaPercent) && !_player.InFountain() && !Utility.IsRecalling(_player))
                                    {
                                        if ((buff != "" && !checkBuff(buff)) || buff == "")
                                        {
                                            useItem(item.id);
                                        }
                                    }
                                }
                                else if (item.type == ItemTypeId.Buff)
                                {
                                    if (checkTarget(item.range))
                                    {
                                        if (!checkBuff(item.name))
                                        {
                                            useItem(item.id);
                                        }
                                    }
                                    else
                                    {
                                        if (checkBuff(item.name))
                                        {
                                            useItem(item.id);
                                        }
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

            target = TargetSelector.GetTarget(range, TargetSelector.DamageType.Physical);

            /*int targetModeSelectedIndex = Config.Item("targetMode").GetValue<StringList>().SelectedIndex;
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
            ts.SetTargetingMode(targetModeSelected);*/

            return target != null ? true : false;
        }

        private void createMenu()
        {
            Config = new Menu("MActivator", "masterActivator", true);

            Config.AddSubMenu(new Menu("Purifiers", "purifiers"));
            createMenuItem(qss, "purifiers");
            createMenuItem(mercurial, "purifiers");
            createMenuItem(cleanse, "purifiers");
            createMenuItem(mikael, "purifiers");
            Config.SubMenu("purifiers").AddItem(new MenuItem("defJustOnCombo", "Just on combo")).SetValue(false);

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
            Config.SubMenu("purify").AddItem(new MenuItem("dehancer", "Dehancer")).SetValue(false);
            Config.SubMenu("purify").AddItem(new MenuItem("zedultexecute", "Zed Ult")).SetValue(true);

            Config.AddSubMenu(new Menu("Smite", "smiteCfg"));
            var menuSmiteSpell = new Menu("Spell", "smiteSpell");
            menuSmiteSpell.AddItem(new MenuItem(smite.menuVariable, smite.menuName).SetValue(true));
            menuSmiteSpell.AddItem(new MenuItem(smiteAOE.menuVariable, smiteAOE.menuName).SetValue(true));
            menuSmiteSpell.AddItem(new MenuItem(smiteDuel.menuVariable, smiteDuel.menuName).SetValue(true));
            menuSmiteSpell.AddItem(new MenuItem(smiteGanker.menuVariable, smiteGanker.menuName).SetValue(true));
            menuSmiteSpell.AddItem(new MenuItem(smiteQuick.menuVariable, smiteQuick.menuName).SetValue(true));
            Config.SubMenu("smiteCfg").AddSubMenu(menuSmiteSpell);

            var menuSmiteMobs = new Menu("Mob", "smiteMobs");
            if (Utility.Map.GetMap().Type.Equals(Utility.Map.MapType.TwistedTreeline))
            {
                menuSmiteMobs.AddItem(new MenuItem("TT_Spiderboss", "Vilemaw")).SetValue(true);
                menuSmiteMobs.AddItem(new MenuItem("TT_NWraith", "Wraith")).SetValue(false);
                menuSmiteMobs.AddItem(new MenuItem("TT_NGolem", "Golem")).SetValue(true);
                menuSmiteMobs.AddItem(new MenuItem("TT_NWolf", "Wolf")).SetValue(true);
            }
            else
            {
                menuSmiteMobs.AddItem(new MenuItem(blue.name, blue.menuName)).SetValue(true);
                menuSmiteMobs.AddItem(new MenuItem(red.name, red.menuName)).SetValue(true);
                menuSmiteMobs.AddItem(new MenuItem(dragon.name, dragon.menuName)).SetValue(true);
                menuSmiteMobs.AddItem(new MenuItem(baron.name, baron.menuName)).SetValue(true);
                menuSmiteMobs.AddItem(new MenuItem(razor.name, razor.menuName)).SetValue(false);
                menuSmiteMobs.AddItem(new MenuItem(krug.name, krug.menuName)).SetValue(false);
                menuSmiteMobs.AddItem(new MenuItem(wolf.name, wolf.menuName)).SetValue(false);
                menuSmiteMobs.AddItem(new MenuItem(gromp.name, gromp.menuName)).SetValue(false);
                menuSmiteMobs.AddItem(new MenuItem(crab.name, crab.menuName)).SetValue(false);

            }
            Config.SubMenu("smiteCfg").AddSubMenu(menuSmiteMobs);

            var menuSmiteDraw = new Menu("Draw", "smiteDraw");
            menuSmiteDraw.AddItem(new MenuItem("dSmite", "Enabled")).SetValue(true);
            menuSmiteDraw.AddItem(new MenuItem("justAS", "Just Selected Mobs")).SetValue(false);
            Config.SubMenu("smiteCfg").AddSubMenu(menuSmiteDraw);

            Config.AddSubMenu(new Menu("Offensive", "offensive"));
            createMenuItem(ignite, "offensive");
            Config.SubMenu("offensive").SubMenu("menu" + ignite.menuVariable).AddItem(new MenuItem("overIgnite", "Over Ignite")).SetValue(false);
            createMenuItem(youmus, "offensive", 100);
            createMenuItem(bilgewater, "offensive", 100);
            createMenuItem(king, "offensive", 100);
            createMenuItem(tiamat, "offensive", 100);
            createMenuItem(hydra, "offensive", 100);
            createMenuItem(dfg, "offensive", 100);
            createMenuItem(divine, "offensive", 80);
            createMenuItem(hextech, "offensive", 80);
            createMenuItem(muramana, "offensive", 80);

            Config.AddSubMenu(new Menu("Off. AOE", "offAOE"));
            createMenuItem(frost, "offAOE", 2);
            createMenuItem(randuin, "offAOE", 1);

            Config.AddSubMenu(new Menu("Deffensive", "deffensive"));
            createMenuItem(barrier, "deffensive", 35);
            createMenuItem(seraph, "deffensive", 45);
            createMenuItem(zhonya, "deffensive", 35);
            createMenuItem(solari, "deffensive", 45);
            createMenuItem(mountain, "deffensive", 45);
            Config.SubMenu("deffensive").AddItem(new MenuItem("justPred", "Just Predicted")).SetValue(true);
            Config.SubMenu("deffensive").AddItem(new MenuItem("useRecalling", "Use Recalling")).SetValue(false);

            Config.AddSubMenu(new Menu("Auto Skill", "autoshield"));
            createMenuItem(blackshield, "autoshield", 90, false, 40);
            createMenuItem(unbreakable, "autoshield", 90, false, 40);
            createMenuItem(bulwark, "autoshield", 90, false, 40);
            createMenuItem(courage, "autoshield", 90);
            createMenuItem(eyeofstorm, "autoshield", 90, false, 40);
            createMenuItem(inspire, "autoshield", 90, false, 40);
            createMenuItem(helppix, "autoshield", 90, false, 40);
            createMenuItem(prismaticbarrier, "autoshield", 90, false, 40);
            createMenuItem(titanswraith, "autoshield", 90, false, 40);
            createMenuItem(commandprotect, "autoshield", 99, false, 40);
            createMenuItem(feint, "autoshield", 90, false, 0);
            createMenuItem(spellshield, "autoshield", 90, false, 0);
            createMenuItem(nocturneShield, "autoshield", 90, false, 0);
            createMenuItem(yasuoShield, "autoshield", 90);
            createMenuItem(fioraRiposte, "autoshield", 90, false, 0);
            createMenuItem(tryndaUlt, "autoshield", 30);
            createMenuItem(nasusUlt, "autoshield", 30, false, 0);
            createMenuItem(renekUlt, "autoshield", 30);
            createMenuItem(leonaW, "autoshield", 60, false, 0);
            createMenuItem(annieE, "autoshield", 60, false, 0);
            createMenuItem(vladW, "autoshield", 45);
            createMenuItem(wukongW, "autoshield", 40, false, 0);

            Config.AddSubMenu(new Menu("Regenerators", "regenerators"));
            createMenuItem(heal, "regenerators", 35);
            Config.SubMenu("regenerators").SubMenu("menu" + heal.menuVariable).AddItem(new MenuItem("useWithHealDebuff", "Use with debuff")).SetValue(true);
            Config.SubMenu("regenerators").SubMenu("menu" + heal.menuVariable).AddItem(new MenuItem("justPredHeal", "Just predicted")).SetValue(false);
            createMenuItem(clarity, "regenerators", 25, true);
            createMenuItem(hpPot, "regenerators", 55);
            createMenuItem(manaPot, "regenerators", 55, true);
            createMenuItem(biscuit, "regenerators", 55);

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
            TargetSelector.AddToMenu(Config.SubMenu("targetSelector"));

            Config.AddItem(new MenuItem("predict", "Predict DMG")).SetValue(true);
            //Config.AddItem(new MenuItem("enabled", "Enabled")).SetValue(true);
            //38
            Config.AddItem(new MenuItem("enabled", "Enabled")).SetValue(new KeyBind('L', KeyBindType.Toggle, true));

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

            if (Config.Item("dehancer").GetValue<bool>())
            {
                if (hero.HasBuffOfType(BuffType.CombatDehancer))
                {
                    cc = true;
                }
            }

            if (Config.Item("zedultexecute").GetValue<bool>())
            {
                if (hero.HasBuff("zedultexecute", false, true))
                {
                    cc = true;
                }
            }

            return cc;
        }
    }
}