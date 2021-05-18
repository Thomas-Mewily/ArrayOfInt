using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LowLevel.Game.CType;
using static LowLevel.Game.Mode;

namespace LowLevel.Game
{
    public class MemoryEditor
    {
        public Memory Memory;

        public int X;
        public int Y;

        /// <summary>
        /// When == -1
        /// </summary>
        public int TpOldPosition = -1;
        public int OldX;
        public int OldY;

        public int AddX = 2;
        public int AddY = 0;

        public MemoryEditor(Memory memory, int x, int y)
        {
            Memory = memory;
            X = x;
            Y = y;
        }

        public MemoryEditor Set(string name)
        {
            if (string.IsNullOrEmpty(name)) { return this; }

            if ("-0123456789".Contains(name[0]))
            {
                Set(TypeEnum.Int, int.Parse(name));
                return this;
            }

            int direction = Mode.DirectionString.IndexOf(name[^1]);
            if (direction < 0)
            {
                direction = 0;
            }
            else
            {
                name = name.Substring(0, name.Length - 1);
            }

            var type = name switch
            {
                "+" => ModeEnum.PlusInt,
                "-" => ModeEnum.MinusInt,
                "*" => ModeEnum.MultiplyInt,
                "/" => ModeEnum.DivideInt,
                "%" => ModeEnum.ModuloInt,
                "<<" => ModeEnum.LShiftInt,
                ">>" => ModeEnum.RShiftInt,
                "copy" => ModeEnum.Copy,
                "swap" => ModeEnum.Swap,
                "goto" => ModeEnum.Goto,
                _ => ModeEnum.ERROR,
            };

            if (type == ModeEnum.ERROR)
            {
                throw new Exception("Error when loading name " + name);
            }

            OldX = X;
            OldY = Y;
            TpOldPosition = 3;
            Set(TypeEnum.Instruction, ((int)type) + direction);

            return this;
        }

        public MemoryEditor Set(TypeEnum type, int value) => Set((int)type, value);
        public MemoryEditor Set(int type, ModeEnum value) => Set(type, (int)value);
        public MemoryEditor Set(TypeEnum type, ModeEnum value) => Set((int)type, (int)value);
        public MemoryEditor Set(int type, int value)
        {
            if (TpOldPosition >= 0)
            {
                TpOldPosition--;
                if (TpOldPosition == 0)
                {
                    X = OldX;
                    Y = OldY;
                }
            }

            Memory[X, Y] = type;
            Memory[X + 1, Y] = value;

            if (type == (int)TypeEnum.Instruction)
            {
                X += 2 * GetDirectionX(value);
                Y += GetDirectionY(value);
            }
            else
            {
                X += AddX;
                Y += AddY;
            }
            return this;
        }
    }


    /*
        0 1 2 3
        4 5 6 7
    */



}
