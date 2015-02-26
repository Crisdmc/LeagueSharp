using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using FasterThanEminem.entity;

namespace FasterThanEminem
{
    internal class FasterThanEminem
    {
        private Dictionary<int, Champ> champMovs = new Dictionary<int, Champ>();
        private int checkTick;
        private Obj_AI_Hero[] heros;

        public FasterThanEminem()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            // Boas vindas
            Game.PrintChat("<font color='#3BB9FF'>FasterThanEminem - by Crisdmc - </font>Loaded");

            Obj_AI_Base.OnNewPath += Obj_AI_Base_OnNewPath;
            checkTick = Utils.TickCount;
            Game.OnGameUpdate += Game_OnGameUpdate;

            heros = ObjectManager.Get<Obj_AI_Hero>().ToArray();
            if (heros.Count() > 0)
            {
                foreach (Obj_AI_Hero hero in heros)
                {
                    champMovs[hero.NetworkId] = new Champ();
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if ( checkTick <= Utils.TickCount )
            {

                if (heros.Count() > 0)
                {
                    foreach (Obj_AI_Hero hero in heros)
                    {
                        int diff = champMovs[hero.NetworkId].newMovs - champMovs[hero.NetworkId].oldMovs;
                        if ( diff > 40 )
                        {
                            Game.PrintChat("Champ-><font color='#1e90ff'>" + hero.SkinName + "</font>   +Movs-><font color='#1e90ff'>" + diff + "</font>");
                        }
                        champMovs[hero.NetworkId].oldMovs = champMovs[hero.NetworkId].newMovs;
                    }
                }
                checkTick = Utils.TickCount + 10000;
            }
        }

        private void Obj_AI_Base_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.Type == GameObjectType.obj_AI_Hero)
            {
                champMovs[sender.NetworkId].newMovs += 1;
            }
        }
    }
}
