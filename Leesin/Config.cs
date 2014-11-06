using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Leesin
{
    class Config
    {
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;
        public Config()
        {
            Menu = new Menu("Lee Sin#", "LeeSinSharp", true);
            //Target Selector
            var targetSelector = new Menu("Target Selector", "TargetSelector");
            SimpleTs.AddToMenu(targetSelector);
            Menu.AddSubMenu(targetSelector);
            //Orbwalker
            Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));
            //
            //Combo Menu
            //
            Menu.AddSubMenu(new Menu("Combo", "Combo"));
            //Normal Combo
            Menu.SubMenu("Combo").AddSubMenu(new Menu("Combo Settings", "ComboSettings"));
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useQ1C", "Use Q1").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useW1C", "Use W1").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useE1C", "Use E1").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useQ2C", "Use Q2").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useW2C", "Use W2").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useE2C", "Use E2").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("E1CMode", "E1 mode").SetValue(new StringList(new []{"Distance and passive check", "Distance only"}, 1)));
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("E2CMode", "E2 mode").SetValue(new StringList(new []{"Distance and passive check", "Distance only"}, 1)));
            //Insec Combo
            Menu.SubMenu("Combo").AddSubMenu(new Menu("Insec Settings", "InsecSettings"));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insectToTower", "Insect to tower").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insectToAlly", "Insect to ally").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insectToMouse", "Insect to mouse position").SetValue(false));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("moveToMouse", "Move to mouse position").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("useFlashInsec", "Use Flash").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insecOrder", "Insec order").SetValue(new StringList(new[] { "R -> Flash/W", "Flash/W -> R" })));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insecMode", "Insec Mode").SetValue(new StringList(new[] { "Prioritize Ward > W", "Prioritize flash" })));
            //General Combo Seetings
            Menu.SubMenu("Combo").AddItem(new MenuItem("infoText1", "Combo Key: " + Menu.Item("Orbwalk").GetValue<KeyBind>() + "(Orbwalker)"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("insec1", "Q -> Insec").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Menu.SubMenu("Combo").AddItem(new MenuItem("insec2", "Insec -> Q").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            //
            //Harass Menu
            //
            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("infoText2", "Harass Key: " + Menu.Item("Farm").GetValue<KeyBind>() + "(Mixed Mode)"));
            //
            //KillSteal Menu
            //
            Menu.AddSubMenu(new Menu("Kill steal", "KillSteal"));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("enabledKS", "Enabled")).SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Toggle));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useQKS", "Use Q").SetValue(true));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useRKS", "Use R").SetValue(true));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useRCollisionKS", "Use R to kill knockable").SetValue(true));

            Menu.AddToMainMenu();
        }
    }
}
