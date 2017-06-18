using System;
using System.Linq;
using HesaEngine.SDK;
using SharpDX;
using static DarkOrianna.MenuManager;
using static DarkOrianna.SpellManager;
using static DarkOrianna.DarkPrediction;
namespace DarkOrianna
{
    public static class DrawingManager
    {
        public static void LoadDrawings()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }
        
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!drawingMenu.GetCheckbox("enable")) return;

            //var enemy = TargetSelector.GetTarget(Q.Range + 1000, TargetSelector.DamageType.Magical);
            //if (enemy != null)
            //{
            //    Drawing.DrawCircle(DarkPrediction.LinearPrediction(Orianna.BallPosition[Orianna.BallPosition.Count - 1], Q, enemy), 50, Color.Pink);
            //}

            //empt.X = 0;
            //empt.Y = 0;
            //empt.Z = 0;
            //
            //if (DarkPrediction.BestCastPosition(Q, 2, 380) != empt)
            //{
            //    Drawing.DrawCircle(DarkPrediction.BestCastPosition(Q, 2, 380), 50, Color.Pink);
            //}


            if (Orianna.BallPosition.Count > 0)
            {
                Drawing.DrawCircle(Orianna.BallPosition[Orianna.BallPosition.Count - 1], Q.Width, Color.Green);
            }


            if (drawingMenu.GetCheckbox("drawQ"))
            {
                Drawing.DrawCircle(ObjectManager.Me.Position, Q.Range, Color.Green);
            }
            if (drawingMenu.GetCheckbox("drawE"))
            {
                Drawing.DrawCircle(ObjectManager.Me.Position, E.Range, Color.Red);
            }
        }
    }
}
