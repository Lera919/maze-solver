using System;
using System.Collections.Generic;
using System.Text;

namespace Maze
{
    /// <summary>
    /// enam of possible ways.
    /// </summary>
    public enum Way
    {
        /// <summary>
        /// Start position.
        /// </summary>
        Start = 0,

        /// <summary>
        /// Valid patth in maze.
        /// </summary>
        Path = -1,

        /// <summary>
        /// Wall in maze.
        /// </summary>
        Wall = -2,
    }
}
