﻿/****************************************************
 * Class: MazeCell
 * Description: Describes a single maze cell
 * Author(s): Matti Jokitulppo
 * Date: April 1, 2014
****************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ErppuTribute
{

    class MazeCell
    {
        public bool[] Walls = new bool[4] { true, true, true, true };
        public bool Visited = false;

    }
}
