using HesaEngine.SDK;
using SharpDX.DirectInput;

namespace DarkOrianna
{
    public static class MenuManager
    {
        public static Menu Home, comboMenu, harassMenu, autoharassMenu, laneclearMenu, lasthitMenu, fleeMenu, drawingMenu, miscMenu, killstealMenu;
        
        private static string prefix = " - ";

        public static void LoadMenu()
        {
            Home = Menu.AddMenu("Dark" + ObjectManager.Me.Hero);

            Orianna.orb = new Orbwalker.OrbwalkerInstance(Home.AddSubMenu("Orbwalker"));

            comboMenu = Home.AddSubMenu(prefix + "Combo");
            comboMenu.Add(new MenuCheckbox("useQ", "Use Q", true));
            comboMenu.Add(new MenuCheckbox("useW", "Use W", true));
            comboMenu.Add(new MenuCheckbox("useE", "Use E", true));
            comboMenu.Add(new MenuSlider("UseRX", "Combo R if >= ", 1, 5, 2));
            comboMenu.Add(new MenuSlider("UseQX", "Q if can ult >= ", 1, 5, 2));

            harassMenu = Home.AddSubMenu(prefix + "Harass");
            harassMenu.Add(new MenuCheckbox("useQ", "Use Q", true));
            harassMenu.Add(new MenuCheckbox("useW", "Use W", true));
            harassMenu.Add(new MenuCheckbox("useE", "Use E", true));


            //laneclearMenu = Home.AddSubMenu(prefix + "Lane Clear");
            //laneclearMenu.Add(new MenuCheckbox("useQ", "Use Q", true));
            //laneclearMenu.Add(new MenuCheckbox("useW", "Use W", true));
            //laneclearMenu.Add(new MenuCheckbox("useE", "Use E", true));
            //laneclearMenu.Add(new MenuCheckbox("useR", "Use R", true));
            //laneclearMenu.Add(new MenuSlider("mana", "Mana % must be >= ", 10, 100, 50));


            //lasthitMenu = Home.AddSubMenu(prefix + "LastHit");
            //lasthitMenu.Add(new MenuCheckbox("useQ", "Use Q", true));
            //lasthitMenu.Add(new MenuCheckbox("useW", "Use W", true));
            //lasthitMenu.Add(new MenuCheckbox("useE", "Use E", true));
            //lasthitMenu.Add(new MenuCheckbox("useR", "Use R", true));
            //lasthitMenu.Add(new MenuSlider("mana", "Mana % must be >= ", 10, 100, 50));


            //fleeMenu = Home.AddSubMenu(prefix + "Flee");
            //fleeMenu.Add(new MenuCheckbox("useQ", "Use Q", true));
            //fleeMenu.Add(new MenuCheckbox("useW", "Use W", true));
            //fleeMenu.Add(new MenuCheckbox("useE", "Use E", true));
            //fleeMenu.Add(new MenuCheckbox("useR", "Use R", true));
            //fleeMenu.Add(new MenuSlider("mana", "Mana % must be >= ", 10, 100, 5));



            drawingMenu = Home.AddSubMenu(prefix + "Drawings");
            drawingMenu.Add(new MenuCheckbox("enable", "Enable", true));
            drawingMenu.Add(new MenuCheckbox("drawQ", "Draw Q", true));
            //drawingMenu.Add(new MenuCheckbox("drawW", "Draw W", true));
            drawingMenu.Add(new MenuCheckbox("drawE", "Draw E", true));
            //drawingMenu.Add(new MenuCheckbox("drawR", "Draw R", true));

            killstealMenu = Home.AddSubMenu(prefix + "KillSteal");
            killstealMenu.Add(new MenuCheckbox("useQ", "Use Q to KS", true));
            killstealMenu.Add(new MenuCheckbox("useW", "Use W to KS", true));
            killstealMenu.Add(new MenuCheckbox("useR", "Use R to KS", true));
            //killstealMenu.Add(new MenuCheckbox("useR", "Use R", true));
            //killstealMenu.Add(new MenuSlider("mana", "Mana % must be >= ", 10, 100, 10));


            miscMenu = Home.AddSubMenu(prefix + "Misc");
            miscMenu.Add(new MenuCheckbox("RC", "CancelR if no hit", true));
            miscMenu.Add(new MenuCheckbox("RX", "Auto R if >= ", true));
            miscMenu.Add(new MenuCheckbox("AW", "Auto W", true));
            miscMenu.Add(new MenuSlider("EA", "E after Q if hp <=", 1, 5000, 250));
            //miscMenu.Add(new MenuSlider("mana", "Mana % must be >= ", 10, 100, 30));
            //miscMenu.Add(new MenuCheckbox("level", "Enable Spell Leveler", true));
            //miscMenu.Add(new MenuSlider("levelDelay", "Level UP Delay", 10, 1000, 200));
            //miscMenu.Add(new MenuCombo("levelFirst", "Level UP First", new[] { "Q", "W", "E" }));
            //miscMenu.Add(new MenuCombo("levelSecond", "Level UP Second", new[] { "Q", "W", "E" }, 1));
            //miscMenu.Add(new MenuCombo("levelThird", "Level UP Third", new[] { "Q", "W", "E" }, 2));


        }

        public static bool GetCheckbox(this Menu menu, string value)
        {
            return menu.Get<MenuCheckbox>(value).Checked;
        }

        public static bool GetKeybind(this Menu menu, string value)
        {
            return menu.Get<MenuKeybind>(value).Active;
        }

        public static int GetSlider(this Menu menu, string value)
        {
            return menu.Get<MenuSlider>(value).CurrentValue;
        }

        public static int GetCombobox(this Menu menu, string value)
        {
            return menu.Get<MenuCombo>(value).CurrentValue;
        }
    }
}
