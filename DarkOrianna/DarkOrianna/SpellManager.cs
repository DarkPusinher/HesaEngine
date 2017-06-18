using System;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using HesaEngine.SDK.GameObjects;
using static DarkOrianna.MenuManager;

namespace DarkOrianna
{
    public static class SpellManager
    {
        public static Spell Q, W, E, R;

        public static void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 815);         //Active: Annie's E    
            W = new Spell(SpellSlot.W, 1305);   //SkillShot: Ezreal's Q
            E = new Spell(SpellSlot.E, 1095);   //Charged: Xerath's Q
            R = new Spell(SpellSlot.R, 1305);   //Targeted: Veigar's R
            

            Q.SetSkillshot(delay: 0.00001f, width: 145, speed: 900, collision: true, type: SkillshotType.SkillshotLine);

            //Do the same with QPred, EPred, RPred, always depending on what they are going to collide.
            //Examples: 
            //Jhin's W. Collides with Heroes, and YasuoWall. But not with Minions.
            //Ezreal's W collides with YasuoWall
            //Ezreal's Q collides with Heroes, Minions, YasuoWall
            Orianna.QPred = new PredictionInput
            {
                Delay = W.Delay,
                Radius = W.Width,
                Speed = W.Speed,
                Type = W.Type,
                CollisionObjects = new[]
                {
                    //CollisionableObjects.Heroes,
                    //CollisionableObjects.Minions,
                    CollisionableObjects.YasuoWall
                }
            };
        }
    }
}