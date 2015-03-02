using System;
using System.IO;
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
        //private StreamWriter log;
        private int checkCCTick;

        #region Items
        MItem qss = new MItem("Quicksilver Sash", "QSS", "qss", 3140, ItemTypeId.Purifier);
        MItem mercurial = new MItem("ItemMercurial", "Mercurial", "mercurial", 3139, ItemTypeId.Purifier);
        MItem bilgewater = new MItem("BilgewaterCutlass", "Bilgewater", "bilge", 3144, ItemTypeId.Offensive, 450);
        MItem king = new MItem("ItemSwordOfFeastAndFamine", "BoRKing", "king", 3153, ItemTypeId.Offensive, 450);
        MItem youmus = new MItem("YoumusBlade", "Youmuu's", "youmus", 3142, ItemTypeId.Offensive);
        MItem tiamat = new MItem("ItemTiamatCleave", "Tiamat", "tiamat", 3077, ItemTypeId.Offensive, 400, SpellSlot.Unknown, SpellType.Self);
        MItem hydra = new MItem("Ravenous Hydra", "Hydra", "hydra", 3074, ItemTypeId.Offensive, 400, SpellSlot.Unknown, SpellType.Self);
        MItem dfg = new MItem("DeathfireGrasp", "DFG", "dfg", 3128, ItemTypeId.Offensive, 750);
        MItem divine = new MItem("ItemSoTD", "SoDivine", "divine", 3131, ItemTypeId.Offensive); //Sword of the Divine
        MItem hextech = new MItem("Hextech Gunblade", "Hextech", "hextech", 3146, ItemTypeId.Offensive, 700);
        MItem muramana = new MItem("Muramana", "Muramana", "muramana", 3042, ItemTypeId.Buff);
        MItem seraph = new MItem("ItemSeraphsEmbrace", "Seraph's", "seraph", 3040, ItemTypeId.Deffensive);
        MItem zhonya = new MItem("ZhonyasHourglass", "Zhonya's", "zhonya", 3157, ItemTypeId.Deffensive);
        MItem wooglet = new MItem("Wooglet's Witchcap", "Wooglet's", "wooglet", 3090, ItemTypeId.Deffensive);
        MItem randuin = new MItem("RanduinsOmen", "Randuin's", "randuin", 3143, ItemTypeId.OffensiveAOE, 500, SpellSlot.Unknown, SpellType.Self);
        //Item banner = new Item("Banner of Command", "BoCommand", "banner", 3060); // falta range
        MItem mountain = new MItem("Face of the Mountain", "FoMountain", "mountain", 3401, ItemTypeId.Deffensive, 700); // falta range
        MItem frost = new MItem("Frost Queen's Claim", "Frost Queen's", "frost", 3092, ItemTypeId.OffensiveAOE, 850);
        MItem solari = new MItem("Locket of the Iron Solari", "Solari", "solari", 3190, ItemTypeId.Deffensive, 600, SpellSlot.Unknown, SpellType.Self);
        MItem mikael = new MItem("Mikael's Crucible", "Mikael's", "mikael", 3222, ItemTypeId.Purifier, 750);
        MItem mikaelHP = new MItem("Mikael's Crucible", "Mikael's", "mikaelHP", 3222, ItemTypeId.Deffensive, 750);
        //Item talisman = new Item("Talisman of Ascension", "Talisman", "talisman", 3069, 600);
        //Item shadows = new Item("Twin Shadows", "Shadows", "shadows", 3023, 750); //2700
        //Item ohmwrecker = new Item("Ohmwrecker", "Ohmwrecker", "ohmwrecker", 3056, 775); // tower atk range Utility.UnderTurret
        MItem hpPot = new MItem("Health Potion", "HP Pot", "hpPot", 2003, ItemTypeId.HPRegenerator);
        MItem manaPot = new MItem("Mana Potion", "Mana Pot", "manaPot", 2004, ItemTypeId.ManaRegenerator);
        MItem biscuit = new MItem("Total Biscuit of Rejuvenation", "Biscuit", "biscuit", 2010, ItemTypeId.HPRegenerator);
        MItem cFlaskHP = new MItem("Crystalline Flask", "Cryst. Flask HP", "cFlaskHP", 2041, ItemTypeId.HPRegenerator);
        MItem cFlaskMP = new MItem("Crystalline Flask", "Cryst. Flask MP", "cFlaskMP", 2041, ItemTypeId.ManaRegenerator);
        #endregion

        #region Wards
        // se tiver thresh cria menu/verifica; Se o thresh estiver longe do skill; for inimigo
        MItem wardTotem = new MItem("Warding Totem", "Ward Totem", "wTotem", 3340, ItemTypeId.Ward, 600);
        MItem pink = new MItem("Vision Ward", "Pink", "vWard", 2043, ItemTypeId.VisionWard, 600); //pink
        MItem ward = new MItem("Stealth Ward", "Ward", "ward", 2044, ItemTypeId.Ward, 600);
        MItem sightStone = new MItem("Sightstone", "Sightstone", "sightStone", 2049, ItemTypeId.Ward, 600);
        MItem rubySightStone = new MItem("Ruby Sightstone", "Ruby Sightstone", "rubySightStone", 2045, ItemTypeId.Ward, 600);
        MItem greatVisionTotem = new MItem("Greater Vision Totem", "G.Vision Totem", "gVTotem", 3362, ItemTypeId.VisionWard, 600);
        MItem greatWardTotem = new MItem("Greater Stealth Totem", "G. Ward Totem", "gWTotem", 3361, ItemTypeId.Ward, 600);
        #endregion

        #region SummonerSpells
        // Heal prioritizes the allied champion closest to the cursor at the time the ability is cast.
        // If no allied champions are near the cursor, Heal will target the most wounded allied champion in range.
        MItem heal = new MItem("Heal", "Heal", "SummonerHeal", 0, ItemTypeId.DeffensiveSpell, 700); // 300? www.gamefaqs.com/pc/954437-league-of-legends/wiki/3-1-summoner-spells
        MItem exhaust = new MItem("Exhaust", "Exhaust", "SummonerExhaust", 0, ItemTypeId.OffensiveSpell, 650); //summonerexhaust, low, debuff (buffs)
        MItem barrier = new MItem("Barrier", "Barrier", "SummonerBarrier", 0, ItemTypeId.DeffensiveSpell);
        MItem cleanse = new MItem("Cleanse", "Cleanse", "SummonerBoost", 0, ItemTypeId.PurifierSpell);
        MItem clarity = new MItem("Clarity", "Clarity", "SummonerMana", 0, ItemTypeId.ManaRegeneratorSpell, 600);
        MItem ignite = new MItem("Ignite", "Ignite", "SummonerDot", 0, ItemTypeId.OffensiveSpell, 600);
        MItem smite = new MItem("Smite", "Smite", "SummonerSmite", 0, ItemTypeId.OffensiveSpell, 500);
        MItem smiteAOE = new MItem("SmiteAOE", "smite AOE", "itemsmiteaoe", 0, ItemTypeId.OffensiveSpell, 500);
        MItem smiteDuel = new MItem("SmiteDuel", "smite Duel", "s5_summonersmiteduel", 0, ItemTypeId.OffensiveSpell, 500);
        MItem smiteQuick = new MItem("SmiteQuick", "smite Quick", "s5_summonersmitequick", 0, ItemTypeId.OffensiveSpell, 500);
        MItem smiteGanker = new MItem("SmiteGanker", "smite Ganker", "s5_summonersmiteplayerganker", 0, ItemTypeId.OffensiveSpell, 500);
        #endregion

        #region Spells
        #region Deffensive
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
        MItem yasuoShield = new MItem("YasuoWMovingWall", "Yasuo Shield", "yShield", 0, ItemTypeId.TeamAbility, 400, SpellSlot.W);
        MItem fioraRiposte = new MItem("FioraRiposte", "Fiora Riposte", "fRiposte", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);// S2 fiora
        MItem fioraDance = new MItem("FioraDance", "Fiora Dance", "fDance", 0, ItemTypeId.Ability, 400, SpellSlot.R, SpellType.TargetEnemy); // FioraDanceStrike
        MItem masterQ = new MItem("AlphaStrike", "Master Q", "masterQ", 0, ItemTypeId.Ability, 600, SpellSlot.Q, SpellType.TargetEnemy);
        MItem tryndaUlt = new MItem("UndyIngrage", "Trynda Ult.", "tIngrage", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.R);// Trynda mia ult
        MItem nasusUlt = new MItem("NasusR", "Nasus Ult.", "nasusR", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.R);
        MItem renekUlt = new MItem("RenektonReignOfTheTyrant", "Renek Ult.", "renekR", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.R); //Renek nek
        MItem leonaW = new MItem("LeonaSolarBarrier", "Leona Barrier", "leonaW", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);
        MItem annieE = new MItem("MoltenShield", "Annie Barrier", "annieE", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.E); // Annie
        MItem vladW = new MItem("VladimirSanguinePool", "Vlad. Pool", "vladW", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W); // nigga VladImir W
        MItem wukongW = new MItem("MonkeyKingDecoy", "Wu. Decoy", "wuW", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);
        MItem akaliW = new MItem("AkaliSmokeBomb", "Akali Smoke", "akaliW", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);
        MItem kayleR = new MItem("JudicatorIntervention", "Intervention", "kayleR", 0, ItemTypeId.TeamAbility, 900, SpellSlot.R);
        //MItem rivenE = new MItem("RivenFeint", "Riven Feint", "rivenE", 0, ItemTypeId.Ability, 325, SpellSlot.E);
        MItem nidaE = new MItem("PrimalSurge", "Primal Surge", "nidaE", 0, ItemTypeId.TeamAbility, 600, SpellSlot.E);
        MItem fizzE = new MItem("FizzJump", "Fizz Jump", "fizzE", 0, ItemTypeId.Ability, 400, SpellSlot.E);
        MItem sionW = new MItem("DeathScaress", "Soul Furnace", "sionW", 0, ItemTypeId.Ability, int.MaxValue, SpellSlot.W);
        MItem sonaW = new MItem("SonaAriaOfPerseverance", "Aria of Perseverance	", "sonaW", 0, ItemTypeId.TeamAbility, 1000, SpellSlot.W, SpellType.Self);
        MItem lissR = new MItem("LissandraR", "Liss R(Self)", "lissR", 0, ItemTypeId.Ability, 550, SpellSlot.R);
        //  sona W range 1000
        // lee W, blindmonkwone(targ), blindmonkwtwo(self-l-steal/mv) 700 range?
        //obduracy malp W
        //defensiveballcurl rammus W
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
        // soraka wish R global - astralblessing W 550 !self
        // imbue taric Q
        #endregion
        #region Offensives
        MItem choR = new MItem("Feast", "Feast", "Feast", 0, ItemTypeId.KSAbility, 255, SpellSlot.R);
        MItem nunuQ = new MItem("Consume", "Consume", "Consume", 0, ItemTypeId.KSAbility, 125, SpellSlot.Q);
        MItem amumuE = new MItem("Tantrum", "Tantrum", "Tantrum", 0, ItemTypeId.KSAbility, 350, SpellSlot.E);
        //nasusq
        MItem gragasR = new MItem("gragasr", "Explosive Cask", "gragasr", 0, ItemTypeId.KSAbility, 1150, SpellSlot.R, SpellType.SkillShotCircle);
        MItem luxR = new MItem("luxmalicecannon", "Final Spark", "luxmalicecannon", 0, ItemTypeId.KSAbility, 3340, SpellSlot.R, SpellType.SkillShotLine); 
        #endregion
        #endregion

        #region Jungle Minions
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
        #endregion

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
                checkCCTick = Utils.TickCount;
                createMenu();

                LeagueSharp.Drawing.OnDraw += onDraw;
                Game.OnGameUpdate += onGameUpdate;
                Obj_AI_Base.OnProcessSpellCast += onProcessSpellCast;
                Game.OnGameEnd += Game_OnGameEnd;

                /*String dTime = DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss");
                if (!File.Exists("C:/Windows/temp/MActivatorLOG" + dTime + ".log"))
                {
                    log = new StreamWriter("C:/Windows/temp/MActivatorLOG" + dTime + ".log");
                }
                else
                {
                    log = File.AppendText("C:/Windows/temp/MActivatorLOG" + dTime + ".log");
                }*/
            }
            catch
            {
                Game.PrintChat("MasterActivator error creating menu!");
            }
        }

        private void Game_OnGameEnd(GameEndEventArgs args)
        {
            //log.Close();
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
                    #region AkaliW
                    // Akali W Ward
                    if (attacker.Type == GameObjectType.obj_AI_Hero && attacker.IsEnemy)
                    {
                        if (args.SData.Name == akaliW.name)
                        {
                            if (Config.Item("menuAkaliW").GetValue<bool>())
                            {
                                if ((Config.Item("akaliWOnCombo").GetValue<bool>() && Config.Item("comboModeActive").GetValue<KeyBind>().Active) ||
                                   (!Config.Item("akaliWOnCombo").GetValue<bool>()))
                                {
                                    int usePercent = Config.Item("menuAkaliW" + "UseOnPercent").GetValue<Slider>().Value;
                                    int attackerHpPercent = (int)((attacker.Health / attacker.MaxHealth) * 100);

                                    if (attackerHpPercent <= usePercent)
                                    {
                                        // Try closer ward position; All wards have same range.
                                        Vector3 wardPos = args.End;
                                        if (wardPos.Distance(_player.Position) > pink.range)
                                        {
                                            wardPos = wardPos.Extend(_player.Position, 220);
                                            if (_player.Distance(wardPos) <= pink.range)
                                            {
                                                return;
                                            }
                                        }

                                        if (Config.Item(akaliW.menuVariable + greatVisionTotem.menuVariable).GetValue<bool>())
                                        {
                                            useItem(greatVisionTotem.id, wardPos);
                                            return;
                                        }

                                        if (Config.Item(akaliW.menuVariable + pink.menuVariable).GetValue<bool>())
                                        {
                                            useItem(pink.id, wardPos);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    AttackId attackId = AttackId.Unknown;
                    if (Config.Item("predict").GetValue<bool>())
                    {
                        #region WithTarget
                        if (spellTarget != null)
                        {
                            if (spellTarget.Type == GameObjectType.obj_AI_Hero)
                            {
                                #region SelfTarget
                                //&& attacker.IsEnemy
                                /*if (attacker.Type == GameObjectType.obj_AI_Hero && attacker.NetworkId == spellTarget.NetworkId)
                                {
                                    Obj_AI_Hero attackerHero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == attacker.NetworkId);
                                    Obj_AI_Hero attackedHero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == spellTarget.NetworkId);

                                    SpellDataInst spellA = attacker.Spellbook.Spells.FirstOrDefault(hero => args.SData.Name.Contains(hero.SData.Name));
                                    spellSlot = spellA == null ? SpellSlot.Unknown : spellA.Slot;

                                    float range1 = args.SData.CastRangeDisplayOverride;
                                    float range2 = args.SData.CastRange;
                                    float range = range1 > 1 ? range1 : range2;

                                    log.WriteLine("Self-Attacker->" + attackerHero.BaseSkinName + "    Spell->" + args.SData.Name + "    Range->" + range + "   Slot->" + spellSlot);

                                    // test prop.
                                    List<Obj_AI_Hero> alliesInRange = attackerHero.IsEnemy ? Utility.GetAlliesInRange(attackerHero, range) : Utility.GetEnemiesInRange(attackerHero, range);
                                    if (alliesInRange.Count > 0)
                                    {
                                        log.WriteLine("Count->" + alliesInRange.Count);
                                        foreach (Obj_AI_Hero hero in alliesInRange)
                                        {
                                            log.WriteLine("Got-> " + hero.BaseSkinName);
                                        }
                                    }
                                    //Console.WriteLine("Target Name2-> " + spellTarget.Name + "  Spell->" + args.SData.Name + "   SpellTT->" + args.SData.SpellTotalTime);
                                }*/
                                #endregion

                                #region EnemyTarget
                                //Config.Item(hero.SkinName).GetValue<bool>()
                                // 750 from greater range(mikael).
                                if (attacker.Type == GameObjectType.obj_AI_Hero && attacker.IsEnemy && (spellTarget.IsMe || (spellTarget.IsAlly && _player.Distance(spellTarget.Position) <= 750)))
                                {
                                    Obj_AI_Hero attackerHero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == attacker.NetworkId);
                                    Obj_AI_Hero attackedHero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == spellTarget.NetworkId);

                                    // Check TeamUse config
                                    if (!Config.Item(attackedHero.SkinName).GetValue<bool>())
                                    {
                                        return;
                                    }

                                    SpellDataInst spellA = attacker.Spellbook.Spells.FirstOrDefault(hero => args.SData.Name.Contains(hero.SData.Name));
                                    spellSlot = spellA == null ? SpellSlot.Unknown : spellA.Slot;

                                    SpellSlot igniteSlot = Utility.GetSpellSlot(attackerHero, ignite.menuVariable);

                                    //log.WriteLine("Attacker->" + attackerHero.BaseSkinName + "   Target->" + attackedHero.BaseSkinName + "    Spell->" + args.SData.Name + "    Slot->" + spellSlot);
                                    if (igniteSlot != SpellSlot.Unknown && spellSlot == igniteSlot)
                                    {
                                        incDmg = Damage.GetSummonerSpellDamage(attackerHero, attackedHero, Damage.SummonerSpell.Ignite);
                                        attackId = AttackId.Ignite;
                                    }

                                    else if (spellSlot == SpellSlot.Item1 || spellSlot == SpellSlot.Item2 || spellSlot == SpellSlot.Item3 || spellSlot == SpellSlot.Item4 || spellSlot == SpellSlot.Item5 || spellSlot == SpellSlot.Item6)
                                    {
                                        if (args.SData.Name == king.name)
                                        {
                                            incDmg = Damage.GetItemDamage(attackerHero, attackedHero, Damage.DamageItems.Botrk);
                                            attackId = AttackId.King;
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
                                        attackId = AttackId.Basic;
                                    }
                                    else
                                    {
                                        incDmg = Damage.GetSpellDamage(attackerHero, attackedHero, spellSlot);
                                        attackId = AttackId.Spell;
                                    }

                                    //Console.WriteLine("Slot->" + spellSlot + "  inc-> " + incDmg + " Spell-> " + args.SData.Name);// 44 = sivir w, 49 = YasuoBasicAttack3, 50 YassuoCritAttack, 45 = LeonaShieldOfDaybreakAttack
                                }
                                else if (attacker.Type == GameObjectType.obj_AI_Turret && attacker.IsEnemy && (spellTarget.IsAlly && _player.Distance(spellTarget.Position) <= 750))
                                {
                                    // TODO: Get multiplier/real dmg
                                    incDmg = attacker.BaseAttackDamage;
                                    attackId = AttackId.Tower;
                                }
                                #endregion
                            }
                        }
                        #endregion
                        #region W/O Target
                        else
                        {
                            if (attacker.Type == GameObjectType.obj_AI_Hero && attacker.IsEnemy)
                            {
                                float range1 = args.SData.CastRangeDisplayOverride;
                                float range2 = args.SData.CastRange;
                                float range = range1 > 1 ? range1 : range2;

                                // CHECK AOE ???
                                //log.WriteLine("Shot-Attacker->" + attacker.BaseSkinName + "    Spell->" + args.SData.Name + "    Range->" + range);

                                //drawPos2 = args.Start.Extend(args.End, range);
                                List<Obj_AI_Hero> alliesInRange = Utility.GetAlliesInRange(args.Start, range);
                                if (alliesInRange.Count > 0)
                                {
                                    Obj_AI_Hero attackerHero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == attacker.NetworkId);
                                    foreach (Obj_AI_Hero hero in alliesInRange)
                                    {
                                        // Check TeamUse config
                                        if (Config.Item(hero.SkinName).GetValue<bool>())
                                        {
                                            // ponto fake
                                            Vector3 fakePoint = args.Start.Extend(args.End, args.Start.Distance(hero.Position));

                                            if (hero.Position.Distance(fakePoint) <= 30)
                                            {
                                                SpellDataInst spellA = attacker.Spellbook.Spells.FirstOrDefault(spell => args.SData.Name.Contains(spell.SData.Name));
                                                spellSlot = spellA == null ? SpellSlot.Unknown : spellA.Slot;

                                                // TEMP log
                                                //log.WriteLine("Shot-Target->" + hero.BaseSkinName + "    Slot->" + spellSlot);

                                                //Calc dmg and check deffs
                                                incDmg = Damage.GetSpellDamage(attackerHero, hero, spellSlot);
                                                callDeff(attacker, hero, incDmg, spellSlot, AttackId.Spell);
                                            }
                                        }
                                    }
                                    return;
                                }
                            }
                        }
                        #endregion
                    }

                    if (incDmg > 0 || spellSlot != SpellSlot.Unknown)
                    {
                        if (spellTarget != null)
                        {
                            if (spellTarget.Team == _player.Team)
                            {
                                Obj_AI_Hero attackedHero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == spellTarget.NetworkId);

                                callDeff(attacker, attackedHero, incDmg, spellSlot, attackId);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem with MasterActivator(Receiving dmg sys.).");
                //log.WriteLine(e);
                Console.WriteLine(e);
            }
        }

        private void onDraw(EventArgs args)
        {
            try
            {
                if (Config.Item("drawStatus").IsActive())
                {
                    Drawing.DrawText(Drawing.Width - 120, 80, Config.Item("enabled").IsActive() ? System.Drawing.Color.Green : System.Drawing.Color.Red, "MActivator");
                }
                
                ksDrawRange(choR);
                ksDrawRange(nunuQ);
                ksDrawRange(amumuE);
                ksDrawRange(gragasR);
                ksDrawRange(luxR);

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
                                        Drawing.DrawLine(hpXPos, hpBarPos.Y, hpXPos, hpBarPos.Y + 5, 2, smiteDmg >= minion.Health ? System.Drawing.Color.Lime : System.Drawing.Color.WhiteSmoke);
                                        
                                        // Draw camp
                                        if (Config.Item("dCamp").IsActive())
                                        {
                                            Drawing.DrawCircle(minion.Position, minion.BoundingRadius + smite.range + _player.BoundingRadius, _player.Distance(minion, false) <= (smite.range + minion.BoundingRadius + _player.BoundingRadius) ? System.Drawing.Color.Lime : System.Drawing.Color.WhiteSmoke);
                                        }

                                        // Draw for abilitys
                                        ksDrawDmg(choR, minion, jMinion, hpBarPos, hpXPos);
                                        ksDrawDmg(nunuQ, minion, jMinion, hpBarPos, hpXPos);
                                        ksDrawDmg(amumuE, minion, jMinion, hpBarPos, hpXPos);
                                        ksDrawDmg(gragasR, minion, jMinion, hpBarPos, hpXPos);
                                        ksDrawDmg(luxR, minion, jMinion, hpBarPos, hpXPos);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                    checkAndUse(cFlaskHP, "RegenerationPotion");
                    checkAndUse(cFlaskMP, "FlaskOfCrystalWater");

                    if (!Config.Item("justPred").GetValue<bool>() || !Config.Item("predict").GetValue<bool>())
                    {
                        checkAndUse(zhonya);
                        checkAndUse(wooglet);
                        checkAndUse(barrier);
                        checkAndUse(seraph);
                        teamCheckAndUse(solari);
                        teamCheckAndUse(mountain);
                        teamCheckAndUse(mikaelHP);
                        checkAndUseShield();
                    }

                    checkAndUse(smite);
                    checkAndUse(smiteAOE);
                    checkAndUse(smiteDuel);
                    checkAndUse(smiteGanker);
                    checkAndUse(smiteQuick);
                    checkAndUse(choR);
                    checkAndUse(nunuQ);
                    checkAndUse(amumuE, "", 0, true);
                    checkAndUse(gragasR);
                    checkAndUse(luxR);

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
            checkAndUse(tiamat);
            checkAndUse(hydra);
            checkAndUse(dfg);
            checkAndUse(divine);
            checkAndUse(hextech);
            checkAndUse(muramana);
            checkAndUse(frost);
            checkAndUse(randuin);
        }

        private void ksDrawRange(MItem item)
        {
            if (Config.Item(item.menuVariable) != null)
            {
                if (Config.Item(item.menuVariable).GetValue<bool>() && Config.Item(item.menuVariable + "drawRange").GetValue<bool>())
                {
                    float range = item == choR ? getChoUltRange() : item.range;
                    Drawing.DrawCircle(_player.Position, range, System.Drawing.Color.Brown);
                }
            }
        }

        private void ksDrawDmg(MItem item, Obj_AI_Base minion, MMinion jMinion, Vector2 hpBarPos, float hpXPos)
        {
            if (Config.Item(item.menuVariable) != null)
            {
                if (Config.Item(item.menuVariable).GetValue<bool>() && Config.Item(item.menuVariable + "drawBar").GetValue<bool>())
                {
                    float spellDmg = (float)Damage.GetSpellDamage(_player, minion, item.abilitySlot);
                    var spellDmgPercent = spellDmg / minion.MaxHealth;

                    hpXPos = hpBarPos.X + (jMinion.width * spellDmgPercent);
                    Drawing.DrawLine(hpXPos, hpBarPos.Y, hpXPos, hpBarPos.Y + 5, 2, spellDmg >= minion.Health ? System.Drawing.Color.BlueViolet : System.Drawing.Color.Black);
                }
            }
        }

        private void callDeff(Obj_AI_Base attacker, Obj_AI_Hero target, double incDmg, SpellSlot spellSlot, AttackId attackId)
        {
            teamCheckAndUse(heal, Config.Item("useWithHealDebuff").GetValue<bool>() ? "" : "summonerhealcheck", incDmg);
            teamCheckAndUse(solari, "", incDmg);
            teamCheckAndUse(mountain, "", incDmg);
            teamCheckAndUse(mikaelHP, "", incDmg);
            checkAndUseShield(incDmg, attacker, target, spellSlot, attackId);

            if (target.IsMe)
            {
                checkAndUse(zhonya, "", incDmg);
                checkAndUse(wooglet, "", incDmg);
                checkAndUse(barrier, "", incDmg);
                checkAndUse(seraph, "", incDmg);
            }
        }

        // And about ignore HP% check?
        private void justUseAgainstCheck(MItem item, double incDmg, Obj_AI_Base attacker = null, Obj_AI_Base attacked = null, SpellSlot attackerSpellSlot = SpellSlot.Unknown, AttackId attackId = AttackId.Unknown)
        {
            // Se tem o spell
            if (Utility.GetSpellSlot(_player, item.name) != SpellSlot.Unknown)
            {
                if (attacker != null && attacked != null)
                {
                    bool use = false;
                    if (attackId != AttackId.Unknown)
                    {
                        switch (attackId)
                        {
                            case AttackId.Basic:
                                use = Config.Item("basic" + item.menuVariable).GetValue<bool>();
                                break;
                            case AttackId.Ignite:
                                use = Config.Item("king" + item.menuVariable).GetValue<bool>();
                                break;
                            case AttackId.King:
                                use = Config.Item("ignite" + item.menuVariable).GetValue<bool>();
                                break;
                            case AttackId.Tower:
                                use = Config.Item("tower" + item.menuVariable).GetValue<bool>();
                                break;
                            case AttackId.Spell:
                                use = Config.Item(item.menuVariable + attacker.BaseSkinName).GetValue<bool>() && Config.Item(attackerSpellSlot + item.menuVariable + attacker.BaseSkinName).GetValue<bool>();
                                break;
                        }
                    }

                    if (use)
                    {
                        bool ignoreHP = false;
                        if (attackId == AttackId.Spell)
                        {
                            ignoreHP = Config.Item("ignore" + item.menuVariable + attacker.BaseSkinName).GetValue<bool>();
                        }

                        if (item.type == ItemTypeId.Ability && attacked.IsMe)
                        {
                            checkAndUse(item, "", incDmg, ignoreHP);
                        }
                        else if (item.type == ItemTypeId.TeamAbility)
                        {
                            teamCheckAndUse(item, "", incDmg, attacked, attacker, ignoreHP);
                        }
                    }
                }
                // OFF JustPred
                else 
                {
                    checkAndUse(item, "", incDmg);
                    teamCheckAndUse(item, "", incDmg, attacked);
                }
            }
        }

        private void checkAndUseShield(double incDmg = 0, Obj_AI_Base attacker = null, Obj_AI_Base attacked = null, SpellSlot attackerSpellSlot = SpellSlot.Unknown, AttackId attackId = AttackId.Unknown)
        {
            justUseAgainstCheck(titanswraith, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(blackshield, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(unbreakable, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(palecascade, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(bulwark, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(courage, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(eyeofstorm, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(inspire, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(helppix, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(prismaticbarrier, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(commandprotect, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(spellshield, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(nocturneShield, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(yasuoShield, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(fioraRiposte, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(tryndaUlt, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(fioraDance, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(masterQ, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(lissR, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(nasusUlt, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(renekUlt, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(leonaW, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(annieE, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(vladW, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(wukongW, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(kayleR, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(nidaE, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(fizzE, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(sionW, incDmg, attacker, attacked, attackerSpellSlot, attackId);
            justUseAgainstCheck(sonaW, incDmg, attacker, attacked, attackerSpellSlot, attackId);
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
            if (item.type == ItemTypeId.Ability || item.type == ItemTypeId.TeamAbility)
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
                    menuUseAgainst.AddItem(new MenuItem("ignite" + item.menuVariable, "Ignite").SetValue(true));
                    menuUseAgainst.AddItem(new MenuItem("king" + item.menuVariable, "BoRKing").SetValue(false));
                    menuUseAgainst.AddItem(new MenuItem("basic" + item.menuVariable, "Basic ATK").SetValue(false));

                    var enemyHero = from hero in ObjectManager.Get<Obj_AI_Hero>()
                                   where hero.Team != _player.Team
                                  select hero;

                    if (enemyHero.Count() > 0)
                    {
                        foreach (Obj_AI_Hero hero in enemyHero)
                        {
                            var menuUseAgainstHero = new Menu(hero.BaseSkinName, "useAgainst" + hero.BaseSkinName);
                            menuUseAgainstHero.AddItem(new MenuItem(item.menuVariable + hero.BaseSkinName, "Enabled").SetValue(true));
                            menuUseAgainstHero.AddItem(new MenuItem(SpellSlot.Q + item.menuVariable + hero.BaseSkinName, "Q").SetValue(false));
                            menuUseAgainstHero.AddItem(new MenuItem(SpellSlot.W + item.menuVariable + hero.BaseSkinName, "W").SetValue(false));
                            menuUseAgainstHero.AddItem(new MenuItem(SpellSlot.E + item.menuVariable + hero.BaseSkinName, "E").SetValue(false));
                            menuUseAgainstHero.AddItem(new MenuItem(SpellSlot.R + item.menuVariable + hero.BaseSkinName, "R").SetValue(false));
                            menuUseAgainstHero.AddItem(new MenuItem("ignore" + item.menuVariable + hero.BaseSkinName, "Ignore %HP").SetValue(true));
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
            else if (item.type == ItemTypeId.KSAbility)
            {
                var abilitySlot = Utility.GetSpellSlot(_player, item.name);
                if (abilitySlot != SpellSlot.Unknown && abilitySlot == item.abilitySlot)
                {
                    var ksAbMenu = new Menu(item.menuName, "menu" + item.menuVariable);
                    ksAbMenu.AddItem(new MenuItem(item.menuVariable, "Enable").SetValue(true));
                    //choRMenu.AddItem(new MenuItem(choR.menuVariable + "plus", "Plus").SetValue(false));
                    ksAbMenu.AddItem(new MenuItem(item.menuVariable + "drawRange", "Draw Range").SetValue(true));
                    ksAbMenu.AddItem(new MenuItem(item.menuVariable + "drawBar", "Draw Bar").SetValue(true));
                    Config.SubMenu(parent).AddSubMenu(ksAbMenu);
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

        private void teamCheckAndUse(MItem item, String buff = "", double incDmg = 0, Obj_AI_Base attacked = null, Obj_AI_Base attacker = null, bool ignoreHP = false)
        {
            if (Config.Item(item.menuVariable) != null)
            {
                // check if is configured to use
                if (Config.Item(item.menuVariable).GetValue<bool>())
                {
                    #region DeffensiveSpell ManaRegeneratorSpell PurifierSpell
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
                    #endregion
                    #region TeamAbility TeamAbilityAOE
                    else if (item.type == ItemTypeId.TeamAbility)
                    {
                        try
                        {
                            if (!Config.Item(attacked.SkinName).GetValue<bool>())
                            {
                                return;
                            }
                            
                            if (_player.Distance(attacked, false) <= item.range)
                            {
                                var spellSlot = Utility.GetSpellSlot(_player, item.name);
                                if (spellSlot != SpellSlot.Unknown)
                                {
                                    if (_player.Spellbook.CanUseSpell(spellSlot) == SpellState.Ready)
                                    {
                                        int usePercent = !ignoreHP ? Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value : 100;
                                        int manaPercent = Config.Item(item.menuVariable + "UseManaPct") != null ? Config.Item(item.menuVariable + "UseManaPct").GetValue<Slider>().Value : 0;

                                        int actualHeroHpPercent = (int)(((attacked.Health - incDmg) / attacked.MaxHealth) * 100); //after dmg not Actual ^^
                                        int playerManaPercent = (int)((_player.Mana / _player.MaxMana) * 100);
                                        if (playerManaPercent >= manaPercent && actualHeroHpPercent <= usePercent)
                                        {
                                            if (item.type == ItemTypeId.TeamAbility && item.spellType != SpellType.SkillShotCircle && item.spellType != SpellType.SkillShotCone && item.spellType != SpellType.SkillShotLine)
                                            {
                                                _player.Spellbook.CastSpell(item.abilitySlot, attacked);
                                            }
                                            else
                                            {
                                                Vector3 pos = attacked.Position;
                                                // extend 20 to attacker direction THIS 20 COST RANGE
                                                if (attacker != null)
                                                {
                                                    if (_player.Distance(attacked.Position.Extend(attacker.Position, 20), false) <= item.range)
                                                    {
                                                        pos = attacked.Position.Extend(attacker.Position, 20);
                                                    }
                                                }
                                                _player.Spellbook.CastSpell(item.abilitySlot, pos);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Problem with MasterActivator(AutoShieldTeam).");
                            Console.WriteLine(e);
                        }
                    }
                    #endregion
                    #region Others
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
                                        #region Purifier
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
                                        #endregion
                                        #region Deffensive
                                        else if (item.type == ItemTypeId.Deffensive)
                                        {
                                            int enemyInRange = Utility.CountEnemiesInRange(hero, 700);
                                            if (enemyInRange >= 1)
                                            {
                                                int usePercent = Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value;
                                                int actualHeroHpPercent = (int)((hero.Health / hero.MaxHealth) * 100);
                                                if (actualHeroHpPercent <= usePercent)
                                                {
                                                    if (item.spellType == SpellType.Self)
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
                                        #endregion
                                    }
                                }
                            }
                        }
                    }
                    #endregion
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

        private void checkAndUse(MItem item, String buff = "", double incDamage = 0, bool ignoreHP = false)
        {
            try
            {
                if (Config.Item(item.menuVariable) != null)
                {
                    // check if is configured to use
                    if (Config.Item(item.menuVariable).GetValue<bool>())
                    {
                        int actualHeroHpPercent = (int)(((_player.Health - incDamage) / _player.MaxHealth) * 100); //after dmg not Actual ^^
                        int actualHeroManaPercent = (int)(_player.MaxMana > 0 ? ((_player.Mana / _player.MaxMana) * 100) : 0);

                        #region DeffensiveSpell ManaRegeneratorSpell PurifierSpell OffensiveSpell KSAbility
                        if (item.type == ItemTypeId.DeffensiveSpell || item.type == ItemTypeId.ManaRegeneratorSpell || item.type == ItemTypeId.PurifierSpell || item.type == ItemTypeId.OffensiveSpell || item.type == ItemTypeId.KSAbility)
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
                                                checkCCTick = Utils.TickCount + 2500;
                                            }
                                        }
                                    }
                                    else if (item.type == ItemTypeId.OffensiveSpell || item.type == ItemTypeId.KSAbility)
                                    {
                                        #region Ignite
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
                                        #endregion
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
                                                
                                                float searchRange = item == choR ? getChoUltRange() : (item.range + 300); // Get minions in 800 range

                                                var minions = MinionManager.GetMinions(_player.Position, searchRange, MinionTypes.All, MinionTeam.Neutral);
                                                if (minions.Count() > 0)
                                                {
                                                    int smiteDmg = getSmiteDmg();

                                                    foreach (Obj_AI_Base minion in minions)
                                                    {
                                                        float range = item == choR ? getChoUltRange() : item.range + minion.BoundingRadius + _player.BoundingRadius;
                                                        if (_player.Distance(minion, false) <= range)
                                                        {
                                                            int dmg = item.type == ItemTypeId.OffensiveSpell ? smiteDmg : (int)Damage.GetSpellDamage(_player, minion, spellSlot);
                                                            if (minion.Health <= dmg && jungleMinions.Any(name => minion.Name.StartsWith(name) && ((minion.Name.Length - name.Length) <= 6) && Config.Item(name).GetValue<bool>()))
                                                            {
                                                                if (item.spellType == SpellType.SkillShotLine || item.spellType == SpellType.SkillShotCone || item.spellType == SpellType.SkillShotCircle)
                                                                {
                                                                    _player.Spellbook.CastSpell(spellSlot, minion.Position);
                                                                }
                                                                else
                                                                {
                                                                    _player.Spellbook.CastSpell(spellSlot, item.spellType == SpellType.Self ? null : minion);
                                                                }
                                                            }
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
                        #endregion
                        else if (item.type == ItemTypeId.Ability || item.type == ItemTypeId.TeamAbility)
                        {
                            try
                            {
                                var spellSlot = Utility.GetSpellSlot(_player, item.name);
                                if (spellSlot != SpellSlot.Unknown)
                                {
                                    if (_player.Spellbook.CanUseSpell(spellSlot) == SpellState.Ready)
                                    {
                                        int usePercent = !ignoreHP ? Config.Item(item.menuVariable + "UseOnPercent").GetValue<Slider>().Value : 100;
                                        int manaPercent = Config.Item(item.menuVariable + "UseManaPct") != null ? Config.Item(item.menuVariable + "UseManaPct").GetValue<Slider>().Value : 0;
                                        //Console.WriteLine("ActualMana%-> " + actualHeroManaPercent + "  Mana%->" + manaPercent + "  Acthp%->" + actualHeroHpPercent + "   Use%->" + usePercent);

                                        if (actualHeroManaPercent >= manaPercent && actualHeroHpPercent <= usePercent)
                                        {
                                            if (item.spellType == SpellType.TargetEnemy)
                                            {
                                                if (checkTarget(item.range))
                                                {
                                                    _player.Spellbook.CastSpell(item.abilitySlot, target);
                                                }
                                            }
                                            else
                                            {
                                                _player.Spellbook.CastSpell(item.abilitySlot, _player);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Problem with MasterActivator(AutoShield).");
                                Console.WriteLine(e);
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
                                                useItem(item.id, (item.range == 0 || item.spellType == SpellType.Self) ? null : target);
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
                                                useItem(item.id, (item.range == 0 || item.spellType == SpellType.Self) ? null : target);
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
                                                checkCCTick = Utils.TickCount + 2500;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private float getChoUltRange()
        {
            float range = choR.range;
            BuffInstance[] buffs = _player.Buffs;
            if (buffs.Length > 0)
            {
                foreach (BuffInstance pBuff in buffs)
                {
                    if (pBuff.Name == "Feast")
                    {
                        // Please give me credits
                        //range = choR.range + ((new float[] { 3.8F, 6.1F, 8.3F }[_player.GetSpell(choR.abilitySlot).Level - 1]) * buff.Count);
                        range = choR.range + ((new float[] { 5.83F, 9.16F, 12.5F }[_player.GetSpell(choR.abilitySlot).Level - 1]) * pBuff.Count);
                        SpellDataInst spell = _player.Spellbook.GetSpell(choR.abilitySlot);
                    }
                }
            }
            return range;
        }

        private void useItem(int id, Obj_AI_Hero target = null)
        {
            try
            {
                if (Items.HasItem(id))
                {
                    if (Items.CanUseItem(id))
                    {
                        Items.UseItem(id, target);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void useItem(int id, Vector3 target)
        {
            try
            {
                if (Items.HasItem(id))
                {
                    if (Items.CanUseItem(id))
                    {
                        Items.UseItem(id, target);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
            Config.SubMenu("purify").AddItem(new MenuItem("ccDelay", "Delay(ms)").SetValue(new Slider(0, 0, 2500)));
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
            Config.SubMenu("purify").AddItem(new MenuItem("dispellExhaust", "Exhaust")).SetValue(false);
            Config.SubMenu("purify").AddItem(new MenuItem("dispellEsNumeroUno", "Es Numero Uno")).SetValue(false);

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
            menuSmiteDraw.AddItem(new MenuItem("dCamp", "Camp")).SetValue(true);
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
            createMenuItem(wooglet, "deffensive", 35);
            createMenuItem(solari, "deffensive", 45);
            createMenuItem(mountain, "deffensive", 45);
            createMenuItem(mikaelHP, "deffensive", 5);
            Config.SubMenu("deffensive").AddItem(new MenuItem("justPred", "Just Predicted")).SetValue(true);
            Config.SubMenu("deffensive").AddItem(new MenuItem("useRecalling", "Use Recalling")).SetValue(false);

            Config.AddSubMenu(new Menu("Auto Skill", "autoshield"));
            createMenuItem(blackshield, "autoshield", 100, false, 40);
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
            createMenuItem(spellshield, "autoshield", 100, false, 0);
            createMenuItem(nocturneShield, "autoshield", 90, false, 0);
            createMenuItem(yasuoShield, "autoshield", 90);
            createMenuItem(fioraRiposte, "autoshield", 90, false, 0);
            createMenuItem(tryndaUlt, "autoshield", 30);
            createMenuItem(fioraDance, "autoshield", 30);
            createMenuItem(masterQ, "autoshield", 45);
            createMenuItem(lissR, "autoshield", 20);
            createMenuItem(nasusUlt, "autoshield", 30, false, 0);
            createMenuItem(renekUlt, "autoshield", 30);
            createMenuItem(leonaW, "autoshield", 60, false, 0);
            createMenuItem(annieE, "autoshield", 60, false, 0);
            createMenuItem(vladW, "autoshield", 45);
            createMenuItem(wukongW, "autoshield", 40, false, 0);
            createMenuItem(kayleR, "autoshield", 30);
            createMenuItem(nidaE, "autoshield", 40);
            createMenuItem(fizzE, "autoshield", 40);
            createMenuItem(sionW, "autoshield", 40, false, 20);
            createMenuItem(sonaW, "autoshield", 40, false, 25);

            createMenuItem(choR, "autoshield");
            createMenuItem(nunuQ, "autoshield");
            createMenuItem(amumuE, "autoshield");
            createMenuItem(gragasR, "autoshield");
            createMenuItem(luxR, "autoshield");

            Config.AddSubMenu(new Menu("Regenerators", "regenerators"));
            createMenuItem(heal, "regenerators", 35);
            Config.SubMenu("regenerators").SubMenu("menu" + heal.menuVariable).AddItem(new MenuItem("useWithHealDebuff", "Use with debuff")).SetValue(true);
            Config.SubMenu("regenerators").SubMenu("menu" + heal.menuVariable).AddItem(new MenuItem("justPredHeal", "Just predicted")).SetValue(false);
            createMenuItem(clarity, "regenerators", 25, true);
            createMenuItem(hpPot, "regenerators", 55);
            createMenuItem(manaPot, "regenerators", 55, true);
            createMenuItem(biscuit, "regenerators", 55);
            createMenuItem(cFlaskHP, "regenerators", 40);
            createMenuItem(cFlaskMP, "regenerators", 40, true);

            Config.AddSubMenu(new Menu("Team Use", "teamUseOn"));

            var allyHeros = from hero in ObjectManager.Get<Obj_AI_Hero>()
                            where hero.IsAlly == true
                            select hero.SkinName;

            foreach (String allyHero in allyHeros)
            {
                Config.SubMenu("teamUseOn").AddItem(new MenuItem(allyHero, allyHero)).SetValue(true);
            }

            // Wards
            Config.AddSubMenu(new Menu("Wards", "wards"));
            var menu = new Menu("Akali W", "menuAkaliW");
            menu.AddItem(new MenuItem("menuAkaliW" + "UseOnPercent", "Use on HP%")).SetValue(new Slider(60, 0, 100));
            menu.AddItem(new MenuItem("menuAkaliW", "Enable").SetValue(false));
            menu.AddItem(new MenuItem("akaliWOnCombo", "Just On Combo").SetValue(true));

            var menuAkaliWWards = new Menu("Wards", "akaliRWards");
            menuAkaliWWards.AddItem(new MenuItem(akaliW.menuVariable + pink.menuVariable, pink.menuName).SetValue(true));
            menuAkaliWWards.AddItem(new MenuItem(akaliW.menuVariable + greatVisionTotem.menuVariable, greatVisionTotem.menuName).SetValue(true));

            menu.AddSubMenu(menuAkaliWWards);
            Config.SubMenu("wards").AddSubMenu(menu);

            // Combo mode
            Config.AddSubMenu(new Menu("Combo Mode", "combo"));
            Config.SubMenu("combo").AddItem(new MenuItem("comboModeActive", "Active")).SetValue(new KeyBind(32, KeyBindType.Press, true));

            // Target selector
            Config.AddSubMenu(new Menu("Target Selector", "targetSelector"));
            TargetSelector.AddToMenu(Config.SubMenu("targetSelector"));

            Config.AddItem(new MenuItem("predict", "Predict DMG")).SetValue(true);

            Config.AddItem(new MenuItem("drawStatus", "Draw Status")).SetValue(true);
            Config.AddItem(new MenuItem("enabled", "Enabled")).SetValue(new KeyBind('L', KeyBindType.Toggle, true));

            Config.AddToMainMenu();
        }

        private bool checkCC(Obj_AI_Hero hero)
        {
            bool cc = false;

            if (checkCCTick > Utils.TickCount)
            {
                Console.WriteLine("tick");
                return cc;
            }

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
                if (hero.HasBuff("zedultexecute", false))
                {
                    cc = true;
                }
            }

            if (Config.Item("dispellExhaust").GetValue<bool>())
            {
                if (hero.HasBuff(exhaust.menuVariable, false))
                {
                    cc = true;
                }
            }

            if (Config.Item("dispellEsNumeroUno").GetValue<bool>())
            {
                if (hero.HasBuff("MordekaiserCOTGPet", false))
                {
                    cc = true;
                }
            }

            checkCCTick = Utils.TickCount + Config.Item("ccDelay").GetValue<Slider>().Value;
            return cc;
        }
    }
}