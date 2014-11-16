using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Leesin
{
    class Config
    {
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;
        public static bool StealthChampiopns;
        public static readonly List<Spell> SpellList = new List<Spell>();
        public Config()
        {
            StealthChampiopns = ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsEnemy && (hero.ChampionName == "Akali" || hero.ChampionName == "MonkeyKing"));

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
            Menu.SubMenu("Combo").AddSubMenu(new Menu("Combo Settings", "ComboSettings")); //Done
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useQC", "Use Q").SetValue(true)); //Done
            if (LeeSinSharp.SmiteSlot != SpellSlot.Unknown)
            {
                Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("smiteCombo", "Smite if a minion(killable) is blocking insec").SetValue(true));  //Done
            }
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useW1C", "Use W1").SetValue(true)); //Done
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useW2C", "Use W2").SetValue(true)); //Done
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useE1C", "Use E1").SetValue(true)); //Done
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("useE2C", "Use E2").SetValue(true)); //Done
            Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("aaBetween", "AAs Between Skills").SetValue(new Slider(1, 0, 2))); //Done
            //Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("E1CMode", "E1 mode").SetValue(new StringList(new []{"Distance and passive check", "Distance only"}, 1)));
            //Menu.SubMenu("Combo").SubMenu("ComboSettings").AddItem(new MenuItem("E2CMode", "E2 mode").SetValue(new StringList(new []{"Distance and passive check", "Distance only"}, 1)));
            //Insec Combo
            Menu.SubMenu("Combo").AddSubMenu(new Menu("Insec Settings", "InsecSettings"));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insectToTower", "Insect to tower").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insectToAlly", "Insect to ally").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insectToMouse", "Insect to mouse position").SetValue(false));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("moveToMouse", "Move to mouse position").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("useFlashInsec", "Use Flash").SetValue(true));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insecDistance", "Insec Position Distance").SetValue(new Slider(250,100,375)));
            if (LeeSinSharp.SmiteSlot != SpellSlot.Unknown)
            {
                Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("smiteInsec", "Smite if a minion(killable) is blocking insec").SetValue(true)); //Done 
            }
            //Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insecOrder", "Insec order").SetValue(new StringList(new[] { "R -> Flash/W", "Flash/W -> R" })));
            Menu.SubMenu("Combo").SubMenu("InsecSettings").AddItem(new MenuItem("insecMode", "Insec Mode").SetValue(new StringList(new[] { "Prioritize Ward > W", "Prioritize flash" })));
            //General Combo Seetings
            Menu.SubMenu("Combo").AddItem(new MenuItem("infoText1", "Combo Key: \"" + (char) (Menu.Item("Orbwalk").GetValue<KeyBind>().Key) + "\"(Orbwalker)"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("insec1", "Q -> Insec").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Menu.SubMenu("Combo").AddItem(new MenuItem("insec2", "Insec -> Q").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Press)));
            //
            //Harass Menu
            //
            Menu.AddSubMenu(new Menu("Harass", "Harass")); //Done
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseQ1H", "Use Q1").SetValue(true)); //Done
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseQ2H", "Use Q2").SetValue(false)); //Done
            if (LeeSinSharp.SmiteSlot != SpellSlot.Unknown)
            {
                Menu.SubMenu("Harass").AddItem(new MenuItem("smiteHarass", "Smite if a minion(killable) is blocking insec").SetValue(false)); //Done
            }
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseWH", "Use W(QQ>JumpBack)").SetValue(false)); //Done
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseWardWH", "Use Ward to jumpback").SetValue(false)); //Done
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseE1H", "Use E1").SetValue(true)); //Done
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseE2H", "Use E2").SetValue(true)); //Done

            Menu.SubMenu("Harass").AddItem(new MenuItem("infoText2", "Harass Key: \"" + (char)(Menu.Item("Farm").GetValue<KeyBind>().Key) + "\"(Mixed Mode)")); //Done
            //
            //Laneclear Menu
            //
            Menu.AddSubMenu(new Menu("Lane Clear", "LaneClear"));
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("infoText3", "Lane/Jungle clear Key: \"" + (char)(Menu.Item("LaneClear").GetValue<KeyBind>().Key) + "\"(LaneClear Mode)")); //Done
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("UseQW", "Use Q").SetValue(true));  //Done 
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("UseWW", "Use W").SetValue(false));  
            Menu.SubMenu("LaneClear").AddItem(new MenuItem("UseEW", "Use E").SetValue(true));  //Done 
            //
            //KillSteal Menu
            //
            Menu.AddSubMenu(new Menu("Kill steal", "KillSteal"));
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("enabledKS", "Enabled")).SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle));//Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useQ1KS", "Use Q1").SetValue(true)); //Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "Use Q2").SetValue(false)); //Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useE1KS", "Use E1").SetValue(true)); //Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useRKS", "Use R").SetValue(true)); //Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("rOverKill", "% R overkill").SetValue(new Slider(50))); //Done
            Menu.SubMenu("KillSteal").AddItem(new MenuItem("useRCollisionKS", "Use R to kill knockable(Bugged)").SetValue(true)); //Partial Done
            //
            //Jungle Menu
            //
            Menu.AddSubMenu(new Menu("Jungle Settings", "Jungle"));
            Menu.SubMenu("Jungle")
                .AddItem(new MenuItem("smiteEnabled", "Enable smite").SetValue(new KeyBind("M".ToCharArray()[0], KeyBindType.Press))); //Done
            Menu.SubMenu("Jungle").AddSubMenu(new Menu("Buff Camps","buffCamp"));
            foreach (var jungleCamp in LeeMethods.JungleCamps)
            {
                Menu.SubMenu("Jungle").SubMenu("buffCamp").AddItem(new MenuItem(jungleCamp, (LeeSinSharp.SmiteSlot != SpellSlot.Unknown ? "Smite " : "Steal ") + jungleCamp).SetValue(true)); //Done
            }
            Menu.SubMenu("Jungle").AddSubMenu(new Menu("Small Camps", "smallCamp"));
            foreach (var smallMinionCamp in LeeMethods.SmallMinionCamps)
            {
                Menu.SubMenu("Jungle").SubMenu("smallCamp").AddItem(new MenuItem(smallMinionCamp, (LeeSinSharp.SmiteSlot != SpellSlot.Unknown ? "Smite " : "Steal ") + smallMinionCamp).SetValue(false)); //Done
            }
            Menu.SubMenu("Jungle")
                .AddItem(new MenuItem("stealCamp", "Steal (toggeled) Camp").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Press)));
            //
            //Drawing Menu
            //
            Menu.AddSubMenu(new Menu("Draw Settings", "Draw"));
            foreach (var spell in SpellList)
            {
                Menu.SubMenu("Draw")
                    .AddItem(
                        new MenuItem(spell.Slot + "Draw", "Draw " + spell.Slot + "range").SetValue(
                            new Circle(true, Color.FromArgb(128, 128, 0, 128))));
            }
            //
            //Misc Menu
            //
            Menu.AddSubMenu(new Menu("Misc Settings", "Misc"));
            if (StealthChampiopns)
            {
                Menu.SubMenu("Misc").AddItem(new MenuItem("autoEEStealth", "Auto reaveal stealth champions(EE).").SetValue(true));     //Done           
            }
            Console.WriteLine(StealthChampiopns);
            Menu.SubMenu("Misc").AddItem(new MenuItem("wardJump", "Ward Jump").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press))); //Done
            Menu.SubMenu("Misc").AddItem(new MenuItem("packetCast", "Packet Cast").SetValue(false));                //Done
            //Add to main menu
            Menu.AddToMainMenu();

        }
    }
}
