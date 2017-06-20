using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DarkVayne.SpellManager;
using static DarkVayne.MenuManager;
using static DarkVayne.Drawing;
using HesaEngine.SDK.GameObjects;
using SharpDX;
using static HesaEngine.SDK.TargetSelector;
using static HesaEngine.SDK.NavMesh;
using static HesaEngine.SDK.Orbwalker;
using System.Runtime.InteropServices;
using SharpDX.DirectInput;
using HesaEngine.SDK.Args;

namespace DarkVayne
{

    public class Main : IScript
    {
        //Don't forget to change the champion first!
        private readonly Champion champion = Champion.Vayne;

        public string Name => "DarkSeries Vayne";

        public string Version => "1.0.0";

        public string Author => "DarkPunisher";

        public static bool check = false;

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(SharpDX.DirectInput.Key vKey);

        public static double timer = 0;

        public static List<AttackableUnit> WEnemy = new List<AttackableUnit>();

        public static int ComboMode = 1;

        public static double KeyPre = 0;

        public static List<string> Names = new List<string> { "QMODE : SAFE", "QMODE : MEDIUM", "QMODE : RISKY" };

        public static PredictionInput QPred, WPred, EPred, RPred;

        public static Orbwalker.OrbwalkerInstance orb;

        public void OnInitialize()
        {
            Game.OnGameLoaded += Game_OnGameLoaded;
        }

        private void Game_OnGameLoaded()
        {
            if (ObjectManager.Me.Hero != champion)
                return;

            LoadMenu();

            LoadSpells();

            //HesaEngine.SDK.AntiGapcloser.OnEnemyGapcloser += Modes.AntiGapcloser.DoAntigapclose;

            LoadDrawings();

            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.AfterAttack += Orbwalker_AfterAttack;
            //Obj_AI_Base.OnBuffGained += OnBuff;
            Interrupter.OnInterruptableTarget += Interrupt;
            
            Chat.Print("<font color='#ffffff'>"+ Name + "</font> <font color='#f8a101'>by"+ Author + "</font><font color='#f8a101'> - Loaded</font>");
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static bool LineEquations(AIHeroClient enemy, Vector3 playerPosi, AIHeroClient player, float distance) // push distance from enemy
        {
            bool wall = false;
            //
            //for (int i = 1; i < distance - 1; i++)
            //{
            //    Vector3 ect = enemy.Position.Extend(playerPosi, -1*i);
            //    
            //    if (ect.IsWall() == true)
            //    {
            //        wall = true;
            //        break;
            //    }
            //}

            Vector3 epos = enemy.Position;
            Vector3 ppos = playerPosi;
            Vector2 epo = (epos).To2D();
            Vector2 ppo = (ppos).To2D();
            
            float x1 = ppo.X;
            float y1 = ppo.Y;
            float x2 = epo.X;
            float y2 = epo.Y;
            
            float m = (y2 - y1) / (x2 - x1);
            float c = y1 - m * x1;
            Vector3 pos = enemy.Position.Extend(playerPosi, -distance);
            Vector2 checkPos;
            
            if (pos.X > x2)
            {
                for (float i = x2; i <= pos.X; i++)
                {
                    y2 = m * i + c;
                    checkPos.X = i;
                    checkPos.Y = y2;
                    Vector3 check = (checkPos).To3D();
            
                    if (check.IsWall())
                    {
                        wall = true;
                        break;
                    }
                    else
                    {
                        wall = false;
                    }
                }
            }
            if (pos.X < x2)
            {
                for (float i = x2; i >= pos.X; i--)
                {
                    y2 = m * i + c;
                    checkPos.X = i;
                    checkPos.Y = y2;
                    Vector3 check = (checkPos).To3D();
                    if (check.IsWall())
                    {
                        wall = true;
                        break;
                    }
                    else
                    {
                        wall = false;
                    }
                }
            }
            return wall;
        }

        public static bool buildingChecks(AIHeroClient enemy, Vector3 playerPosi, AIHeroClient player, float distance)
        {
            bool collision = false;
            Vector3 porte = enemy.Position.Extend(playerPosi, -distance);
            Vector2 from = (enemy.Position).To2D();
            Vector2 to = (porte).To2D();
            float m = ((to.Y - from.Y) / (to.X - from.X));
            float x;
            float y;
            float m2 = (-(to.X - from.X) / (to.Y - from.Y));
            var minions = ObjectManager.Turrets.All;
            var heros = ObjectManager.Get<Obj_Building>();
            var mnexus = ObjectManager.AllyNexus;
            var enexus = ObjectManager.EnemyNexus;
            foreach (var minion in minions)
            {
                Vector3 minionP = minion.Position;
                Vector2 minionPos = (minionP).To2D();
                float px = minionPos.X;
                float py = minionPos.Y;
                x = ((m2 * px) - (from.X * m) + (from.Y - py)) / (m2 - m);
                y = m * (x - from.X) + from.Y;
                Vector2 colliPose;
                colliPose.X = x;
                colliPose.Y = y;
                if (colliPose.Distance(minionPos) <= enemy.BoundingRadius + minion.BoundingRadius - 30 && colliPose.Distance(from) <= distance && colliPose.Distance((playerPosi).To2D()) > (enemy.Position).To2D().Distance(colliPose)
                    && playerPosi.Distance(enemy.Position) < playerPosi.To2D().Distance(colliPose))
                {
                    collision = true;
                    break;
                }
            }
            foreach (var hero in heros)
            {
                Vector3 heroP;
                heroP = hero.Position;
                Vector2 heroPos = (heroP).To2D();
                float herox = heroPos.X;
                float heroy = heroPos.Y;
                x = ((m2 * herox) - (from.X * m) + (from.Y - heroy)) / (m2 - m);
                y = m * (x - from.X) + from.Y;
                Vector2 colliPose;
                colliPose.X = x;
                colliPose.Y = y;
                if (hero != mnexus && hero != enexus && colliPose.Distance(heroPos) <= enemy.BoundingRadius + hero.BoundingRadius - 100 && colliPose.Distance(from) <= distance && colliPose.Distance((playerPosi).To2D()) > (enemy.Position).To2D().Distance(colliPose)
                    && playerPosi.Distance(enemy.Position) < playerPosi.To2D().Distance(colliPose))
                {
                    collision = true;
                    break;
                }
            }
            Vector3 mnexusP;
            mnexusP = mnexus.Position;
            Vector2 mnexusPos = (mnexusP).To2D();
            float mnexusx = mnexusPos.X;
            float mnexusy = mnexusPos.Y;
            x = ((m2 * mnexusx) - (from.X * m) + (from.Y - mnexusy)) / (m2 - m);
            y = m * (x - from.X) + from.Y;
            Vector2 colliPos;
            colliPos.X = x;
            colliPos.Y = y;
            if (colliPos.Distance(mnexusPos) <= enemy.BoundingRadius + mnexus.BoundingRadius - 100 && colliPos.Distance(from) <= distance && colliPos.Distance((playerPosi).To2D()) > (enemy.Position).To2D().Distance(colliPos)
                && playerPosi.Distance(enemy.Position) < playerPosi.To2D().Distance(colliPos))
            {
                collision = true;
            }
            Vector3 enexusP;
            enexusP = enexus.Position;
            Vector2 enexusPos = (enexusP).To2D();
            float enexusx = enexusPos.X;
            float enexusy = enexusPos.Y;
            x = ((m2 * enexusx) - (from.X * m) + (from.Y - enexusy)) / (m2 - m);
            y = m * (x - from.X) + from.Y;
            Vector2 colliPos1;
            colliPos1.X = x;
            colliPos1.Y = y;
            if (colliPos1.Distance(enexusPos) <= enemy.BoundingRadius + enexus.BoundingRadius - 100 && colliPos1.Distance(from) <= distance && colliPos1.Distance((playerPosi).To2D()) > (enemy.Position).To2D().Distance(colliPos1)
                && playerPosi.Distance(enemy.Position) < playerPosi.To2D().Distance(colliPos1))
            {
                collision = true;
            }
            return collision;
        }

        public static int EnemiesInRange(float range, Vector3 Source)
        {
            var Targets = ObjectManager.Heroes.Enemies;
            var player = ObjectManager.Player;
            var enemiesInRange = 0;

            foreach (var target in Targets)
            {
                if (target != null && !target.IsDead && target.IsValidTarget(range) && target.IsInRange(player, range) && target.IsChampion())
                {
                    var flDistance = (target.Position - Source).Length();
                    if (flDistance < range)
                    {
                        enemiesInRange++;
                    }
                }
            }
            return enemiesInRange;
        }

        public static Vector3 SmartQ()
        {
            AIHeroClient player = ObjectManager.Player;
            TargetSelector.Mode = TargetSelector.TargetingMode.LessAttack;
            AIHeroClient enemys = GetTarget(player.AttackRange, DamageType.Magical, false);

            var enemySafe = ObjectManager.Heroes.Enemies.Where(enemy => enemy != null && enemy.IsValidTarget(player.AttackRange)).MinOrDefault(enemy => enemy.Position.Distance(player.Position));

            GameObject temporar = ObjectManager.EnemyNexus;

            Vector3 empt;
            empt.X = 0;
            empt.Y = 0;
            empt.Z = 0;

            var closestTurret = ObjectManager.Turrets.Ally.Where(turret => !turret.IsDead).MinOrDefault(turret => ObjectManager.Player.Distance(turret));

            if (ComboMode == 0)
            {
                if (enemySafe != null && enemys != null && !enemys.IsDead && enemys.IsValidTarget(player.AttackRange + Q.Range) && !enemys.IsZombie
                        && (comboMenu.GetCheckbox("ComboQ") || harassMenu.GetCheckbox("HarassQ")) && comboMenu.GetCheckbox("QSmart") && Q.IsReady())
                {
                    if (E.IsReady() && ((comboMenu.GetCheckbox("ComboQ") && comboMenu.GetCheckbox("QSmart")) || (harassMenu.GetCheckbox("HarassQ") && comboMenu.GetCheckbox("QSmart"))))
                    {
                        List<Vector3> stun = new List<Vector3>();


                        for (int i = 5; i < 360; i += 5)
                        {
                            Vector2 postQ1;
                            Vector2 posQ1;

                            postQ1 = enemys.Position.To2D().RotateAroundPoint(player.Position.To2D(), i);
                            posQ1 = player.Position.To2D().Extend(postQ1, Q.Range);
                            if (LineEquations(enemys, posQ1.To3D(), player, 425) == true   ||  buildingChecks(enemys, posQ1.To3D(), player, 425) == true)
                            {

                                stun.Add(posQ1.To3D());
                            }

                        }
                        if (stun.Count != 0)
                        {
                            
                            double cnt = (stun.Count / 2) - 1;
                            int cvrt = Convert.ToInt32(Math.Ceiling(cnt));
                            
                            return stun[cvrt];
                        }
                    }
                    if (EnemiesInRange(1500, player.Position) > 2)
                    {
                        if (EnemiesInRange(player.AttackRange + Q.Range, player.Position) >= 2)
                        {
                            List<Vector3> temp = new List<Vector3>();
                            for (int i = 5; i < 360; i += 5)
                            {
                                Vector3 posteQ1;
                                Vector3 poseQ1;
                                posteQ1 = (enemySafe.Position.To2D().RotateAroundPoint(player.Position.To2D(), i)).To3D();
                                poseQ1 = player.Position.Extend(posteQ1, Q.Range);
                                if (enemySafe.Position.Distance(poseQ1) >= player.AttackRange - 150 && enemySafe.Position.Distance(poseQ1) <= player.AttackRange - 100/* && !poseQ1.IsWall()*/)
                                {
                                    temp.Add(poseQ1);
                                }
                            }
                            if (temp.Count == 0 && enemySafe.Position.Distance(player.Position) < 300)
                            {
                                return player.Position.Extend(enemySafe.Position, -450);
                            }
                            else if (temp.Count == 0 && enemySafe.Position.Distance(player.Position) > 300)
                            {
                                return Game.CursorPosition;
                            }
                            Vector3 tem = ObjectManager.EnemyNexus.Position;
                            for (int y = 0; y < temp.Count; y++)
                            {
                                if (closestTurret != null && closestTurret.Position.Distance(tem) > closestTurret.Position.Distance(temp[y]))
                                {
                                    tem = temp[y];
                                }
                            }
                            return tem;
                        }
                        else
                            return Game.CursorPosition;
                    }
                    else
                        return Game.CursorPosition;
                }
                else if (((comboMenu.GetCheckbox("ComboQ") && Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.Combo) || (Core.Orbwalker.ActiveMode == OrbwalkingMode.Harass && harassMenu.GetCheckbox("HarassQ"))) && !comboMenu.GetCheckbox("QSmart"))
                {
                    return Game.CursorPosition;
                }
                else
                    return empt;
            }
            else if (ComboMode == 1)
            {
                if (enemys != null && !enemys.IsDead && enemys.IsValidTarget(player.AttackRange + Q.Range) && !enemys.IsZombie
                        && (comboMenu.GetCheckbox("ComboQ") || harassMenu.GetCheckbox("HarassQ")) && comboMenu.GetCheckbox("QSmart") && Q.IsReady())
                {
                    if (E.IsReady() && ((comboMenu.GetCheckbox("ComboQ") && comboMenu.GetCheckbox("QSmart")) || (harassMenu.GetCheckbox("HarassQ") && comboMenu.GetCheckbox("QSmart"))))
                    {
                        List<Vector3> stun = new List<Vector3>();


                        for (int i = 5; i < 360; i += 5)
                        {
                            Vector2 postQ1;
                            Vector2 posQ1;

                            postQ1 = enemys.Position.To2D().RotateAroundPoint(player.Position.To2D(), i);
                            posQ1 = player.Position.To2D().Extend(postQ1, Q.Range);
                            if (LineEquations(enemys, posQ1.To3D(), player, 425) == true   ||  buildingChecks(enemys, posQ1.To3D(), player, 425) == true)
                            {

                                stun.Add(posQ1.To3D());
                            }

                        }
                        if (stun.Count != 0)
                        {
                            double cnt = (stun.Count / 2) - 1;
                            int cvrt = Convert.ToInt32(Math.Ceiling(cnt));
                            return stun[cvrt];
                        }
                    }
                    if (EnemiesInRange(1500, player.Position) > 2)
                    {
                        if (EnemiesInRange(player.AttackRange + Q.Range, player.Position) >= 2)
                        {
                            List<Vector3> temp = new List<Vector3>();
                            for (int i = 5; i < 360; i += 5)
                            {
                                Vector3 posteQ1;
                                Vector3 poseQ1;
                                posteQ1 = (enemys.Position.To2D().RotateAroundPoint(player.Position.To2D(), i)).To3D();
                                poseQ1 = player.Position.Extend(posteQ1, Q.Range);
                                if (enemys.Position.Distance(poseQ1) >= player.AttackRange - 150 && enemys.Position.Distance(poseQ1) <= player.AttackRange - 100/* && !poseQ1.IsWall()*/)
                                {
                                    temp.Add(poseQ1);
                                }
                            }
                            if (temp.Count == 0 && enemys.Position.Distance(player.Position) < 300)
                            {
                                return player.Position.Extend(enemys.Position, -450);
                            }
                            else if (temp.Count == 0 && enemys.Position.Distance(player.Position) > 300)
                            {
                                return Game.CursorPosition;
                            }
                            Vector3 tem = ObjectManager.EnemyNexus.Position;
                            for (int y = 0; y < temp.Count; y++)
                            {
                                if (closestTurret != null && closestTurret.Position.Distance(tem) > closestTurret.Position.Distance(temp[y]))
                                {
                                    tem = temp[y];
                                }
                            }
                            return tem;
                        }
                        else
                            return Game.CursorPosition;
                    }
                    else
                        return Game.CursorPosition;
                }
                else if (((comboMenu.GetCheckbox("ComboQ") && Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.Combo) || (Core.Orbwalker.ActiveMode == OrbwalkingMode.Harass && harassMenu.GetCheckbox("HarassQ"))) && !comboMenu.GetCheckbox("QSmart"))
                {
                    return Game.CursorPosition;
                }
                else
                    return empt;
            }
            else if (ComboMode == 2)
            {
                if (enemys != null && !enemys.IsDead && enemys.IsValidTarget(player.AttackRange + Q.Range) && !enemys.IsZombie
                        && (comboMenu.GetCheckbox("ComboQ") || harassMenu.GetCheckbox("HarassQ")) && comboMenu.GetCheckbox("QSmart") && Q.IsReady())
                {
                    if (E.IsReady() && ((comboMenu.GetCheckbox("ComboQ") && comboMenu.GetCheckbox("QSmart")) || (harassMenu.GetCheckbox("HarassQ") && comboMenu.GetCheckbox("QSmart"))))
                    {
                        List<Vector3> stun = new List<Vector3>();


                        for (int i = 5; i < 360; i += 5)
                        {
                            Vector2 postQ1;
                            Vector2 posQ1;

                            postQ1 = enemys.Position.To2D().RotateAroundPoint(player.Position.To2D(), i);
                            posQ1 = player.Position.To2D().Extend(postQ1, Q.Range);
                            if (LineEquations(enemys, posQ1.To3D(), player, 425) == true   ||  buildingChecks(enemys, posQ1.To3D(), player, 425) == true)
                            {

                                stun.Add(posQ1.To3D());
                            }

                        }
                        if (stun.Count != 0)
                        {
                            double cnt = (stun.Count / 2) - 1;
                            int cvrt = Convert.ToInt32(Math.Ceiling(cnt));
                            return stun[cvrt];
                        }
                        else
                            return Game.CursorPosition;
                    }
                    else
                        return Game.CursorPosition;
                }
                else
                    return empt;
            }
            else
                return empt;
        }

        public static int ChangePriority()
        {
            if (miscMenu.GetKeybind("Changer") /*&& Chat.IsChatOpen*/ && Game.Time > KeyPre)
            {
                if (ComboMode == 1)
                {
                    ComboMode = 2;
                    KeyPre = Game.Time + 0.250;
                }
                else if (ComboMode == 2)
                {
                    ComboMode = 0;
                    KeyPre = Game.Time + 0.250;
                }
                else
                {
                    ComboMode = 1;
                    KeyPre = Game.Time + 0.250;
                }
            }
            return ComboMode;
        }

        //public static void LockW()
        //{
        //    var player = ObjectManager.Me;
        //
        //    var enemies = ObjectManager.Heroes.Enemies;
        //    foreach (var enemy in enemies)
        //    {
        //        if (enemy != null && !enemy.IsDead && enemy.IsValidTarget() && WStacked != null)
        //        {
        //            if (enemy.IsValidTarget(player.AttackRange) && enemy.HasBuff("VayneSilveredDebuff")
        //                && WStacked.Position.Distance(enemy.Position) < 50)
        //            {
        //                TargetSelector.SetTarget(enemy);
        //            }
        //        }
        //        else
        //            Mode = TargetingMode.LessAttack;
        //    }
        //}

        public static void ELogic()
        {
            var player = ObjectManager.Me;
            var enemies = ObjectManager.Heroes.Enemies;

            foreach (var enemy in enemies)
            {
                if(enemy != null && enemy.IsValidTarget(E.Range) && !enemy.IsDead)
                {
                    if (buildingChecks(enemy, player.Position, player, 425) == true || LineEquations(enemy, player.Position, player, 425))
                    {
                        E.CastOnUnit(enemy);
                        break;
                    }
                }
            }
        }

        private static void Orbwalker_AfterAttack(AttackableUnit Source, AttackableUnit target)
        {

            if (Core.Orbwalker.ActiveMode == OrbwalkingMode.Combo && comboMenu.GetCheckbox("ComboQ") || Core.Orbwalker.ActiveMode == OrbwalkingMode.Harass && harassMenu.GetCheckbox("HarassQ"))
            {
                if (Source == ObjectManager.Player)
                {
                    if (true)
                    {
                        Vector3 empt;
                        empt.X = 0;
                        empt.Y = 0;
                        empt.Z = 0;
                        Vector3 SmartResult = SmartQ();
                        if (SmartResult != empt)
                        {
                            Q.Cast(SmartResult);
                        }
                    }
                }
            }
        }

        //private static void LaneClear()
        //{
        //    var player = ObjectManager.Me;
        //    var minion = ObjectManager.MinionsAndMonsters.Enemy.Where(minions => player.Position.Distance(minions.Position) <= player.AttackRange
        //   && !player.CanAttack && minions.Health < player.GetAutoAttackDamage(minions, true)).MinOrDefault(minions => minions.Health);
        //
        //    Chat.Print("1");
        //    if(minion != null /*&& minion.IsValidTarget(player.AttackRange) && !minion.IsDead && Q.IsReady()*/)
        //    {
        //        Chat.Print("2");
        //        Q.CastOnUnit(minion);
        //    }
        //}

        private static void KS()
        {
            var player = ObjectManager.Me;
            var enemy = ObjectManager.Heroes.Enemies.Where(enemies => enemies != null && enemies.IsValidTarget(player.AttackRange + Q.Range)
            && !enemies.IsDead && enemies.Health <= player.GetAutoAttackDamage(enemies, true)).MaxOrDefault(enemies => enemies.TotalAttackDamage);

            if(enemy != null && enemy.IsValidTarget(player.AttackRange + Q.Range) && Q.IsReady() && killstealMenu.GetCheckbox("KSQ"))
            {
                Q.CastOnUnit(enemy);
            }
        }
                                                             
        private void Game_OnUpdate()
        {
            KS(); 

            ChangePriority();


            if (Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.Combo && comboMenu.GetCheckbox("ComboE"))
            {
                ELogic();
            }

            if (Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.Harass && harassMenu.GetCheckbox("HarassE"))
            {
                ELogic();
            }

            //if (Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.LaneClear && laneclearMenu.GetCheckbox("FarmQ"))
            //{
            //    LaneClear();
            //}
            //
            //if (Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.LastHit && lasthitMenu.GetCheckbox("LastHitQ"))
            //{
            //    LaneClear();
            //}


            if (ObjectManager.Me.HasBuff("vaynetumblefade") && check == false)
            {
                Core.Orbwalker.SetAttack(false);
                timer = Game.GameTimeTickCount + 1;
                check = true;
            }
            if (Game.GameTimeTickCount - timer > comboMenu.GetSlider("RKeep") && check == true)
            {
                Core.Orbwalker.SetAttack(true);
                check = false;
            }

            if(comboMenu.GetCheckbox("ComboR"))
            {
                if(EnemiesInRange(ObjectManager.Me.AttackRange, ObjectManager.Me.Position) >= comboMenu.GetSlider("ComboRxt"))
                {
                    R.CastOnUnit(ObjectManager.Me);
                }
            }
        }
        
        private static void Interrupt(AIHeroClient Source, Interrupter.InterruptableTargetEventArgs args)
        {
            if (!Source.IsEnemy || args.DangerLevel < Interrupter.DangerLevel.Medium)
                return;

            if (Source != null && Source.IsValidTarget(E.Range)
                && miscMenu.GetCheckbox("IE") && E.IsReady())
            {
                E.CastOnUnit(Source);
            }
        }
   }
}