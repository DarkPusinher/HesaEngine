using HesaEngine.SDK;

namespace DarkVayne
{
    public static class MenuManager
    {
        public static Menu Home, comboMenu, harassMenu, autoharassMenu, laneclearMenu, lasthitMenu, fleeMenu, drawingMenu, miscMenu, killstealMenu;
        
        private static string prefix = " - ";

        public static void LoadMenu()
        {
            Home = Menu.AddMenu("Dark" + ObjectManager.Me.Hero);

            Main.orb = new Orbwalker.OrbwalkerInstance(Home.AddSubMenu("Orbwalker"));

            comboMenu = Home.AddSubMenu(prefix + "Combo");
            var ComboQ = comboMenu.Add(new MenuCheckbox("ComboQ", "SmartQ", true));
            var SmartQ = comboMenu.Add(new MenuCheckbox("QSmart", "Use Q only AA reset", true));
            var ComboE = comboMenu.Add(new MenuCheckbox("ComboE", "Use E", true));
            var ComboR = comboMenu.Add(new MenuCheckbox("ComboR", "R if x enemies", false));
            var ComboRx = comboMenu.Add(new MenuSlider("ComboRxt", "R if x enemies", 1, 5, 5));
            var ComboRMin = comboMenu.Add(new MenuSlider("RKeep","Keep invis.", 0, 1000, 500));

            harassMenu = Home.AddSubMenu(prefix + "Harass");
            harassMenu.Add(new MenuCheckbox("HarassQ", "Use Q to harass", true));
            harassMenu.Add(new MenuCheckbox("HarassE", "Use E to harass", true));


            //laneclearMenu = Home.AddSubMenu(prefix + "Lane Clear");
            //laneclearMenu.Add(new MenuCheckbox("FarmQ", "Use Q farm", true));
            

            //lasthitMenu = Home.AddSubMenu(prefix + "LastHit");
            //lasthitMenu.Add(new MenuCheckbox("LastHitQ", "Use Q lasthit", true));


            //fleeMenu = Home.AddSubMenu(prefix + "Flee");
            //fleeMenu.Add(new MenuCheckbox("FleeE", "Use E for flee", true));



            drawingMenu = Home.AddSubMenu(prefix + "Drawings");
            drawingMenu.Add(new MenuCheckbox("enable", "Enable", true));
            drawingMenu.Add(new MenuCheckbox("drawQ", "Draw Q", true));
            drawingMenu.Add(new MenuCheckbox("drawE", "Draw E", true));
            drawingMenu.Add(new MenuCheckbox("drawQMode", "Q mode on player", true));
            

            killstealMenu = Home.AddSubMenu(prefix + "KillSteal");
            killstealMenu.Add(new MenuCheckbox("KSQ", "Use Q KS", true));


            miscMenu = Home.AddSubMenu(prefix + "Misc");
            //miscMenu.Add(new MenuCheckbox("agE", "AntiGapclose E", true));
            miscMenu.Add(new MenuCheckbox("IE", "Interrupt E", true));
            miscMenu.Add(new MenuKeybind("Changer", "Q Mode Changer"));


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
