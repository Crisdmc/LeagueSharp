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

        private const string CharName = "MasterYi";

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
            masterYi = new Master();
            jg = new Jungle();
            Game.PrintChat("MasterYi - by Crisdmc");

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
                Config.SubMenu("combo").AddItem(new MenuItem("useWon", "Use W on %")).SetValue(new Slider(40, 100, 0));
                Config.SubMenu("combo").AddItem(new MenuItem("shortW", "Short W")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useE", "Use E")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useR", "Use R")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("orbLock", "Orbwalk Lock")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("usePacket", "Use Packet")).SetValue(false);

                // Lane Clear
                Config.AddSubMenu(new Menu("Lane Clear", "laneclear"));
                Config.SubMenu("laneclear").AddItem(new MenuItem("useQLC", "Use Q")).SetValue(true);

                // Draw
                Config.AddSubMenu(new Menu("Draw", "draw"));
                Config.SubMenu("draw").AddItem(new MenuItem("drawQ", "Draw Q")).SetValue(true);

                // Jungle Slack
                Config.AddSubMenu(new Menu("Jungle Slack", "slack"));
                Config.SubMenu("slack").AddItem(new MenuItem("activeSlack", "Active(IMPLEMENTING)")).SetValue(new KeyBind("F1".ToCharArray()[0], KeyBindType.Toggle,false));

                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Game.OnGameUpdate += OnGameUpdate;
            }
            catch
            {
                Game.PrintChat("MasterYi script error!");
            }

        }

        private void OnGameUpdate(EventArgs args)
        {

            if (masterYi.orbwalker.ActiveMode.ToString() == "Combo")
            {
                masterYi.combo(Config.SubMenu("combo")); 
            }

            if (masterYi.orbwalker.ActiveMode.ToString() == "Mixed")
            {
                masterYi.orbwalker.SetMovement(true);
            }

            if (masterYi.orbwalker.ActiveMode.ToString() == "LaneClear")
            {
                masterYi.orbwalker.SetMovement(true);
                masterYi.laneClear(Config.Item("useQLC").GetValue<bool>());
            }

            if (Config.Item("activeSlack").GetValue<KeyBind>().Active)
            {
                //Game.PrintChat("<font color='#F7A100'>Slack on development phase!</font>");
                masterYi.orbwalker.SetMovement(true);
                //jg.teste();
            }
        }

        private void onDraw(EventArgs args)
        {
            if (Config.Item("drawQ").GetValue<bool>())
            {
                Drawing.DrawCircle(masterYi.player.Position, masterYi.Q.Range, Color.Blue);
            }
            Drawing.DrawCircle(masterYi.player.Position, 300, Color.Blue);
        }
    }
}
