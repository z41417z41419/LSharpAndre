using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Leesin
{
    internal static class LeeUtility
    {
        public static int WaittingForWard;
        public static Vector3 WardCastPosition;

        public static void SendMessage(string msg)
        {
            Game.PrintChat("<font color=\"#00BFFF\">LeeSin# -</font> <font color=\"#FFFFFF\">" + msg + "</font>");
        }

        public static Vector3 GetInsecVector3(Obj_AI_Hero target, bool flash, Vector2 defaultPos)
        {
            var distance = flash ? 100 : Config.Menu.Item("insecDistance").GetValue<Slider>().Value;
            Vector3 insecPosition;
            var qwFlashSpell = LeeMethods.Q;
            qwFlashSpell.Delay += flash ? 0 : 250;
            var predictedPosition = qwFlashSpell.GetPrediction(target).CastPosition;
            var towerList =
                ObjectManager.Get<Obj_Turret>()
                    .Where(
                        t =>
                            t.IsAlly && !t.IsDead &&
                            Vector3.DistanceSquared(t.Position, predictedPosition) <= 1200 * 1200)
                    .ToList();
            var championList =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        h =>
                            h.IsAlly && !h.IsMe && !h.IsDead &&
                            Vector3.DistanceSquared(h.Position, predictedPosition) <= 1200 * 1200)
                    .OrderBy(h => Vector3.DistanceSquared(h.Position, predictedPosition))
                    .ToList();
            //var distance = Config.Menu.Item("insecDistance").GetValue<Slider>().Value;
            if (towerList.Count > 0 && MenuParamBool("insectToTower"))
            {
                insecPosition = predictedPosition.To2D().Extend(towerList[0].Position.To2D(), -distance).To3D();
            }
            else if (championList.Count > 0 && MenuParamBool("insectToAlly"))
            {
                var midPoint = new Vector2();
                foreach (var hero in championList)
                {
                    midPoint.X += hero.ServerPosition.X;
                    midPoint.Y += hero.ServerPosition.Y;
                }
                midPoint = new Vector2(midPoint.X / championList.Count, midPoint.Y / championList.Count);
                insecPosition = predictedPosition.To2D().Extend(midPoint, -distance).To3D();
            }
            else
            {
                insecPosition = predictedPosition.To2D().Extend(defaultPos, -distance).To3D();
            }
            return insecPosition;
        }

        public static void WardJump(Vector3 pos, bool useWard = true, bool checkObjects = true)
        {
            if (!LeeMethods.W.IsReady() || LeeMethods.W.Instance.Name == "blindmonkwtwo" ||
                WaittingForWard > Environment.TickCount)
            {
                return;
            }
            WardCastPosition = NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall)
                ? LeeMethods.Player.GetPath(pos).Last()
                : pos;
            var jumpObject =
                ObjectManager.Get<Obj_AI_Base>()
                    .OrderBy(obj => obj.Distance(LeeMethods.Player.ServerPosition))
                    .FirstOrDefault(
                        obj =>
                            obj.IsAlly && !obj.IsMe &&
                            (!(obj.Name.IndexOf("turret", StringComparison.InvariantCultureIgnoreCase) >= 0) &&
                             //Doesn't create a new substring KappaQ
                             Vector3.DistanceSquared(pos, obj.ServerPosition) <= 150 * 150));
            if (jumpObject != null && checkObjects)
            {
                LeeMethods.W.CastOnUnit(jumpObject);
                WaittingForWard = Environment.TickCount;
                return;
            }
            if (!useWard)
            {
                return;
            }
            var ward = Items.GetWardSlot();
            if (ward == null || ward.Stacks == 0)
            {
                return;
            }
            ward.UseItem(WardCastPosition);
            WaittingForWard = Environment.TickCount + 250 + Game.Ping;
        }

        public static bool MenuParamBool(string menuName)
        {
            return Config.Menu.Item(menuName).GetValue<bool>();
        }

        public static bool CastQ(Obj_AI_Hero target, QMode qMode)
        {
            if (!LeeMethods.Q.IsReady() || LeeMethods.Q.Instance.Name != "BlindMonkQOne")
            {
                return false;
            }
            var casted = false;
            var qPrediction = LeeMethods.Q.GetPrediction(target);
            var useSmite = LeeSinSharp.SmiteSlot != SpellSlot.Unknown &&
                           LeeMethods.Player.SummonerSpellbook.CanUseSpell(LeeSinSharp.SmiteSlot) == SpellState.Ready &&
                           (qPrediction.CollisionObjects.Count == 1 && qPrediction.Hitchance.Equals(HitChance.High) &&
                            ((qMode == QMode.Combo && MenuParamBool("smiteCombo")) ||
                             (qMode == QMode.Insec && MenuParamBool("smiteInsec")) ||
                             (qMode == QMode.Harass && MenuParamBool("smiteHarass"))));
            if (useSmite)
            {
                LeeMethods.Player.SummonerSpellbook.CastSpell(LeeSinSharp.SmiteSlot, qPrediction.CollisionObjects[0]);
                LeeMethods.Q.Cast(qPrediction.CastPosition);
                casted = true;
            }
            else if (qPrediction.CollisionObjects.Count == 0)
            {
                LeeMethods.Q.Cast(qPrediction.CastPosition);
                casted = true;
            }

            return casted;
        }

        public static int BuffCount(string buffName)
        {
            var buff = LeeMethods.Player.Buffs.FirstOrDefault(b => b.Name == buffName);
            return buff == null ? 0 : buff.Count;
        }

        public static List<Vector2> Rectangle(Vector2 startVector2, Vector2 endVector2, float radius)
        {
            var points = new List<Vector2>();
            var o = new Vector2(-endVector2.Y + startVector2.Y, endVector2.X - endVector2.Y);
            var lenght = Math.Sqrt(o.X * o.X + o.Y * o.Y);
            o.X = (float) (o.X / lenght * (radius * 2));
            o.Y = (float) (o.Y / lenght * (radius * 2) / 2);

            points.Add(startVector2 + o);
            points.Add(startVector2 - o);
            points.Add(endVector2 - o);
            points.Add(endVector2 + o);
            return points;
        }
    }

    internal enum QMode
    {
        Combo,
        Insec,
        Harass
    }

    internal enum HarassStage
    {
        Nothing,
        Started,
        Doing,
        Finished
    }
}