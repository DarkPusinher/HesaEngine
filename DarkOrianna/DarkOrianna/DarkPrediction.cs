using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HesaEngine.SDK;
using SharpDX;
using HesaEngine.SDK.GameObjects;
using static DarkOrianna.MenuManager;
using static DarkOrianna.SpellManager;

namespace DarkOrianna
{
    public static class DarkPrediction
    {
        public static Vector3 deneme;
        public static Vector3 empt;
        public static double delayers = 0;
        public static double delayer3 = 0;
        public static Vector3 epos123;
        public static Vector3 ppos123;
        public static Vector3 direction5;
        public static bool stopper = false;
        public static bool stoper = false;
        public static bool stoperC = false;
        public static bool check = false;
        public static bool lol = false;

        public static List<Vector3> epos = new List<Vector3>();
        public static List<Vector3> eposC = new List<Vector3>();
        public static List<double> travelDistances = new List<double>();
        public static double delayer = 0;
        public static double delayer1 = 0;
        public static double delayer2 = 0;
        public static double delayer5 = 0;
        public static double delayerC = 0;
        public static double h = 0;
        public static double k = 0;

        public static bool CollisionChecker(Vector3 enemy, Vector3 player, Spell Q)
        {
            bool collision = false;
            Vector3 porte = enemy;
            Vector2 from = (player).To2D();
            Vector2 to = (porte).To2D();
            float m = ((to.Y - from.Y) / (to.X - from.X));
            float x;
            float y;
            float m2 = (-(to.X - from.X) / (to.Y - from.Y));
            var minions = ObjectManager.MinionsAndMonsters.Enemy.Where(minionz => minionz != null && player.Distance(minionz.Position) <= Q.Range);

            foreach (var minion in minions)
            {
                Vector3 minionP = minion.Position;
                Vector2 minionPos = (minionP).To2D();
                float px = minionPos.X;
                float py = minionPos.Y;
                x = ((m2 * px) - (from.X * m) + (from.Y - py)) / (m2 - m);
                y = m * (x - from.X) + from.Y;
                Vector2 colliPos;
                colliPos.X = x;
                colliPos.Y = y;
                if (colliPos.Distance(minionPos) <= Q.Width + minion.BoundingRadius - 10 && (porte).To2D().Distance(colliPos) < (porte).To2D().Distance(from) && from.Distance(colliPos) < (enemy).To2D().Distance(from))
                {
                    collision = true;
                    break;
                }
            }
            return collision;
        } // Vector3 to Vector3 collision checker. Only checks minions

        public static Vector3 LinearPrediction(Vector3 skillInitialPosition, Spell spell, AIHeroClient enemy)
        {
            var player = ObjectManager.Me;

            empt.X = 0;
            empt.Y = 0;
            empt.Z = 0;

            double SS = spell.Speed;
            double temporar = 10000;
            float t = 0;
            List<double> Ds = new List<double>();

            if (enemy != null && !enemy.IsDead && enemy.IsValidTarget(spell.Range))
            {
                if (!enemy.IsMoving)
                {
                    stoper = false;
                    return enemy.Position;
                }


                if (enemy.IsDashing())
                {
                    
                    stoper = false;
                    return empt;
                }

                float ES = enemy.MovementSpeed;

                if (Game.GameTimeTickCount >= delayer)
                {
                    
                    epos.Add(enemy.Position);
                    delayer = Game.GameTimeTickCount + 10;
                }

                //if (epos.size() > 3 && stoper == false)
                //{
                //    if (ToVec2((epos[epos.size() - 2] - epos[epos.size() - 3]).VectorNormalize()) != ToVec2((epos[epos.size() - 3] - epos[epos.size() - 4]).VectorNormalize()))
                //    {
                //        Vec2 loc1 = ToVec2(enemy->GetPosition() + ((epos[epos.size() - 2] - epos[epos.size() - 3]).VectorNormalize())) * 1000;
                //        Vec2 loc2 = ToVec2(enemy->GetPosition() + ((epos[epos.size() - 3] - epos[epos.size() - 4]).VectorNormalize())) * 1000;
                //        if (Distance(loc1, loc2) > 10)
                //        {
                //            stoper = true;
                //            delayer2 = GGame->TickCount() + 300;
                //        }
                //        else
                //        {
                //            return empt;
                //        }
                //    }
                //    else
                //    {
                //        return empt;
                //    }
                //}
                //
                //if (stoper = true && GGame->TickCount() <= delayer2)
                //{
                //    Vec3 direction = (epos[epos.size() - 1] - epos[epos.size() - 2]).VectorNormalize();
                //    double D = 0;
                //    h = BallLocation.x;
                //    k = BallLocation.y;
                //
                //    for (int i = 0; i < 6000; i++)
                //    {
                //        Vec3 EF = enemy->GetPosition() + direction * (ES / 1000) * i;
                //        D = abs(sqrt(pow((EF.x - h), 2) + pow(EF.y - k, 2)) - (SS / 1000) * i);
                //        Ds.push_back(D);
                //        if (Ds.size() > 0)
                //        {
                //            if (Ds[Ds.size() - 1] <= temporar)
                //            {
                //                temporar = Ds[Ds.size() - 1];
                //            }
                //            else
                //            {
                //                t = i - 1;
                //                break;
                //            }
                //        }
                //    }
                //    t = (t + (Q->GetDelay() * 1000) + (GGame->Latency() / 1));
                //    Vec3 EFuture = enemy->GetPosition() + direction * (ES / 1000) * t;
                //    Vec3 fut = Extend(EFuture, enemy->GetPosition(), (Q->Radius() - 30));
                //    return fut;
                //}
                //else
                //{
                //    return empt;
                //}

                if (epos.Count > 1)
                {

                    Vector3 direction = (epos[epos.Count - 1] - epos[epos.Count - 2]).Normalized();
                    double D = 0;
                    h = skillInitialPosition.X;
                    k = skillInitialPosition.Z;
                    //float xvar = direction.X*(ES / 1000);
                    //float yvar = direction.Y * (ES / 1000);
                    //float zvar = direction.Z * (ES / 1000);
                    //Vector3 dir = new Vector3();
                    //dir.X = xvar;
                    //dir.Y = yvar;
                    //dir.Z = zvar;
                    
                    for (int i = 0; i < 6000; i += 10)
                    {
                        Vector3 EF = enemy.Position + (direction * (ES / 1000) * i);
                        D = Math.Abs(Math.Sqrt(Math.Pow((EF.X - h), 2) + Math.Pow(EF.Z - k, 2)) - (SS / 1000) * i);
                        Ds.Add(D);
                        if (Ds[Ds.Count - 1] <= temporar)
                        {
                            temporar = Ds[Ds.Count - 1];
                        }
                        else
                        {
                            t = i - 1;
                            break;
                        }
                    }
                    t = t + (spell.Delay * 1000) + (Game.Ping / 1);
                    Vector3 EFuture = enemy.Position + (direction * (ES / 1000) * t);
                    Vector3 fut = EFuture.Extend(enemy.Position, (spell.Width - 30));
                    return fut;
                }
                else
                    return empt;
            }
            else
                return empt;

        }  // Vector3 to enemy prediction. Can be used for circler skillshots as well if the skill is travelling in the air. Like Anivia Q, Orianna Q

        public static Vector3 BestCastPosition(Spell spell, int targetHit, int radiusSpell) // Finds Best Cast position of a skill
        {
            
            var enemies = ObjectManager.Heroes.Enemies.Where(enemy => enemy != null & enemy.IsValidTarget(Q.Range));
            int count = 0;
            List<AIHeroClient> bestEnemies = new List<AIHeroClient>();
            float Xs = 0;
            float Zs = 0;
            empt.X = 0;
            empt.Y = 0;
            empt.Z = 0;

            
            foreach (var enemt in enemies)
            {
                count = 0;
                foreach (var ene in enemies)
                {
                        
                    if (enemt.Position.Distance(ene.Position) <= 2 * radiusSpell)
                    {
                        count += 1;
                    }
                    if (count >= targetHit)
                    {
                        bestEnemies.Add(enemt);
                    }
                }
            }

            if (bestEnemies.Count >= targetHit && bestEnemies.Count > 1)
            {
                
                for (int k = 0; k < bestEnemies.Count; k++)
                {
                    Xs += bestEnemies[k].Position.X;
                    Zs += bestEnemies[k].Position.Z;
                }
                float avgX = Xs / bestEnemies.Count;
                float avgZ = Zs / bestEnemies.Count;
                Vector2 BestPosi;
                BestPosi.X = avgX;
                BestPosi.Y = avgZ;
                if (BestPosi.Distance(ObjectManager.Me.Position.To2D()) <= spell.Range)
                {
                    return BestPosi.To3D();
                }
                else
                    return empt;
            }
            else
                return empt;
        }  // 

        public static Vector3 CirclerPrediction(Spell spell, AIHeroClient enemy)
        {
            var player = ObjectManager.Me;

            empt.X = 0;
            empt.Y = 0;
            empt.Z = 0;

            if (enemy != null && !enemy.IsDead && enemy.IsValidTarget(spell.Range))
            {
                if (!enemy.IsMoving)
                {
                    stoperC = false;
                    return enemy.Position;
                }


                if (enemy.IsDashing())
                {

                    stoperC = false;
                    return empt;
                }

                float ES = enemy.MovementSpeed;

                if (Game.GameTimeTickCount >= delayerC)
                {

                    eposC.Add(enemy.Position);
                    delayerC = Game.GameTimeTickCount + 10;
                }
                if (eposC.Count > 1)
                {

                    Vector3 direction = (eposC[eposC.Count - 1] - eposC[eposC.Count - 2]).Normalized();
                    Vector3 EFuture = enemy.Position + (direction * (ES / 1000) * (spell.Delay + Game.Ping));
                    Vector3 fut = EFuture.Extend(enemy.Position, (spell.Width - 30));
                    return fut;
                }
                else
                    return empt;
            }
            else
                return empt;
        } // this is good for when skill doesn't travel in the air like Brand W, Cassiopeia Q(Constant delay)
    }
}
