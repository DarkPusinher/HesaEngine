using System;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using HesaEngine.SDK.GameObjects;
using static DarkVayne.MenuManager;

namespace DarkVayne
{
    public static class SpellManager
    {
        public static Spell Q, W, E, R;

        public static void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 300);         //Active: Annie's E    
            E = new Spell(SpellSlot.E, 750);   //Charged: Xerath's Q
            R = new Spell(SpellSlot.R);   //Targeted: Veigar's R
            
            //Do the same with QPred, EPred, RPred, always depending on what they are going to collide.
            //Examples: 
            //Jhin's W. Collides with Heroes, and YasuoWall. But not with Minions.
            //Ezreal's W collides with YasuoWall
            //Ezreal's Q collides with Heroes, Minions, YasuoWall
           // Main.WPred = new PredictionInput
           // {
           //     Delay = W.Delay,
           //     Radius = W.Width,
           //     Speed = W.Speed,
           //     Type = W.Type,
           //     CollisionObjects = new[]
           //     {
           //         CollisionableObjects.Heroes,
           //         CollisionableObjects.Minions,
           //         CollisionableObjects.YasuoWall
           //     }
           // };
        }
    }
}