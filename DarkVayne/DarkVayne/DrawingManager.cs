using System;
using System.Linq;
using HesaEngine.SDK;
using SharpDX;
using static DarkVayne.MenuManager;
using static DarkVayne.SpellManager;
using HesaEngine.SDK.GameObjects;
using HesaEngine.SDK.Notifications;
using static HesaEngine.SDK.TargetSelector;

namespace DarkVayne
{
    public static class Drawing
    {
        public static void LoadDrawings()
        {
            HesaEngine.SDK.Drawing.OnDraw += Drawing_OnDraw;
        }
        
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!drawingMenu.GetCheckbox("enable")) return;

            if (drawingMenu.GetCheckbox("drawQMode"))
            {
                Vector2 drawPosition = HesaEngine.SDK.Drawing.WorldToScreen(ObjectManager.Me.Position);
                drawPosition.Y += 25;
                HesaEngine.SDK.Drawing.DrawText(drawPosition, Color.Red, Main.Names[Main.ComboMode]);
            }

            if (drawingMenu.GetCheckbox("drawE"))
            {
                //AIHeroClient enemySafe = TargetSelector.GetTarget(ObjectManager.Me.AttackRange, DamageType.Physical);
                //var closestTurret = ObjectManager.Turrets.Ally.Where(turret => !turret.IsDead).MinOrDefault(turret => ObjectManager.Player.Distance(turret));
                //var inhis = ObjectManager.Get<Obj_Building>().MinOrDefault(inhi => ObjectManager.Player.Distance(inhi));



                HesaEngine.SDK.Drawing.DrawCircle(ObjectManager.Me.Position, E.Range, Color.Red);
                //Drawing.DrawCircle(ObjectManager.Me.Position, Q.Range, Color.Red);
                //Drawing.DrawCircle(ObjectManager.Me.Position, closestTurret.BoundingRadius, Color.Red);
                //Drawing.DrawCircle(inhis.Position, inhis.BoundingRadius, Color.Yellow);
            }
            if (drawingMenu.GetCheckbox("drawQ"))
            {
                HesaEngine.SDK.Drawing.DrawCircle(ObjectManager.Me.Position, Q.Range, Color.Red);
            }
        }
    }
}
