using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;


namespace LazyYi
{
    internal class Script
    {

        public const string CharName = "MasterYi";

        public static Menu Config;

        public static Obj_AI_Hero target;

        public Script()
        {
            /* CallBAcks */
            CustomEvents.Game.OnGameLoad += onLoad;

        }

        private static void onLoad(EventArgs args)
        {

            Game.PrintChat("LazyYi - by Crisdmc");

            try
            {
                Config = new Menu("LazyYi", "MasterYi", true);
                // OrbWalker
                Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                Master.orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));
                
                // Target Selector
                var TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
                SimpleTs.AddToMenu(TargetSelectorMenu);
                Config.AddSubMenu(TargetSelectorMenu);
                
                // Combo
                Config.AddSubMenu(new Menu("Combo", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("useQ", "Use Q")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useW", "Use W")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useWon", "Use W on %")).SetValue(new Slider(35, 100, 0));
                Config.SubMenu("combo").AddItem(new MenuItem("shortW", "Short W")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useE", "Use E")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useR", "Use R")).SetValue(true);

                // Lane Clear
                Config.AddSubMenu(new Menu("Lane Clear", "laneclear"));
                Config.SubMenu("laneclear").AddItem(new MenuItem("useQLC", "Use Q")).SetValue(true);

                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Game.OnGameUpdate += OnGameUpdate;

                GameObject.OnCreate += OnCreateObject;
                GameObject.OnDelete += OnDeleteObject;
            }
            catch
            {
                Game.PrintChat("MasterYi script error!");
            }

        }

        private static void OnGameUpdate(EventArgs args)
        {

            if (Master.orbwalker.ActiveMode.ToString() == "Combo")
            {
                // Se tem alguém no range do Q, pega target
                if(Master.Q.IsReady())
                {
                    target = SimpleTs.GetTarget(Master.Q.Range, SimpleTs.DamageType.Physical);
                }
                    
                // Pega target pelo range básico
                else
                {
                    target = SimpleTs.GetTarget(250, SimpleTs.DamageType.Physical); //125
                }
                    

                Master.doCombo(target);
            }

            if (Master.orbwalker.ActiveMode.ToString() == "Mixed")
            {
                
            }

            if (Master.orbwalker.ActiveMode.ToString() == "LaneClear")
            {
                Master.doLaneClear();
            }
        }

        private static void onDraw(EventArgs args)
        {
            Drawing.DrawCircle(Master.Player.Position, Master.Q.Range, Color.Blue);
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
        }
    }
}
