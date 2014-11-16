using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Leesin
{
    internal static class LeeMethods
    {
        public static Obj_AI_Hero Player;

        public static readonly Spell Q = new Spell(SpellSlot.Q, 1100),
            W = new Spell(SpellSlot.W, 700),
            E = new Spell(SpellSlot.E, 350),
            R = new Spell(SpellSlot.R, 375);

        private static Vector3 _harassInitialVector3;
        private static HarassStage _harassStage = HarassStage.Nothing;
        public static readonly string[] JungleCamps = { "Worm", "Dragon", "LizardElder", "AncientGolem" };
        public static readonly string[] SmallMinionCamps = { "Wraith", "Golem", "GreatWraith", "GiantWolf" };
        private static int waitForSpell;

        public static void Harass(Obj_AI_Hero targetHero, bool useW = true)
        {
            if (!targetHero.IsValidTarget())
            {
                _harassStage = HarassStage.Nothing;
                return;
            }
            switch (_harassStage)
            {
                case HarassStage.Nothing:
                    _harassStage = HarassStage.Started;
                    break;
                case HarassStage.Started:
                    _harassStage = HarassStage.Doing;
                    if (E.IsReady() &&
                        Vector3.DistanceSquared(Player.ServerPosition, targetHero.ServerPosition) <= 350 * 350 &&
                        LeeUtility.MenuParamBool("UseE1H"))
                    {
                        E.Cast();
                        if (LeeUtility.MenuParamBool("UseE2H"))
                        {
                            Utility.DelayAction.Add(
                                250 - Game.Ping / 2 + 10, () =>
                                {
                                    E.Cast(Player, true);
                                    _harassStage = LeeUtility.MenuParamBool("UseQ1H") && Q.IsReady()
                                        ? HarassStage.Doing
                                        : HarassStage.Finished;
                                });
                        }
                    }
                    else
                    {
                        _harassStage = HarassStage.Finished;
                    }
                    if (Q.IsReady() && LeeUtility.MenuParamBool("UseQ1H"))
                    {
                        //LeeUtility.CastQ(targetHero, QMode.Harass);
                        if (LeeUtility.CastQ(targetHero, QMode.Harass) && LeeUtility.MenuParamBool("UseQ2H"))
                        {
                            var delay = (int) (targetHero.Distance(Player) / Q.Speed * 1000) + 300 + Game.Ping;
                            Utility.DelayAction.Add(
                                delay, () =>
                                {
                                    Q.Cast(targetHero);
                                    _harassInitialVector3 = Player.ServerPosition;
                                });
                            Utility.DelayAction.Add(delay * 2, () => _harassStage = HarassStage.Finished);
                        }
                        else
                        {
                            _harassStage = HarassStage.Finished;
                        }
                    }
                    else
                    {
                        _harassStage = HarassStage.Finished;
                    }
                    break;
                case HarassStage.Doing:
                    break;
                case HarassStage.Finished:
                    if (LeeUtility.MenuParamBool("UseWH") && useW)
                    {
                        LeeUtility.WardJump(_harassInitialVector3, LeeUtility.MenuParamBool("UseWardWH"));
                    }
                    _harassStage = HarassStage.Nothing;
                    break;
            }
        }

        public static void Combo(Obj_AI_Hero targetHero) //Thanks Roach_ For helping me with combo
        {
            if (!targetHero.IsValidTarget())
            {
                return;
            }
            var enemyQBuffed = targetHero.HasBuff("BlindMonkQOne", true);
            var autoAttacks = Config.Menu.Item("aaBetween").GetValue<Slider>().Value;
            var passiveCount = LeeUtility.BuffCount("blindmonkpassive_cosmetic");
            if (!enemyQBuffed && autoAttacks != 0 && passiveCount >= Math.Abs(autoAttacks - 2))
            {
                return;
            }
            if (LeeUtility.MenuParamBool("useQC") && Q.IsReady() && Q.Instance.Name == "BlindMonkQOne" && R.IsReady() &&
                Vector3.DistanceSquared(targetHero.ServerPosition, Player.ServerPosition) <= 375 * 375)
            {
                LeeUtility.CastQ(targetHero, QMode.Combo);
            }
            else if (LeeUtility.MenuParamBool("useQC") && enemyQBuffed && R.IsReady())
            {
                if (targetHero.Distance(Player) <= R.Range)
                {
                    R.Cast(targetHero, LeeUtility.MenuParamBool("packetCast"));
                    Utility.DelayAction.Add(750, () => Q.Cast(targetHero));
                }
                else
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, targetHero.ServerPosition);
                }
            }
            else
            {
                if (LeeUtility.MenuParamBool("useQC") && LeeUtility.CastQ(targetHero, QMode.Combo))
                {
                    Utility.DelayAction.Add(
                        (int) (targetHero.Distance(Player) / Q.Speed * 1000) + 300 + Game.Ping, () => Q.Cast());
                }
                if (Vector3.DistanceSquared(targetHero.ServerPosition, Player.ServerPosition) <= 350 * 350 &&
                    E.IsReady())
                {
                    if (E.IsReady() && LeeUtility.MenuParamBool("useE1C") &&
                        (E.Instance.Name == "BlindMonkEOne" || LeeUtility.MenuParamBool("useE2C")))
                    {
                        E.Cast();
                    }
                    else if (!Q.IsReady() && !E.IsReady() && !R.IsReady() && LeeUtility.MenuParamBool("useW1C") &&
                             (W.Instance.Name == "BlindMonkWOne" || LeeUtility.MenuParamBool("useW2C")) &&
                             Player.Mana - 50 >= 50)
                    {
                        W.Cast();
                    }
                }
            }
        }

        public static void InsecCombo(Obj_AI_Hero targetHero)
        {
            if (((!W.IsReady() || Items.GetWardSlot() == null) &&
                 Player.SummonerSpellbook.CanUseSpell(LeeSinSharp.FlashSlot) != SpellState.Ready) || !R.IsReady() ||
                !Q.IsReady() || Q.Instance.Name != "BlindMonkQOne")
            {
                if (Config.Menu.Item("insec1").GetValue<KeyBind>().Active ||
                    Config.Menu.Item("insec2").GetValue<KeyBind>().Active)
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
                return;
            }
            var useFlash = LeeUtility.MenuParamBool("useFlashInsec") &&
                           ((Config.Menu.Item("insecMode").GetValue<StringList>().SelectedIndex == 0 &&
                             (!W.IsReady() || Items.GetWardSlot() == null)) ||
                            (Config.Menu.Item("insecMode").GetValue<StringList>().SelectedIndex == 1 &&
                             Player.SummonerSpellbook.CanUseSpell(LeeSinSharp.FlashSlot) == SpellState.Ready));
            //Console.WriteLine(Game.Time + useFlash.ToString());
            //useFlash = true;

            var pos = Player.ServerPosition.To2D();

            if (Config.Menu.Item("insec1").GetValue<KeyBind>().Active)
            {
                if (LeeUtility.CastQ(targetHero, QMode.Insec))
                {
                    var delay = (int) (Player.Distance(targetHero) / Q.Speed * 1000) + 300 + Game.Ping;
                    Utility.DelayAction.Add(delay, () => Q.Cast());
                    if (useFlash)
                    {
                        Utility.DelayAction.Add(
                            (int) (delay * 1.2), () =>
                            {
                                if (Vector3.DistanceSquared(targetHero.ServerPosition, Player.ServerPosition) <=
                                    375 * 375)
                                {
                                    R.Cast(targetHero);
                                    //Console.WriteLine("casted R");
                                    Utility.DelayAction.Add(
                                        200 - Game.Ping,
                                        () =>
                                            Player.SummonerSpellbook.CastSpell(
                                                LeeSinSharp.FlashSlot, LeeUtility.GetInsecVector3(targetHero, true, pos)));
                                }
                            });
                    }
                    else
                    {
                        var wardJumpPos = LeeUtility.GetInsecVector3(targetHero, false, pos);
                        Utility.DelayAction.Add(
                            (int) (delay * 1.1), () =>
                            {
                                if (Vector3.DistanceSquared(wardJumpPos, Player.ServerPosition) <= 600 * 600)
                                {
                                    LeeUtility.WardJump(wardJumpPos, true, false);
                                    //Utility.DelayAction.Add(300 + Game.Ping, () => LeeUtility.WardJump(wardJumpPos, true, false));
                                    Utility.DelayAction.Add(
                                        250 + Game.Ping,
                                        () => R.Cast(targetHero, LeeUtility.MenuParamBool("packetCast")));
                                }
                            });
                    }
                }
            }
            else if (Config.Menu.Item("insec2").GetValue<KeyBind>().Active)
            {
                var insecPos = LeeUtility.GetInsecVector3(targetHero, useFlash, pos);
                var inDistance = Player.Distance(insecPos) <= (useFlash ? 400 : 600);
                if (useFlash)
                {
                    if (inDistance &&
                        Vector3.DistanceSquared(Player.ServerPosition, targetHero.ServerPosition) <= 375 * 375)
                    {
                        R.Cast(targetHero);
                        Utility.DelayAction.Add(
                            200 - Game.Ping, () => Player.SummonerSpellbook.CastSpell(LeeSinSharp.FlashSlot, insecPos));
                        Utility.DelayAction.Add(
                            250, () =>
                            {
                                Q.Cast(targetHero);
                                var delay = (int) (Player.Distance(targetHero) / Q.Speed * 1000) + 300 + Game.Ping;
                                Utility.DelayAction.Add(delay + 500, () => Q.Cast(targetHero));
                            });
                    }
                    else
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                }
                else
                {
                    if (inDistance)
                    {
                        LeeUtility.WardJump(insecPos);
                        Utility.DelayAction.Add(250 + Game.Ping, () => R.Cast(targetHero, false));
                        Utility.DelayAction.Add(
                            250, () =>
                            {
                                var delay = (int) (Player.Distance(targetHero) / Q.Speed * 1000) + 300 + Game.Ping;
                                Q.Cast(targetHero);
                                Utility.DelayAction.Add(delay + 500, () => Q.Cast(targetHero));
                            });
                    }
                    else
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                }
            }
        }

        public static void KSer()
        {
            foreach (var hero in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        h => h.IsEnemy && h.Distance(Player) < (Q.IsReady() ? Q.Range : R.Range) && h.IsValidTarget())
                    .OrderBy(h => h.Health))
            {
                if (LeeUtility.MenuParamBool("useQ1KS"))
                {
                    var predictedHealth = Q.GetHealthPrediction(hero);
                    var q1Damage = Q.GetDamage(hero, 0);
                    if (Q.IsReady() && predictedHealth <= q1Damage)
                    {
                        Q.Cast(hero);
                    }
                    else if (Q.IsReady() && LeeUtility.MenuParamBool("useQ2KS") &&
                             predictedHealth < q1Damage + Q.GetDamage(hero, 1))
                    {
                        Q.Cast(hero);
                        Utility.DelayAction.Add(
                            (int) (Math.Ceiling(Player.Distance(hero) / Q.Speed * 1000) + 300 + Game.Ping),
                            () => Q.Cast(Player));
                    }
                }
                if (E.IsReady() && LeeUtility.MenuParamBool("useE1KS") &&
                    Vector3.DistanceSquared(Player.ServerPosition, hero.ServerPosition) <= 350 * 350 &&
                    E.GetHealthPrediction(hero) <= W.GetDamage(hero))
                {
                    E.Cast();
                }
                if (R.IsReady() && LeeUtility.MenuParamBool("useRKS") &&
                    Vector3.DistanceSquared(Player.ServerPosition, hero.ServerPosition) <= 375 * 375 &&
                    R.GetHealthPrediction(hero) <=
                    R.GetDamage(hero) * Config.Menu.Item("rOverKill").GetValue<Slider>().Value / 100)
                {
                    R.Cast();
                }
                if (R.IsReady() && LeeUtility.MenuParamBool("useRCollisionKS") &&
                    Vector3.DistanceSquared(Player.ServerPosition, hero.ServerPosition) <= 375 * 375)
                {
                    var hero1 = hero;
                    var startPos = Player.ServerPosition.To2D();
                    var endPos = Player.ServerPosition.To2D().Extend(hero1.ServerPosition.To2D(), 1200);

                    var polygon = new Polygon(LeeUtility.Rectangle(startPos, endPos, hero1.BoundingRadius));
                    foreach (var victim in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(
                                h =>
                                    h.IsEnemy && h != hero1 &&
                                    R.GetHealthPrediction(h) <=
                                    R.GetDamage(h) * Config.Menu.Item("rOverKill").GetValue<Slider>().Value / 100))
                    {
                        if (polygon.Contains(victim.ServerPosition.To2D()))
                        {
                            R.Cast(hero1);
                        }
                    }
                }
            }
        }

        public static void LaneClear()
        {
            if (Player.HasBuff("blindmonkpassive_cosmetic", true) || waitForSpell >= Environment.TickCount)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Player.ServerPosition, E.Range);
            var jungleMinions = MinionManager.GetMinions(
                Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral);
            minions.AddRange(jungleMinions);
            minions.RemoveAll(m => m.Name.IndexOf("ward", StringComparison.InvariantCultureIgnoreCase) >= 0);
            //Doesn't create a new substring Kappa
            foreach (var minion in minions.OrderByDescending(m => m.MaxHealth).ThenBy(m => m.Health))
            {
                if (LeeUtility.MenuParamBool("UseEW") && E.IsReady() &&
                    Vector3.DistanceSquared(minion.ServerPosition, Player.ServerPosition) <= 350 * 350)
                {
                    Console.WriteLine("E");
                    E.Cast();
                    waitForSpell = Environment.TickCount + 250 + Game.Ping;
                }
                else if (LeeUtility.MenuParamBool("UseQW") && Q.IsReady() &&
                         Q.GetHealthPrediction(minion) <= Q.GetDamage(minion))
                {
                    Q.Cast(minion);
                    waitForSpell = Environment.TickCount + 250 + Game.Ping;
                }
                else if (LeeUtility.MenuParamBool("UseWW") && W.IsReady())
                {
                    W.Cast();
                    waitForSpell = Environment.TickCount + 250 + Game.Ping;
                }
            }
        }

        public static void Smite()
        {
            var jungleMinions = MinionManager.GetMinions(
                Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral);
            if (jungleMinions != null)
            {
                foreach (var jungleMinion in
                    jungleMinions.Where(
                        jungleMinion =>
                            JungleCamps.Any(j => jungleMinion.Name.StartsWith(j)) ||
                            SmallMinionCamps.Any(j => jungleMinion.Name.StartsWith(j)))
                        .Where(
                            jungleMinion =>
                                LeeUtility.MenuParamBool(jungleMinion.Name) &&
                                jungleMinion.Health <=
                                Player.GetSummonerSpellDamage(jungleMinion, Damage.SummonerSpell.Smite)))
                {
                    Player.SummonerSpellbook.CastSpell(LeeSinSharp.SmiteSlot, jungleMinion);
                }
            }
        }

        public static void CampStealer()
        {
            var jungleMinions = MinionManager.GetMinions(
                Player.ServerPosition, Q.IsReady() ? Q.Range : 760, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (jungleMinions != null)
            {
                foreach (var jungleMinion in
                    jungleMinions.Where(
                        minion =>
                            Q.GetHealthPrediction(minion) <
                            Q.GetDamage(minion) + Q.GetDamage(minion, 1) +
                            Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite))
                        .Where(
                            jungleMinion =>
                                JungleCamps.Any(j => jungleMinion.Name.StartsWith(j)) ||
                                SmallMinionCamps.Any(j => jungleMinion.Name.StartsWith(j)))
                        .Where(jungleMinion => LeeUtility.MenuParamBool(jungleMinion.Name)))
                {
                    Q.Cast(jungleMinion);
                    var backPos = Player.ServerPosition;
                    Utility.DelayAction.Add(250, () => Q.Cast());
                    var minion = jungleMinion;
                    Utility.DelayAction.Add(
                        (int) ((Player.Distance(jungleMinion) - 725) / Q.Speed * 1000) + 300 + Game.Ping, () =>
                        {
                            Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite);
                            LeeUtility.WardJump(backPos);
                        });
                    Player.SummonerSpellbook.CastSpell(LeeSinSharp.SmiteSlot, jungleMinion);
                }
            }
        }
    }
}