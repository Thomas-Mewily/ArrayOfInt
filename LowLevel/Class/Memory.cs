using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LowLevel.Game.CType;
using static LowLevel.Game.Mode;

namespace LowLevel.Game
{
    public class Memory
    {
        /// <summary>
        /// [y][x]
        /// </summary>
        public int[,] Data;
        public ExtraConsole ExtraConsole;
        public List<CPU> Cpu = new List<CPU>();

        public Memory() 
        {

        }


        public int SizeX => Data.GetLength(1);
        public int SizeY => Data.GetLength(0);
        public int Length => Data.Length;

        public int this[int x, int y]
        {
            get => Data[y, x];
            set => Data[y, x] = value;
        }
        public int this[int tile]
        {
            get => this[GetTileX(tile), GetTileY(tile)];
            set => this[GetTileX(tile), GetTileY(tile)] = value;
        }

        public bool IsInside(int idx) => idx >= 0 && idx < Length;
        public bool IsInside(int x, int y) => x >= 0 && x < SizeX && y >= 0 && y < SizeY;

        public void Reset(int sizeX, int sizeY) 
        {
            Data = new int[Math.Max(1, sizeY), Math.Max(2, sizeX - sizeX % 2)];
            ExtraConsole = new ExtraConsole();
        }

        public void Place(int x, int y, int value) => this[x, y] = value;
        public void Place(int x, int y, TypeEnum value) => Place(x, y, (int)value);
        public void Place(int x, int y, ModeEnum value) => Place(x, y, (int)value);

        public void Place(int x, int y, int type, int value)            { Place(x, y, type); Place(x + 1, y, value); }
        public void Place(int x, int y, TypeEnum type, int value)       { Place(x, y, type); Place(x + 1, y, value); }
        public void Place(int x, int y, TypeEnum type, ModeEnum value)  { Place(x, y, type); Place(x + 1, y, value); }
        public void Place(int x, int y, int type, ModeEnum value)       { Place(x, y, type); Place(x + 1, y, value); }

        public void PlaceChars(int xType, int yType, string chars) 
        {
            for(int i = 0; i < chars.Length; i++)
            {
                Place(xType, yType, TypeEnum.Letter, chars[i]);
                yType += ((xType+=2) / SizeX - (xType-2) / SizeX);
                xType %= SizeX;
            }
        }


        public int GetTileX(int tileIdx) => tileIdx % SizeX;
        public int GetTileY(int tileIdx) => tileIdx / SizeX;
        public int GetTile(int x, int y) => y * SizeX + x;

        public void AddCpu(int registerX, int registerY) => AddCpu(GetTile(registerX, registerY));
        public void AddCpu(int registerIdx)
        {
            CPU cpu = new CPU(this, registerIdx);
            Cpu.Add(cpu);
        }

        public override string ToString() => GetString(Useful.ToHexa);
        //Thank to https://stackoverflow.com/questions/46630565/convert-2d-array-to-string-in-c-looking-for-most-elegant-way
        public string GetString(Func<int, string> effect) => string.Join(",", Enumerable.Range(0, Data.GetUpperBound(0) + 1).Select(x => Enumerable.Range(0, Data.GetUpperBound(1) + 1).Select(y => Data[x, y])).Select(z => "{" + string.Join(",", z.Select(i => effect(i))) + "}"));

        public void Execute() 
        {
            foreach(var c in Cpu.ToList()) 
            {
                c.Execute();
            }
        }

        public void ExecuteStep() 
        {
            foreach (var c in Cpu.ToList())
            {
                c.ExecuteStep();
            }
        }

        #region Debug
        public void PrintHexa() => Print(Useful.ToHexa);
        public void PrintBase10() => Print(Useful.ToBase10);

        public void Print(Func<int, string> effect, bool showAdress = true)
        {
            if (showAdress)
            {
                Useful.ResetConsoleColor(AdressColor);
                Console.Write("Adress ");
            }
            Useful.ResetConsoleColor(TypeColor);
            Console.Write("Type ");
            Useful.ResetConsoleColor(Value);
            Console.WriteLine("Value ");

            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x += 2)
                {
                    if (showAdress)
                    {
                        Useful.ResetConsoleColor(AdressColor);
                        Console.Write(effect(GetTile(x, y)) + " ");
                    }
                    Useful.ResetConsoleColor(TypeColor);
                    Console.Write(effect(this[x, y]) + " ");
                    Useful.ResetConsoleColor(Value);
                    Console.Write(effect(this[x + 1, y]) + " ");

                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static ConsoleColor AdressColor = ConsoleColor.DarkGreen;
        public static ConsoleColor Value = ConsoleColor.White;

        public static ConsoleColor TypeColor = ConsoleColor.Yellow;

        public static ConsoleColor FixedPointer = ConsoleColor.Green;
        public static ConsoleColor RelativePointer = ConsoleColor.Yellow;
        public static ConsoleColor CpuPointer = ConsoleColor.Cyan;
        public static ConsoleColor TileIdx = ConsoleColor.DarkYellow;
        public static ConsoleColor CpuTileIdx = ConsoleColor.DarkCyan;
        public static ConsoleColor CpuIsHereBackground = ConsoleColor.DarkMagenta;
        public static ConsoleColor Free = ConsoleColor.DarkGray;
        public static ConsoleColor UnknowType = ConsoleColor.DarkRed;

        public void PrintPretty(bool showAdress = true) 
        {
            Useful.ResetConsoleColor();
            Console.Write(" ");
            if (showAdress)
            {
                Useful.ResetConsoleColor(AdressColor);
                Console.Write("Adress ");
            }
            Useful.ResetConsoleColor(Value);
            Console.Write("Value ");
            Useful.ResetConsoleColor(Free);
            Console.Write("Free ");
            Useful.ResetConsoleColor(FixedPointer);
            Console.Write("Fixed Pointer ");
            Useful.ResetConsoleColor(RelativePointer);
            Console.Write("Relative Pointer ");
            Useful.ResetConsoleColor(TileIdx);
            Console.Write("Tile Idx ");
            Useful.ResetConsoleColor(CpuPointer);
            Console.Write("Cpu Pointer ");
            Useful.ResetConsoleColor(CpuTileIdx);
            Console.Write("Cpu Tile Idx ");
            Useful.ResetConsoleColor(ConsoleColor.White);
            Console.Write("X : Cpu register -> ");
            Useful.ResetConsoleColor(ConsoleColor.White, CpuIsHereBackground);
            Console.Write("Cpu is here");
            Useful.ResetConsoleColor(UnknowType);
            Console.WriteLine(" Unknow Type ");

            Useful.ResetConsoleColor();
            Console.WriteLine(" " + Cpu.Count + " Cpu");

            int minLength = 11;
            int adressSize = (int)(Math.Log10(SizeX * SizeY) + 1);

            for (int y = 0; y < SizeY; y++) 
            {
                for (int x = 0; x < SizeX; x+=2)
                {
                    int adress = GetTile(x, y);
                    if (showAdress)
                    {
                        Useful.ResetConsoleColor(AdressColor);
                        //Console.Write(Useful.ToBase10(adress).LengthCenter(minLength));
                        Console.Write(" "+adress.ToString("D" + adressSize.ToString()));
                    }
                    Useful.ResetConsoleColor(Value, ConsoleColor.Black);
                    Console.Write(Cpu.Any(c => c.RegisterTile == adress) ? "X" : " ");
                    Useful.ResetConsoleColor(Value, Cpu.Any(c => c.InstructionTile == adress) ? CpuIsHereBackground : ConsoleColor.Black);

                    switch (this[x, y])
                    {
                        case (int)TypeEnum.Int:
                            break;
                        case (int)TypeEnum.Color:
                            Useful.ResetConsoleColor(ConsoleColor.Cyan, Console.BackgroundColor);
                            break;
                        case (int)TypeEnum.CpuPointer:
                            Useful.ResetConsoleColor(CpuPointer, Console.BackgroundColor);
                            break;
                        case (int)TypeEnum.FixedPointer:
                            Useful.ResetConsoleColor(FixedPointer, Console.BackgroundColor);
                            break;
                        case (int)TypeEnum.RelativePointer:
                            Useful.ResetConsoleColor(RelativePointer, Console.BackgroundColor);
                            break;
                        case (int)TypeEnum.Free:
                            Useful.ResetConsoleColor(Free, Console.BackgroundColor);
                            break;
                        case (int)TypeEnum.Instruction:
                            Console.Write(GetRepresentation(this[x + 1, y]).LengthCenter(minLength - 1) + GetCharDirection(this[x + 1, y]) + " ");
                            continue;
                        case (int)TypeEnum.Letter:
                            Useful.ResetConsoleColor(ConsoleColor.Gray, Console.BackgroundColor);
                            break;
                        case (int)TypeEnum.TileIdx:
                            Useful.ResetConsoleColor(TileIdx, Console.BackgroundColor);
                            break;
                        case (int)TypeEnum.CpuTileIdx:
                            Useful.ResetConsoleColor(CpuTileIdx, Console.BackgroundColor);
                            break;
                        default:
                            Useful.ResetConsoleColor(UnknowType, Console.BackgroundColor);
                            break;
                    }

                    Console.Write((this[x, y] != (int)TypeEnum.Instruction ? Useful.ToBase10(this[x + 1, y]) : GetStringRepresentation(x+1, y).LengthCenter(minLength)) + " ");
                }
                Useful.ResetConsoleColor();
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public string GetStringRepresentation(int tile) => GetStringRepresentation(GetTileX(tile), GetTileY(tile));
        public string GetStringRepresentation(int x, int y) 
        {
            if (GetTile(x, y) % 2 == 0) 
            {
                return ((TypeEnum)this[x, y]).ToString();
            }
            else 
            {
                x = GetTileX(GetTile(x - 1, y));
                //Value
                switch (this[x, y])
                {
                    case (int)TypeEnum.Color:
                        {
                            uint data = (uint)this[x + 1, y];
                            return "#" + Useful.ToHexa((byte)(data >> 24), 2) + Useful.ToHexa((byte)(data >> 16), 2) + Useful.ToHexa((byte)(data >> 8), 2) + Useful.ToHexa((byte)(data), 2);
                        }
                    case (int)TypeEnum.Instruction:
                        return GetRepresentation(this[x + 1, y]) + GetCharDirection(this[x + 1, y]);
                    case (int)TypeEnum.Letter:
                        return (this[x + 1, y] < 32 ? "\\" + ((int)this[x + 1, y]).ToString() : ((char)(this[x + 1, y])).ToString());
                    default:
                        return this[x + 1, y].ToString();
                }
            }

        }
        #endregion
    }
}
