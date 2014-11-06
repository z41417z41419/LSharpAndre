using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

namespace Leesin
{
    class Utility
    {
        public static void SendMessage(string msg)
        {
            Game.PrintChat("<font color=\"#00BFFF\">LeeSin# -</font> <font color=\"#FFFFFF\">" + msg + "</font>");
        }
    }
}
