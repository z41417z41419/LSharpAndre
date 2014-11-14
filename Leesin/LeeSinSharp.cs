using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Leesin
{
    internal class LeeSinSharp
    {
        //public static Obj_AI_Hero Player = ObjectManager.Player;

        public static SpellSlot SmiteSlot;
        public static SpellSlot FlashSlot;


        public LeeSinSharp()
        {
            LeeMethods.Player = ObjectManager.Player;
            SmiteSlot = LeeMethods.Player.GetSpellSlot("SummonerSmite");
            FlashSlot = LeeMethods.Player.GetSpellSlot("summonerflash");
            LeeMethods.Q.SetSkillshot(0.25f, 65f, 1800f, true, SkillshotType.SkillshotLine);
            LeeMethods.R.SetTargetted(0.25f, float.MaxValue);
            Config.SpellList.Add(LeeMethods.Q);
            Config.SpellList.Add(LeeMethods.W);
            Config.SpellList.Add(LeeMethods.E);
            Config.SpellList.Add(LeeMethods.R);

            // ReSharper disable once ObjectCreationAsStatement
            new Config();
            if (Config.StealthChampiopns)
            {
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            }
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            LeeUtility.SendMessage("Loaded");
        }

        private void OnDraw(EventArgs args)
        {
            foreach (var spell in Config.SpellList)
            {
                var menuItem = Config.Menu.Item(spell.Slot + "Draw").GetValue<Circle>();
                if (menuItem.Active)
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);                    
                }
            }
            //Drawing.DrawText(100, 100, Color.White, "Harass Stage: " + LeeMethods.harassStage);
            //Console.WriteLine(ObjectManager.Player.BoundingRadius);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                if (Config.Menu.Item("wardJump").GetValue<KeyBind>().Active)
                {
                    LeeMethods.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    LeeUtility.WardJump(Game.CursorPos);
                }

                if (SmiteSlot != SpellSlot.Unknown && Config.Menu.Item("stealCamp").GetValue<KeyBind>().Active)
                {
                    LeeMethods.CampStealer();
                }
                if (SmiteSlot != SpellSlot.Unknown && Config.Menu.Item("smiteEnabled").GetValue<KeyBind>().Active)
                {
                    LeeMethods.Smite();
                }
                //Console.WriteLine(LeeUtility.MenuParamBool("enabledKS"));
                if (Config.Menu.Item("enabledKS").GetValue<KeyBind>().Active)
                {
                    LeeMethods.KSer();
                }

                Obj_AI_Hero target = SimpleTs.GetTarget(
                    LeeMethods.Q.IsReady() ? LeeMethods.Q.Range : LeeMethods.R.Range, SimpleTs.DamageType.Physical);
                LeeMethods.InsecCombo(target);
                switch (Config.Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        LeeMethods.Combo(target);
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        LeeMethods.Harass(target);
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        LeeMethods.LaneClear();
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsAlly || sender.Type.Equals(GameObjectType.obj_AI_Hero) ||
                (((Obj_AI_Hero) sender).ChampionName != "Wukong" && ((Obj_AI_Hero) sender).ChampionName != "Akali") ||
                Vector3.DistanceSquared(sender.ServerPosition, LeeMethods.Player.ServerPosition) <= 350 * 350 ||
                !LeeMethods.E.IsReady(250))
            {
                return;
            }
            if (args.SData.Name == "MonkeyKingDecoy" || args.SData.Name == "AkaliSmokeBomb")
            {
                Utility.DelayAction.Add(250, () => LeeMethods.E.Cast());
            }
        }
    }
}