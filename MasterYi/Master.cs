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
        private Obj_AI_Hero player = ObjectManager.Player;
        private Spellbook sBook;
        public Orbwalking.Orbwalker orbwalker;
        private Obj_AI_Hero target;

        private SpellDataInst Qdata;
        private SpellDataInst Wdata;
        private SpellDataInst Edata;
        private SpellDataInst Rdata;
        private Spell Q = new Spell(SpellSlot.Q, 600);
        private Spell W = new Spell(SpellSlot.W, 0);
        private Spell E = new Spell(SpellSlot.E, 0);
        private Spell R = new Spell(SpellSlot.R, 0);

        private void load()
        {
            sBook = player.Spellbook;
            Qdata = sBook.GetSpell(SpellSlot.Q);
            Wdata = sBook.GetSpell(SpellSlot.W);
            Edata = sBook.GetSpell(SpellSlot.E);
            Rdata = sBook.GetSpell(SpellSlot.R);
        }

        public void laneClear(bool useQLC)
        {
            // Verifica se o Q está pronto e está configurado para usar
            if (Q.IsReady() && useQLC)
            {
                // Obtem todos os minions do time inimigo, no range do Q
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy);
                var closestMinion = new Obj_AI_Base();
                
                // Se tiver encontrado algum minion no range
                if (allMinions.Count() > 0)
                {
                    foreach (Obj_AI_Base minion in allMinions)
                    {
                        if (allMinions.IndexOf(minion) == 0)
                        {
                            closestMinion = minion;
                        }
                        else if (player.Distance(minion.Position) < player.Distance(closestMinion.Position))
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

        public void combo(Menu comboMenu)
        {
            obtainTarget(comboMenu.Item("useQ").GetValue<bool>());
           
            // verifica se target é válido
            if (!target.IsValidTarget())
            {
                orbwalker.SetMovement(true);
                return;
            }

            // Se o orbwalker lock está ativado
            if (comboMenu.Item("orbLock").GetValue<bool>())
            {
                orbwalker.SetMovement(false);
            }

            // Se o Q está configurado para usar
            if (comboMenu.Item("useQ").GetValue<bool>())
            {
                useQ(target);
            }

            // Se o E está configurado para usar
            if (comboMenu.Item("useE").GetValue<bool>())
            {
                useE(target);
            }

            // Se o W está configurado para usar
            if (comboMenu.Item("useW").GetValue<bool>())
            {
                useW(target, comboMenu);
            }

            // Se o R está configurado para usar
            if (comboMenu.Item("useR").GetValue<bool>())
            {
                useR(target);
            }
        }

        private void useQ(Obj_AI_Hero target)
        {
            // Se o que não pode ser usado
            if (!Q.IsReady())
            {
                return;
            }

            // implementar outras verificações, para dodge e tal.
            Q.Cast(target);
        }

        private void useW(Obj_AI_Hero target, Menu comboMenu)
        {
            // Se o W não está disponível
            if (!W.IsReady())
            {
                return;
            }

            // Verifica se HP está abaixo do configurado para usar o W

            if (myHPPercent() < comboMenu.Item("useWon").GetValue<Slider>().Value)
            {
                W.Cast();

                if (comboMenu.Item("shortW").GetValue<bool>())
                {
                    float trueAARange = player.AttackRange + target.BoundingRadius;

                    if (Q.IsReady() && target.IsValidTarget() && comboMenu.Item("useQ").GetValue<bool>())
                    {
                        Q.Cast(target);
                    }
                    else if (player.Distance(target) <= trueAARange)
                    {
                        player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                    }
                }
            }
        }

        private void useE(Obj_AI_Hero target)
        {
            // Se o E não está disponível
            if (!E.IsReady())
            {
                return;
            }

            float trueAARange = player.AttackRange + target.BoundingRadius;

            // Distancia até o target
            float dist = player.Distance(target);

            if (dist < trueAARange)
            {
                E.Cast();
            }

        }

        private void useR(Obj_AI_Hero target)
        {
            // Se o R não está disponível
            if (!R.IsReady())
            {
                return;
            }

            float trueAARange = player.AttackRange + target.BoundingRadius;

            // Distancia até o target
            float dist = player.Distance(target);

            if (dist < trueAARange)
            {
                R.Cast();
            }
        }

        private Obj_AI_Hero obtainTarget(bool useQ)
        {
            // Se tem alguém no range do Q e estiver configurado para usar no combo, pega target
            if (Q.IsReady() && useQ)
            {
                target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            }
            // Pega target pelo range básico
            else
            {
                target = SimpleTs.GetTarget(250, SimpleTs.DamageType.Physical); //125
            }

            return target;
        }

        private int myHPPercent()
        {
            return (int)((player.Health / player.MaxHealth) * 100);
        }

        public Spell getSpell(String spellKey)
        {
            switch (spellKey)
            {
                case "Q":
                    return Q;
                case "W":
                    return W;
                case "E":
                    return E;
                case "R":
                    return R;
                default:
                    return Q;
            }
        }

        public Obj_AI_Hero getPlayer()
        {
            return player;
        }
    }
}
