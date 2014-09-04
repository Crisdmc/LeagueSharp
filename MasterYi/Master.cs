using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;

namespace MasterYi
{
    class Master
    {
        private Obj_AI_Hero _player = ObjectManager.Player;
        private Spellbook sBook;
        public Orbwalking.Orbwalker orbwalker;
        private Obj_AI_Hero target;

        private SpellDataInst qData;
        private SpellDataInst wData;
        private SpellDataInst eData;
        private SpellDataInst rData;
        private Spell _Q = new Spell(SpellSlot.Q, 600);
        private Spell _W = new Spell(SpellSlot.W, 0);
        private Spell _E = new Spell(SpellSlot.E, 0);
        private Spell _R = new Spell(SpellSlot.R, 0);

        public Master()
        {
            load();
        }

        private void load()
        {
            sBook = _player.Spellbook;
            qData = sBook.GetSpell(SpellSlot.Q);
            wData = sBook.GetSpell(SpellSlot.W);
            eData = sBook.GetSpell(SpellSlot.E);
            rData = sBook.GetSpell(SpellSlot.R);
        }

        public void laneClear(bool useQLC)
        {
            setOrbwalkToCursorPos();

            // Verifica se o Q está pronto e está configurado para usar
            if (_Q.IsReady() && useQLC)
            {
                // Obtem todos os minions do time inimigo, no range do Q
                var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _Q.Range, MinionTypes.All, MinionTeam.Enemy);
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
                        else if (_player.Distance(minion.Position) < _player.Distance(closestMinion.Position))
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
                        _Q.Cast(closestMinion);
                    }
                }
            }
        }

        public void combo(Menu menu)
        {
            bool comboUseQ = menu.Item("useQ").GetValue<bool>();
            bool usePacket = menu.Item("usePacket").GetValue<bool>();
            
            obtainTarget(comboUseQ);

            if (target != null)
            {
                // Se está "castando" e o último "castado" é o W, e a opção shortW está ativada
                if (sBook.IsCastingSpell && (_player.LastCastedSpellName() == wData.Name) && menu.Item("shortW").GetValue<bool>())
                {
                    int shortWRangeOpt = menu.Item("shortWRange").GetValue<StringList>().SelectedIndex;
                    float trueAARange = _player.AttackRange + target.BoundingRadius;
                    interruptW(comboUseQ, usePacket, shortWRangeOpt == 0 ? trueAARange : 300);
                }

                // Se o orbwalker lock está ativado
                if (menu.Item("orbLock").GetValue<bool>() && target.Distance(Game.CursorPos) < 200)
                {
                    setOrbwalkingToTargetPos();
                }
                else
                {
                    setOrbwalkToCursorPos();
                }

                // Se o Q está configurado para usar
                if (comboUseQ)
                {
                    useQ(target, usePacket);
                }

                // Se o E está configurado para usar
                if (menu.Item("useE").GetValue<bool>())
                {
                    useE(target, usePacket);
                }

                // Se o W está configurado para usar e está configurado para usar no combo
                int useWWhen = menu.Item("useWWhen").GetValue<StringList>().SelectedIndex;
                if (menu.Item("useW").GetValue<bool>() && useWWhen == 0)
                {
                    int useWOn = menu.Item("useWon").GetValue<Slider>().Value;
                    useW(useWOn, usePacket);
                }

                // Se o R está configurado para usar
                if (menu.Item("useR").GetValue<bool>())
                {
                    useR(target, usePacket);
                }
            }
            else
            {
                setOrbwalkToCursorPos();
            }
        }

        public void mixedMode()
        {
            setOrbwalkToCursorPos();
        }

        public void lastHit()
        {
            setOrbwalkToCursorPos();
        }

        public void setOrbwalkToCursorPos()
        {
            orbwalker.SetOrbwalkingPoint(Game.CursorPos);
        }
        public void setOrbwalkingToTargetPos()
        {
            orbwalker.SetOrbwalkingPoint(target.Position);
        }

        private void useQ(Obj_AI_Hero target, bool packet)
        {
            // Se o que não pode ser usado
            if (!_Q.IsReady())
            {
                return;
            }

            // implementar outras verificações, para dodge e tal.
            _Q.Cast(target, packet);
        }

        public void useW(int useWOn, bool usePacket)
        {
            // Se o W não está disponível
            if (!_W.IsReady())
            {
                return;
            }

            // Verifica se HP está abaixo do configurado para usar o W
            if (myHPPercent() < useWOn)
            {
                _W.Cast(_player, usePacket);
            }
        }

        private void useE(Obj_AI_Hero target, bool usePacket)
        {
            // Se o E não está disponível
            if (!_E.IsReady())
            {
                return;
            }

            float trueAARange = _player.AttackRange + target.BoundingRadius;

            // Distancia até o target
            float dist = _player.Distance(target);

            if (dist < trueAARange)
            {
                _E.Cast(_player, usePacket);
            }

        }

        private void useR(Obj_AI_Hero target, bool usePacket)
        {
            // Se o R não está disponível
            if (!_R.IsReady())
            {
                return;
            }

            float trueAARange = _player.AttackRange + target.BoundingRadius;

            // Distancia até o target
            float dist = _player.Distance(target);

            if (dist < trueAARange)
            {
                _R.Cast(_player, usePacket);
            }
        }

        private Obj_AI_Hero obtainTarget(bool useQ)
        {
            // Se tem alguém no range do Q e estiver configurado para usar no combo, pega target
            if (_Q.IsReady() && useQ)
            {
                target = SimpleTs.GetTarget(_Q.Range, SimpleTs.DamageType.Physical);
            }
            // Pega target para ataque básico
            else
            {
                target = SimpleTs.GetTarget(300, SimpleTs.DamageType.Physical); //125 range básico
            }

            return target;
        }

        private void interruptW(bool useQ, bool usePacket, float range)
        {
            if (_Q.IsReady() && target.IsValidTarget() && useQ)
            {
                // Mover para garantir que vai parar o cast do W.
                _player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                _Q.Cast(target, usePacket);
            }
            else if (_player.Distance(target) <= range)
            {
                _player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
            }
        }
        
        private int myHPPercent()
        {
            return (int)((_player.Health / _player.MaxHealth) * 100);
        }

        /* Order-> 0 = QEW(2W)  1 = QWE(2E)*/
        public void autoUpSkill(int order, int newLevel)
        {
            List<int> firstOrderLevels = new List<int>() { 1, 3, 5, 7, 9 };
            List<int> secondOrderLevels = new List<int>() { 4, 8, 10, 12, 13 };
            List<int> thirdOrderLevels = new List<int>() { 2, 14, 15, 17, 18 };
            
            // Se estiver em nível de evoluir determinado skill
            if (firstOrderLevels.Contains(newLevel))
            {
                sBook.LevelUpSpell(SpellSlot.Q);
            }
            else if (secondOrderLevels.Contains(newLevel))
            {
                sBook.LevelUpSpell(order == 0 ? SpellSlot.E : SpellSlot.W);
            }
            else if (thirdOrderLevels.Contains(newLevel))
            {
                sBook.LevelUpSpell(order == 0 ? SpellSlot.W : SpellSlot.E);
            }
            else
            {
                sBook.LevelUpSpell(SpellSlot.R);
            }
        }

        public Spell Q
        {
            get
            {
                return _Q;
            }
        }
        
        public Spell W
        {
            get
            {
                return _W;
            }
        }

        public Spell E
        {
            get
            {
                return _E;
            }
        }

        public Spell R
        {
            get
            {
                return _R;
            }
        }

        public Obj_AI_Hero player
        {
            get
            {
                return _player;
            }
        }
    }
}
