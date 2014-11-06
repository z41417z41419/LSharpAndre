using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Leesin
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                if (ObjectManager.Player.ChampionName == "LeeSin")
                {
                    // ReSharper disable once ObjectCreationAsStatement
                    new LeeSinSharp();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to load LeeSin#: " + exception);
            }
        }
    }
}
