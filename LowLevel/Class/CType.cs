using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LowLevel.Game.CType;
using static LowLevel.Game.Mode;

namespace LowLevel.Game
{
    public class CType
    {
        private CType() { }

        public enum TypeEnum 
        {
            Free = 0,

            FixedPointer = 1,
            DirectFixedPointer = 2, //Can get the "value" of a pointer
            RelativePointer = 3,
            DirectRelativePointer = 4,
            CpuPointer = 5,
            DirectCpuPointer = 6,

            TileIdx = 8,
            CpuTileIdx = 9,

            Instruction = 16,

            Int = 32,
            Color = 33,
            Letter = 34,
        }
    }
}
