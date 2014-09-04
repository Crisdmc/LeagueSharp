using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;


namespace MasterYi
{
    internal class Script
    {

        private const string charName = "MasterYi";

        private Menu Config;

        private Master masterYi;
        private Jungle jg;

        public Script()
        {
            /* CallBacks */
            CustomEvents.Game.OnGameLoad += onLoad;
        }

        private void onLoad(EventArgs args)
        {
            // Se for o Master Yi
            if (ObjectManager.Player.BaseSkinName == charName)
            {
                masterYi = new Master();
                jg = new Jungle();

                // Boas vindas
                Game.PrintChat(string.Format("<font color='#3BB9FF'>{0} - by Crisdmc - </font>Loaded", charName));

                try
                {
                    Config = new Menu("MasterYi", "MasterYi", true);
                    // OrbWalker
                    Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                    masterYi.orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

                    // Target Selector
                    var TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
                    SimpleTs.AddToMenu(TargetSelectorMenu);
                    Config.AddSubMenu(TargetSelectorMenu);

                    // Combo
                    Config.AddSubMenu(new Menu("Combo", "combo"));
                    Config.SubMenu("combo").AddItem(new MenuItem("useQ", "Use Q")).SetValue(true);
                    Config.SubMenu("combo").AddItem(new MenuItem("useW", "Use W")).SetValue(true);
                    Config.SubMenu("combo").AddItem(new MenuItem("useE", "Use E")).SetValue(true);
                    Config.SubMenu("combo").AddItem(new MenuItem("useR", "Use R")).SetValue(true);
                    Config.SubMenu("combo").AddItem(new MenuItem("orbLock", "Orbwalk Lock")).SetValue(false);
                    Config.SubMenu("combo").AddItem(new MenuItem("usePacket", "Use Packet")).SetValue(false);

                    // W options
                    Config.AddSubMenu(new Menu("W Options", "wOptions"));
                    Config.SubMenu("wOptions").AddItem(new MenuItem("useWWhen", "")).SetValue(new StringList(new[] { "Combo", "After Attack", "Combo(AA)" }, 2));
                    Config.SubMenu("wOptions").AddItem(new MenuItem("useWon", "Use W on %")).SetValue(new Slider(40, 100, 0));
                    Config.SubMenu("wOptions").AddItem(new MenuItem("shortW", "Interrupt W")).SetValue(true);
                    Config.SubMenu("wOptions").AddItem(new MenuItem("shortWRange", "")).SetValue(new StringList(new[] { "AA Range", "300" }, 0));

                    // Lane Clear
                    Config.AddSubMenu(new Menu("Lane Clear", "laneclear"));
                    Config.SubMenu("laneclear").AddItem(new MenuItem("useQLC", "Use Q")).SetValue(true);

                    // Draw
                    Config.AddSubMenu(new Menu("Draw", "draw"));
                    Config.SubMenu("draw").AddItem(new MenuItem("drawQ", "Q")).SetValue(true);

                    // Jungle Slack
                    Config.AddSubMenu(new Menu("Jungle Slack", "slack"));
                    Config.SubMenu("slack").AddItem(new MenuItem("activeSlack", "Active(IMPLEMENTING)")).SetValue(new KeyBind("F1".ToCharArray()[0], KeyBindType.Toggle, false));

                    // Additionals
                    Config.AddSubMenu(new Menu("Additionals", "additionals"));
                    Config.SubMenu("additionals").AddItem(new MenuItem("autoUpSkill", "Auto Up Skills")).SetValue(true);
                    Config.SubMenu("additionals").AddItem(new MenuItem("autoSkillOrder", "")).SetValue(new StringList(new[] { "Q>E>W(2W)", "Q>W>E(2E)" }, 0));

                    Config.AddToMainMenu();
                }
                catch
                {
                    Game.PrintChat("MasterYi error creating menu!");
                }

                // attach events
                Drawing.OnDraw += onDraw;
                Game.OnGameUpdate += OnGameUpdate;
                CustomEvents.Unit.OnLevelUp += onLevelUpEvent;
                Orbwalking.AfterAttack += afterAttackEvent;

                // Se o auto up de skill estiver ligado
                if (Config.Item("autoUpSkill").GetValue<bool>())
                {
                    int order = Config.Item("autoSkillOrder").GetValue<StringList>().SelectedIndex;
                    masterYi.autoUpSkill(order, masterYi.player.Level);
                }
            }
        }

        private void OnGameUpdate(EventArgs args)
        {

            if (masterYi.orbwalker.ActiveMode.ToString() == "Combo")
            {
                masterYi.combo(Config); 
            }

            if (masterYi.orbwalker.ActiveMode.ToString() == "Mixed")
            {
                masterYi.mixedMode();
            }

            if (masterYi.orbwalker.ActiveMode.ToString() == "LaneClear")
            {
                masterYi.laneClear(Config.Item("useQLC").GetValue<bool>());
            }

            if (masterYi.orbwalker.ActiveMode.ToString() == "LastHit")
            {
                masterYi.lastHit();
            }

            if (Config.Item("activeSlack").GetValue<KeyBind>().Active)
            {
                //jg.teste();
            }
        }

        private void onDraw(EventArgs args)
        {
            if (Config.Item("drawQ").GetValue<bool>())
            {
                Drawing.DrawCircle(masterYi.player.Position, masterYi.Q.Range, Color.Blue);
            }
        }

        private void onLevelUpEvent(Obj_AI_Base champ, CustomEvents.Unit.OnLevelUpEventArgs evt)
        {
            // se for eu que evolui de level
            if (champ.IsMe && Config.Item("autoUpSkill").GetValue<bool>())
            {
                int order = Config.Item("autoSkillOrder").GetValue<StringList>().SelectedIndex;
                masterYi.autoUpSkill(order, evt.NewLevel);
            }
        }

        private void afterAttackEvent(Obj_AI_Base champ, Obj_AI_Base target)
        {
            int useWWhen = Config.Item("useWWhen").GetValue<StringList>().SelectedIndex;
            int useWOn = Config.Item("useWon").GetValue<Slider>().Value;
            bool usePacket = Config.Item("usePacket").GetValue<bool>();

            // Se for after attack
            if (useWWhen == 1)
            {
                masterYi.useW(useWOn, usePacket);
            }
            // Se for combo(aa)
            else if (useWWhen == 2 && masterYi.orbwalker.ActiveMode.ToString() == "Combo")
            {
                masterYi.useW(useWOn, usePacket);
            }
        }
    }
}
