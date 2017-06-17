using HesaEngine.SDK;
using HesaEngine.SDK.GameObjects;

namespace DarkVayne
{
    public static class Pred
    {
        //I made this just for testing purposes.
        //You can just use:
        //W.CastIfHitchanceEquals(target, HitChance.Medium);
        public static void PredictionCast(this Spell spell, Obj_AI_Base target, HitChance hit = HitChance.Medium)
        {
            var pred = spell.GetPrediction(target);
            if (pred.Hitchance >= hit)
            {
                spell.Cast(pred.CastPosition);
            }
        }
    }
}
