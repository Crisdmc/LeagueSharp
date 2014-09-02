using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;
using MasterYi.entity;

namespace MasterYi
{
    class Jungle
    {

        private Obj_AI_Hero _player = ObjectManager.Player;

        // Campos
        private JungleCamp blueCamp;
        private JungleCamp greatGhostCamp;
        private JungleCamp wolvesCamp;
        private JungleCamp ghostsCamp;
        private JungleCamp redCamp;
        private JungleCamp golensCamp;

        public Jungle()
        {
            NeutralMinionCamp.OnCreate += onCreateJungleCamp;
            NeutralMinionCamp.OnDelete += onDeleteJungleCamp;

            if ( _player.Team == GameObjectTeam.Order)
            {
                // Cria os campos do lado azul
                createOrderCamps();
            }
            else
            {
                // Cria os campos do lado roxo
                createChaosCamps();
            }
        }

        private void onCreateJungleCamp(GameObject camp, EventArgs args)
        {
            if (camp.Team == GameObjectTeam.Neutral)
            {
            }
        }

        private void onDeleteJungleCamp(GameObject camp, EventArgs args)
        {
            if (camp.Team == GameObjectTeam.Neutral && camp.IsDead)
            {
                jungleMinionDied(camp.Name);
            }
        }

        private void jungleMinionDied(String name)
        {

        }

        public void teste()
        {
            
            float gameTime = Game.Time;

            _player.IssueOrder(GameObjectOrder.MoveTo, blueCamp.position);

            
            var testando = ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.Team == GameObjectTeam.Neutral);
            List<Obj_AI_Minion> minions = testando.ToList();

            if (minions.Count() > 0)
            {
                foreach (Obj_AI_Minion minion in minions)
                {
                }
            }
            
        }

        private void createOrderCamps()
        {
            blueCamp = new JungleCamp
            {
                position = new Vector3(3632.7F, 7600.373F, 54.173F),
                spawnTime = TimeSpan.FromSeconds(115),
                respawnTime = TimeSpan.FromSeconds(300),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("AncientGolem1.1.1"),
                    new JungleMinion("YoungLizard1.1.2"),
                    new JungleMinion("YoungLizard1.1.3")
                }
            };

            greatGhostCamp = new JungleCamp
            {
                position = new Vector3(1684F, 8207F, 54.92368F),
                spawnTime = TimeSpan.FromSeconds(125),
                respawnTime = TimeSpan.FromSeconds(50),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("GreatWraith13.1.1")
                }
            };

            wolvesCamp = new JungleCamp
            {
                position = new Vector3(3373.678F, 6223.346F, 55.60942F),
                spawnTime = TimeSpan.FromSeconds(125),
                respawnTime = TimeSpan.FromSeconds(50),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("GiantWolf2.1.1"),
                    new JungleMinion("Wolf2.1.2"),
                    new JungleMinion("Wolf2.1.3")
                }
            };

            ghostsCamp = new JungleCamp
            {
                position = new Vector3(6446.097F, 5214.808F, 56.04607F),
                spawnTime = TimeSpan.FromSeconds(125),
                respawnTime = TimeSpan.FromSeconds(50),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("Wraith3.1.1"),
                    new JungleMinion("LesserWraith3.1.2"),
                    new JungleMinion("LesserWraith3.1.3"),
                    new JungleMinion("LesserWraith3.1.4")
                }
            };

            redCamp = new JungleCamp
            {
                position = new Vector3(7455.615F, 3890.203F, 56.86591F),
                spawnTime = TimeSpan.FromSeconds(115),
                respawnTime = TimeSpan.FromSeconds(300),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("LizardElder4.1.1"),
                    new JungleMinion("YoungLizard4.1.2"),
                    new JungleMinion("YoungLizard4.1.3")
                }
            };

            golensCamp = new JungleCamp
            {
                position = new Vector3(7455.615F, 3890.203F, 56.86591F),
                spawnTime = TimeSpan.FromSeconds(115),
                respawnTime = TimeSpan.FromSeconds(300),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("Golem5.1.2"),
                    new JungleMinion("SmallGolem5.1.1")
                }
            };
        }

        private void createChaosCamps()
        {
            blueCamp = new JungleCamp
            {
                position = new Vector3(10386.61F, 6811.112F, 54.8691F),
                spawnTime = TimeSpan.FromSeconds(115),
                respawnTime = TimeSpan.FromSeconds(300),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("AncientGolem7.1.1"),
                    new JungleMinion("YoungLizard7.1.2"),
                    new JungleMinion("YoungLizard7.1.3")
                }
            };

            greatGhostCamp = new JungleCamp
            {
                position = new Vector3(12337F, 6263F, 54.81839F),
                spawnTime = TimeSpan.FromSeconds(125),
                respawnTime = TimeSpan.FromSeconds(50),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("GreatWraith4.1.1")
                }
            };

            wolvesCamp = new JungleCamp
            {
                position = new Vector3(10696.1F, 7964.808F, 65.09323F),
                spawnTime = TimeSpan.FromSeconds(125),
                respawnTime = TimeSpan.FromSeconds(50),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("GiantWolf8.1.1"),
                    new JungleMinion("Wolf8.1.2"),
                    new JungleMinion("Wolf8.1.3")
                }
            };

            ghostsCamp = new JungleCamp
            {
                position = new Vector3(7580.368F, 9250.405F, 55.48497F),
                spawnTime = TimeSpan.FromSeconds(125),
                respawnTime = TimeSpan.FromSeconds(50),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("Wraith9.1.1"),
                    new JungleMinion("LesserWraith9.1.2"),
                    new JungleMinion("LesserWraith9.1.3"),
                    new JungleMinion("LesserWraith9.1.4")
                }
            };

            redCamp = new JungleCamp
            {
                position = new Vector3(6504.241F, 10584.56F, 54.635F),
                spawnTime = TimeSpan.FromSeconds(115),
                respawnTime = TimeSpan.FromSeconds(300),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("LizardElder10.1.1"),
                    new JungleMinion("YoungLizard10.1.2"),
                    new JungleMinion("YoungLizard10.1.3")
                }
            };

            golensCamp = new JungleCamp
            {
                position = new Vector3(6140.464F, 11935.47F, 39.59138F),
                spawnTime = TimeSpan.FromSeconds(115),
                respawnTime = TimeSpan.FromSeconds(300),
                minions = new List<JungleMinion>
                {
                    new JungleMinion("Golem11.1.2"),
                    new JungleMinion("SmallGolem11.1.1")
                }
            };
        }
    }
}
