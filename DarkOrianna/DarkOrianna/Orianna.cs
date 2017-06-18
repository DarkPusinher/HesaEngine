using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using HesaEngine.SDK.GameObjects;
using static DarkOrianna.SpellManager;
using static DarkOrianna.MenuManager;
using static DarkOrianna.DrawingManager;
using static DarkOrianna.DarkPrediction;
using SharpDX;
using System.Collections.Generic;
using HesaEngine.SDK.Args;
using System;
using System.Linq;

namespace DarkOrianna
{
    public class Orianna : IScript
    {
        //Don't forget to change the champion first!
        private readonly Champion champion = Champion.Orianna;

        public string Name => "Dark" + champion;

        public string Version => "1.0.0";

        public string Author => "DarkPunisher";

        public static PredictionInput QPred, WPred, EPred, RPred;

        public static Orbwalker.OrbwalkerInstance orb;

        public static List<Vector3> BallPosition = new List<Vector3>();

        public static Obj_AI_Base all;

        public static bool checkball = false;

        public static double ti = 0;

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

            empt.X = 0;
            empt.Y = 0;
            empt.Z = 0;

            LoadDrawings();

            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += OnCreate;
            Obj_AI_Base.OnBuffGained += OnBuffGain;
            SpellBook.OnCastSpell += OnCast_Spell;
           

            Chat.Print("<font color='#ffffff'>" + Name + "</font> <font color='#f8a101'>by" + Author + "</font><font color='#f8a101'> - Loaded</font>");
        }

        public static int EnemiesInRange(float range, Vector3 Source)
        {
            var Targets = ObjectManager.Heroes.Enemies.Where(enemyl => enemyl != null && enemyl.IsValidTarget(R.Range));
            var player = ObjectManager.Player;
            var enemiesInRange = 0;

            foreach(var enemy in Targets)
            {
                if(enemy.Position.Distance(Source) < range)
                {
                    enemiesInRange++;
                }
            }
            return enemiesInRange;
        }

        private static void GetBall()
        {
            if(checkball == true)
            {
                BallPosition.Add(all.Position);
            }
            if (BallPosition.Count > 0)
            {
                if (BallPosition[BallPosition.Count - 1].Distance(ObjectManager.Me.Position) > 1305)
                {
                    all = ObjectManager.Me;
                    checkball = true;
                }
                if (BallPosition[BallPosition.Count - 1].Distance(ObjectManager.Me.Position) < 140)
                {
                    all = ObjectManager.Me;
                    checkball = true;
                }
            }
            if(BallPosition.Count == 0)
            {
                all = ObjectManager.Me;
                checkball = true;
            }
        }

        private static bool ECheck(AIHeroClient enemy, AIHeroClient player)
        {
            bool collision = false;
            Vector3 porte = BallPosition[BallPosition.Count - 1].Extend(player.Position, -400);
            Vector2 from = BallPosition[BallPosition.Count - 1].To2D();
            Vector2 to = (porte).To2D();
            float m = ((to.Y - from.Y) / (to.X - from.X));
            float x;
            float y;
            float m2 = (-(to.X - from.X) / (to.Y - from.Y));

            Vector3 minionP = enemy.Position;
            Vector2 minionPos = (minionP).To2D();
            float px = minionPos.X;
            float py = minionPos.Y;
            x = ((m2 * px) - (from.X * m) + (from.Y - py)) / (m2 - m);
            y = m * (x - from.X) + from.Y;
            Vector2 colliPos;
            colliPos.X = x;
            colliPos.Y = y;
            if (colliPos.Distance(minionPos) <= enemy.BoundingRadius + Q.Width - 100 && colliPos.Distance( from) <= from.Distance((player.Position).To2D())
                && from.Distance((player.Position).To2D()) > player.Position.To2D().Distance(colliPos))
            {
                collision = true;
            }
            return collision;
        }

        private static float CalculateDamage(Obj_AI_Base @object)
        {
            return ((float)ObjectManager.Player.GetAutoAttackDamage(@object, true)) / @object.Health;
        }

        private static void autoEDmg()
        {
            var player = ObjectManager.Me;
            var enemies = ObjectManager.Heroes.Enemies;

            foreach (var enemy in enemies)
            {
                if (enemy != null && !enemy.IsDead && enemy.IsValidTarget())
                {
                    if (ECheck(enemy, player) == true && E.IsReady())
                    {
                        E.CastOnUnit(player);
                    }
                }
            }
        }

        private static void autoW()
        {
            var player = ObjectManager.Me;

            if(EnemiesInRange(140, BallPosition[BallPosition.Count - 1]) >= 1)
            {
                W.CastOnUnit(player);
            }
        }

        private static void checkerE()
        {
            if (E.IsReady())
            {
                var enemy = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                if (enemy != null && !enemy.IsDead && enemy.IsValidTarget(Q.Range))
                {
                    var ally = ObjectManager.Heroes.Allies.MinOrDefault(allies => allies.Position.Distance(enemy.Position));
                    if (ally != null && ally.Position.Distance(enemy.Position) < BallPosition[BallPosition.Count - 1].Distance(enemy.Position))
                    {
                        E.CastOnUnit(ally);
                    }
                }
            }
        }

        private static void Combo()
        {
            var player = ObjectManager.Me;
            TargetSelector.Mode = TargetSelector.TargetingMode.LessCast;
            empt.X = 0;
            empt.Y = 0;
            empt.Z = 0;

            var enemy = TargetSelector.GetTarget(W.Range);
            if (enemy != null)
            {
                if (!enemy.IsDead && enemy.IsValidTarget(Q.Range) && Q.IsReady() && R.IsReady())
                {
                    if(BestCastPosition(Q, comboMenu.GetSlider("UseQX"), 380) != empt)
                    {
                        Q.Cast(BestCastPosition(Q, comboMenu.GetSlider("UseQX"), 380));
                    }
                }
                if (!enemy.IsDead && enemy.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    var location = LinearPrediction(BallPosition[BallPosition.Count - 1], Q, enemy);
                    if (location != empt && player.Distance(location) <= Q.Range)
                    {
                        checkerE();
                        Q.Cast(location);
                    }
                }
            }
        }

        private static void RKill()
        {
            var player = ObjectManager.Me;
            TargetSelector.Mode = TargetSelector.TargetingMode.LessCast;
            empt.X = 0;
            empt.Y = 0;
            empt.Z = 0;

            var enemy = TargetSelector.GetTarget(W.Range);
            if (enemy != null)
            {

                if (!enemy.IsDead && enemy.IsValidTarget(R.Range) && R.IsReady() && killstealMenu.GetCheckbox("useR")
                    && enemy.Health <= R.GetDamage(enemy) && EnemiesInRange(380, BallPosition[BallPosition.Count - 1]) >= 1)
                {
                    R.CastOnUnit(player);
                }
            }
        }

        private static void KS()
        {
            var player = ObjectManager.Me;
            TargetSelector.Mode = TargetSelector.TargetingMode.LessCast;
            empt.X = 0;
            empt.Y = 0;
            empt.Z = 0;

            var enemy = TargetSelector.GetTarget(W.Range);
            if (enemy != null)
            {
                if (!enemy.IsDead && enemy.IsValidTarget(Q.Range) && Q.IsReady() && R.IsReady())
                {
                    if (BestCastPosition(Q, comboMenu.GetSlider("UseQX"), 380) != empt)
                    {
                        Q.Cast(BestCastPosition(Q, comboMenu.GetSlider("UseQX"), 380));
                    }
                }
                if (!enemy.IsDead && enemy.IsValidTarget(Q.Range) && Q.IsReady() && W.IsReady() && R.IsReady() && killstealMenu.GetCheckbox("useR") && killstealMenu.GetCheckbox("useQ") && killstealMenu.GetCheckbox("useW")
                    && enemy.Health <= Q.GetDamage(enemy) + W.GetDamage(enemy) + R.GetDamage(enemy) + player.GetAutoAttackDamage(enemy))
                {
                    var location = LinearPrediction(BallPosition[BallPosition.Count - 1], Q, enemy);
                    if (location != empt && player.Distance(location) <= Q.Range)
                    {
                        checkerE();
                        Q.Cast(location);
                    }
                }
                if (!enemy.IsDead && enemy.IsValidTarget(Q.Range) && Q.IsReady() && W.IsReady() && !R.IsReady() && killstealMenu.GetCheckbox("useW") && killstealMenu.GetCheckbox("useQ")
                    && enemy.Health <= Q.GetDamage(enemy) + W.GetDamage(enemy) + player.GetAutoAttackDamage(enemy))
                {
                    var location = LinearPrediction(BallPosition[BallPosition.Count - 1], Q, enemy);
                    if (location != empt && player.Distance(location) <= Q.Range)
                    {
                        checkerE();
                        Q.Cast(location);
                    }
                }
                if (!enemy.IsDead && enemy.IsValidTarget(Q.Range) && Q.IsReady() && !W.IsReady() && !R.IsReady() && killstealMenu.GetCheckbox("useQ")
                    && enemy.Health <= Q.GetDamage(enemy) + player.GetAutoAttackDamage(enemy))
                {
                    var location = LinearPrediction(BallPosition[BallPosition.Count - 1], Q, enemy);
                    if (location != empt && player.Distance(location) <= Q.Range)
                    {
                        checkerE();
                        Q.Cast(location);
                    }
                }
            }
        }

        private static void autoR()
        {
            var player = ObjectManager.Me;

            if (EnemiesInRange(380, BallPosition[BallPosition.Count - 1]) >= comboMenu.GetSlider("UseRX") && R.IsReady())
            {
                R.CastOnUnit(player);
            }
        }

        private static void autoEShield()
        {
            var player = ObjectManager.Player;
        
            if(player.Health <= miscMenu.GetSlider("EA") && !Q.IsReady() && E.IsReady())
            {
                E.CastOnUnit(player);
            }
        }

        private void OnCast_Spell(SpellBook sender, SpellbookCastSpellEventArgs args)
        {
            if(sender.Owner.IsMe && args.Slot == SpellSlot.R)
            {
                if (EnemiesInRange(380, BallPosition[BallPosition.Count -1]) == 0)
                {
                    args.Process = false;
                }
            }
        }

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainedEventArgs args)
        {
            if(args.Buff.DisplayName == "orianaredactshield")
            {
                all = args.Buff.Target;
                checkball = true;
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if(sender.Name == "TheDoomBall")
            {
                checkball = false;
                BallPosition.Add(sender.Position);
                
            }

        }

        private void Game_OnUpdate()
        {
            var mana = ObjectManager.Me.ManaPercent;
            GetBall();
            KS();
            RKill();

            autoEShield();

            if (miscMenu.GetCheckbox("AW"))
            {
                autoW();
            }

            if(miscMenu.GetCheckbox("RX"))
            {
                autoR();
            }

            if(Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.Combo)
            {
                if(comboMenu.GetCheckbox("useQ"))
                {
                    Combo();
                }
                if (comboMenu.GetCheckbox("useW"))
                {
                    autoW();
                }
                if (comboMenu.GetCheckbox("useE"))
                {
                    autoEDmg();
                }
                autoR();
            }

            if (Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.Harass)
            {
                if (harassMenu.GetCheckbox("useQ"))
                {
                    Combo();
                }
                if (harassMenu.GetCheckbox("useW"))
                {
                    autoW();
                }
                if (harassMenu.GetCheckbox("useE"))
                {
                    autoEDmg();
                }
            }
        }
    }
}
