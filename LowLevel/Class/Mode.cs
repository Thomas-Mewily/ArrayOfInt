using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LowLevel.Game.CType;
using static LowLevel.Game.Mode;

namespace LowLevel.Game
{
    public class Mode
    {
        /// <summary>
        /// nb :
        /// 0 v
        /// 1 <
        /// 2 ^
        /// 3 >
        /// </summary>
        
        // Boolean : 0 : false, 1 : true (or other value)

        private Mode() { }

        public const int Spacing = 4;

        public enum ModeEnum 
        {
            ERROR = -1 * Spacing,
            Copy = 0 * Spacing,
            Swap = 1 * Spacing,
            Goto = 2 * Spacing,
            CpuDirection = 3 * Spacing,
            StopCpu = 4 * Spacing,
            If = 5 * Spacing,

            WriteConsole = 8 * Spacing,
            ReadConsole = 9 * Spacing,
            SetConsoleColor = 10 * Spacing,
            ClearConsole = 11 * Spacing,

            GetTime = 16 * Spacing,
            
            PlusInt = 32 * Spacing,
            MinusInt = 33 * Spacing,
            MultiplyInt = 34 * Spacing,
            DivideInt = 35 * Spacing,
            ModuloInt = 36 * Spacing,
            RShiftInt = 37 * Spacing,
            LShiftInt = 38 * Spacing,
            AbsInt = 39 * Spacing,
            PowInt = 40 * Spacing,
            SqrtInt = 41 * Spacing,

            EqualInt = 48 * Spacing,
            DifferentInt = 49 * Spacing,
            SuperiorInt = 50 * Spacing,
            SuperiorOrEqualInt = 51 * Spacing,
            InferiorInt = 52 * Spacing,
            InferiorOrEqualInt = 53 * Spacing,

            // Boolean (0 :  false, other value (generally 1) : true)
            Lazy_OR = 64 * Spacing,
            OR = 65 * Spacing,
            Lazy_AND = 66 * Spacing,
            AND = 67 * Spacing,
            NOT = 68 * Spacing,
        }

        public static Dictionary<int, string> Representation = new Dictionary<int, string>()
        {
            { (int)ModeEnum.ERROR, "ERROR" },
            { (int)ModeEnum.Copy, "copy" },
            { (int)ModeEnum.Swap, "swap" },
            { (int)ModeEnum.Goto, "goto" },
            { (int)ModeEnum.CpuDirection, "dir" },
            { (int)ModeEnum.StopCpu, "stop" },
            { (int)ModeEnum.If, "if" },

            { (int)ModeEnum.WriteConsole, "write" },
            { (int)ModeEnum.ReadConsole, "read" },
            { (int)ModeEnum.SetConsoleColor, "color" },
            { (int)ModeEnum.ClearConsole, "clear" },

            { (int)ModeEnum.GetTime, "time" },

            { (int)ModeEnum.PlusInt, " + " },
            { (int)ModeEnum.MinusInt, "-" },
            { (int)ModeEnum.MultiplyInt, "*" },
            { (int)ModeEnum.DivideInt, "/" },
            { (int)ModeEnum.ModuloInt, "%" },
            { (int)ModeEnum.RShiftInt, ">>" },
            { (int)ModeEnum.LShiftInt, "<<" },
            { (int)ModeEnum.AbsInt, "abs" },
            { (int)ModeEnum.PowInt, "pow" },
            { (int)ModeEnum.SqrtInt, "sqrt" },

            { (int)ModeEnum.EqualInt, "==" },
            { (int)ModeEnum.DifferentInt, "!=" },
            { (int)ModeEnum.SuperiorInt, ">" },
            { (int)ModeEnum.SuperiorOrEqualInt, ">=" },
            { (int)ModeEnum.InferiorInt, "<" },
            { (int)ModeEnum.InferiorOrEqualInt, "<=" },


            { (int)ModeEnum.Lazy_OR, "||" },
            { (int)ModeEnum.OR, "|" },
            { (int)ModeEnum.Lazy_AND, "&&" },
            { (int)ModeEnum.AND, "&" },
            { (int)ModeEnum.NOT, "not" },
        };

        public static string GetRepresentation(ModeEnum mode) => GetRepresentation((int)mode);
        /// <summary>
        /// Result have 4 letters
        /// </summary>
        /// <param name="nb"></param>
        /// <returns></returns>
        public static string GetRepresentation(int nb) 
        {
            if (Representation.ContainsKey(GetWithoutDirection(nb)) == false) { return "????"; }
            var s = Representation[GetWithoutDirection(nb)];
            return s;
        }

        public static int GetDirectionX(int nb) => DirectionXArray[GetNb(nb)];
        public static int GetDirectionY(int nb) => DirectionYArray[GetNb(nb)];

        public static int GetNb(int nb) => nb % Spacing;
        public static int GetWithoutDirection(int nb) => nb - nb % Spacing;

        public static char GetCharDirection(int nb) => DirectionString[GetNb(nb)];
        public static char GetCharDirection(ModeEnum nb) => GetCharDirection((int)nb);

        public const string DirectionString = "v<^>";

        public static int[] DirectionXArray = new int[] { 0, -1, 0, 1 };
        public static int[] DirectionYArray = new int[] { 1, 0, -1, 0 };
    }

}
