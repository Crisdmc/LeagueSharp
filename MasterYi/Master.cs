using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;

namespace LazyYi
{
    class Master
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Spellbook sBook = Player.Spellbook;

        public static Orbwalking.Orbwalker orbwalker;

        public static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        public static Spell Q = new Spell(SpellSlot.Q, 600);
        public static Spell W = new Spell(SpellSlot.W, 0);
        public static Spell E = new Spell(SpellSlot.E, 0);
        public static Spell R = new Spell(SpellSlot.R, 0);

        public static void doLaneClear()
        {
            // Verifica se o Q está pronto e está configurado para usar
            if (Master.Q.IsReady() && Script.Config.Item("useQLC").GetValue<bool>())
            {
                // Obtem todos os minions do time inimigo, no range do Q
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Master.Q.Range, MinionTypes.All, MinionTeam.Enemy);
                var closestMinion = new Obj_AI_Base();
                
                if (allMinions.Count() > 0)
                {
                    foreach (Obj_AI_Base minion in allMinions)
                    {
                        if (allMinions.IndexOf(minion) == 0)
                        {
                            closestMinion = minion;
                        }
                        else if (Master.Player.Distance(minion.Position) < Master.Player.Distance(closestMinion.Position))
                        {
                            closestMinion = minion;
                        }
                    }
                    if (!closestMinion.IsValidTarget())
                    {
                        return;
                    }
                    else
                    {
                        Q.Cast(closestMinion);
                    }
                }
            }
        }

        public static void doCombo(Obj_AI_Hero target)
        {
            // verifica se target é válido
            if (!target.IsValidTarget())
            {
                return;
            }

            // Se o Q está configurado para usar
            if (Script.Config.Item("useQ").GetValue<bool>())
            {
                useQ(target);
            }

            // Se o E está configurado para usar
            if (Script.Config.Item("useE").GetValue<bool>())
            {
                useE(target);
            }

            // Se o W está configurado para usar
            if (Script.Config.Item("useW").GetValue<bool>())
            {
                useW(target);
            }

            // Se o R está configurado para usar
            if (Script.Config.Item("useR").GetValue<bool>())
            {
                useR(target);
            }
        }

        public static void useQ(Obj_AI_Hero target)
        {
            // Se o que não pode ser usado
            if (!Q.IsReady())
            {
                return;
            }

            // implementar outras verificações, para dodge e tal.
            Q.Cast(target);
        }

        public static void useR(Obj_AI_Hero target)
        {
            // Se o R não está disponível
            if (!R.IsReady())
            {
                return;
            }

            float trueAARange = Player.AttackRange + target.BoundingRadius;

            // Distancia até o target
            float dist = Player.Distance(target);

            if (dist < trueAARange)
            {
                R.Cast();
            }
        }

        public static void useE(Obj_AI_Hero target)
        {
            // Se o E não está disponível
            if (!E.IsReady())
            {
                return;
            }

            float trueAARange = Player.AttackRange + target.BoundingRadius;

            // Distancia até o target
            float dist = Player.Distance(target);
            
            if (dist < trueAARange) 
            {
                E.Cast();
            }

        }

        public static void useW(Obj_AI_Hero target)
        {
            // Se o W não está disponível
            if (!W.IsReady())
            {
                return;
            }

            // Verifica se HP está abaixo do configurado para usar o W
            if (myHPPercent() < Script.Config.Item("useWon").GetValue<Slider>().Value)
            {
                W.Cast();
            }
        }

        public static int myHPPercent()
        {
            return (int)((Player.Health / Player.MaxHealth) * 100);
        }
    }
}
