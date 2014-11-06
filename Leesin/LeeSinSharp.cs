using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

namespace Leesin
{
    class LeeSinSharp
    {
        public LeeSinSharp()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Config();
            Game.PrintChat("<font color=\"#00BFFF\">LeeSin# -</font> <font color=\"#FFFFFF\">Loaded</font>");
        }
    }
}
