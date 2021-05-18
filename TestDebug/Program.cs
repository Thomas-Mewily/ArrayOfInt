using LowLevel;
using LowLevel.Game;
using System;
using System.Diagnostics;
using System.IO;
using static LowLevel.Game.CType;
using static LowLevel.Game.Mode;

namespace TestDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            Useful.Load();
            Console.WriteLine("Hello World!");

            //TestLoopAndStop4();
            //TestSimpleIfLoop();

            Execute(FindXPrime(32));
            //Test3();

            Console.WriteLine("Over!");
            Console.ReadLine();
        }

        public static void Execute(Memory Memory) 
        {
            Memory.PrintPretty();

            DateTime start = DateTime.Now;
            Memory.Execute();
            DateTime end = DateTime.Now;

            Memory.PrintPretty();
            Memory.ExtraConsole.WriteToConsole();

            Console.WriteLine("Done in " + (end - start).TotalSeconds + " s");
        }

        public static void ExecuteDebug(Memory Memory)
        {
            Memory.PrintPretty();

            while (Memory.Cpu.Count != 0)
            {
                Console.ReadLine();
                Memory.ExecuteStep();
                Memory.PrintPretty();
                //Memory.PrintBase10();
                Memory.ExtraConsole.WriteToConsole();
            }


        }

        public static Memory FindXPrime(int nbPrime, bool writeInConsole = true)
        {
            var Memory = new Memory();

            Memory.Reset(12, 17);

            Memory.Place(0, 0, TypeEnum.FixedPointer, Memory.GetTile(0, 2));
            Memory.Place(2, 0, TypeEnum.Int, 0);
            Memory.Place(4, 0, TypeEnum.Int, 1);

            int nbX = 4 * 2;
            int nbY = 0;
            Memory.Place(nbX, nbY, TypeEnum.Int, 0);

            int iX = 5 * 2;
            int iY = 0;
            Memory.Place(iX, iY, TypeEnum.Int, 2);

            int pX = 4 * 2;
            int pY = 1;
            Memory.Place(pX, pY, TypeEnum.Int, nbPrime);

            int jX = 5 * 2;
            int jY = 1;
            Memory.Place(jX, jY, TypeEnum.Int, -1);

            int y = 1;
            Memory.Place(0, ++y, TypeEnum.Instruction, ModeEnum.Goto + 3);
            Memory.Place(2 * 1, y, TypeEnum.Int, Memory.GetTile(0, 7));

            Memory.Place(2*0, ++y, TypeEnum.Instruction, ModeEnum.PlusInt + 3);
            Memory.Place(2 * 1, y, TypeEnum.FixedPointer, Memory.GetTile(iX + 1, iY));
            Memory.Place(2 * 2, y, TypeEnum.Int, 1);
            Memory.Place(2 * 3, y, TypeEnum.FixedPointer, Memory.GetTile(iX + 1, iY));

            Memory.Place(2 * 5, 2, TypeEnum.Instruction, ModeEnum.InferiorInt);
            Memory.Place(2 * 5, 3, TypeEnum.FixedPointer, Memory.GetTile(nbX + 1, nbY));
            Memory.Place(2 * 5, 4, TypeEnum.FixedPointer, Memory.GetTile(pX + 1, pY));

            Memory.Place(2 * 0, ++y, TypeEnum.Instruction, ModeEnum.If + 3);
            Memory.Place(2 * 1, y, TypeEnum.FixedPointer, Memory.GetTile(2 * 5, 2));
            Memory.Place(2 * 2, y, TypeEnum.Int, 1);
            Memory.Place(2 * 3, y, TypeEnum.Int, Memory.GetTile(0, 2));

            Memory.Place(0, ++y, TypeEnum.Instruction, ModeEnum.StopCpu + 3);



            y++;
            Memory.Place(2 * 0, ++y, TypeEnum.Instruction, ModeEnum.Copy + 3);
            Memory.Place(2 * 1, y, TypeEnum.Int, 2);
            Memory.Place(2 * 2, y, TypeEnum.FixedPointer, Memory.GetTile(jX + 1, jY));

            {
                Memory.Place(0, ++y, TypeEnum.Instruction, ModeEnum.If + 3);
                Memory.Place(2 * 1, y, TypeEnum.FixedPointer, Memory.GetTile(2 * 1, y + 1));
                Memory.Place(2 * 2, y, TypeEnum.Int, 1);
                Memory.Place(2 * 3, y, TypeEnum.Int, Memory.GetTile(2 * 0, y + 6));

                Memory.Place(2 * 1, ++y, TypeEnum.Instruction, ModeEnum.SuperiorInt + 3);
                Memory.Place(2 * 2, y, TypeEnum.FixedPointer, Memory.GetTile(jX + 1, jY));
                Memory.Place(2 * 3, y, TypeEnum.FixedPointer, Memory.GetTile(2 * 1, y + 1));

                Memory.Place(2 * 1, ++y, TypeEnum.Instruction, ModeEnum.SqrtInt + 3);
                Memory.Place(2 * 2, y, TypeEnum.FixedPointer, Memory.GetTile(iX + 1, iY));
                Memory.Place(2 * 3, y, TypeEnum.Int, 2);
            }

            {
                Memory.Place(2 * 0, ++y, TypeEnum.Instruction, ModeEnum.If + 3);
                Memory.Place(2 * 1, y, TypeEnum.FixedPointer, Memory.GetTile(2 * 5, y - 1));
                Memory.Place(2 * 2, y, TypeEnum.Int, 0);
                Memory.Place(2 * 3, y, TypeEnum.Int, Memory.GetTile(0, 3));

                Memory.Place(2 * 5, y - 1, TypeEnum.Instruction, ModeEnum.ModuloInt);
                Memory.Place(2 * 5, y, TypeEnum.FixedPointer, Memory.GetTile(iX + 1, iY));
                Memory.Place(2 * 5, y + 1, TypeEnum.FixedPointer, Memory.GetTile(jX + 1, jY));
            }

            {
                Memory.Place(0, ++y, TypeEnum.Instruction, ModeEnum.PlusInt + 3);
                Memory.Place(2 * 1, y, TypeEnum.FixedPointer, Memory.GetTile(jX + 1, jY));
                Memory.Place(2 * 2, y, TypeEnum.Int, 1);
                Memory.Place(2 * 3, y, TypeEnum.FixedPointer, Memory.GetTile(jX + 1, jY));
            }


            Memory.Place(0, ++y, TypeEnum.Instruction, ModeEnum.Goto + 3);
            Memory.Place(2 * 1, y, TypeEnum.Int, Memory.GetTile(0, 8));

            if (writeInConsole) 
            {
                Memory.Place(0, ++y, TypeEnum.Instruction, ModeEnum.WriteConsole + 3);
                Memory.Place(2 * 1, y, TypeEnum.FixedPointer, Memory.GetTile(iX + 1, iY));
                Memory.Place(2 * 3, y, TypeEnum.Int, 1);
            }


            Memory.Place(0, ++y, TypeEnum.Instruction, ModeEnum.PlusInt + 3);
            Memory.Place(2 * 1, y, TypeEnum.FixedPointer, Memory.GetTile(nbX + 1, nbY));
            Memory.Place(2 * 2, y, TypeEnum.Int, 1);
            Memory.Place(2 * 3, y, TypeEnum.FixedPointer, Memory.GetTile(nbX + 1, nbY));

            Memory.Place(0, ++y, TypeEnum.Instruction, ModeEnum.Goto + 3);
            Memory.Place(2 * 1, y, TypeEnum.Int, Memory.GetTile(0, 3));




            Memory.AddCpu(0, 0);
            return Memory;
        }


        public static void TestSimpleIfLoop()
        {
            var Memory = new Memory();

            Memory.Reset(12, 12);

            Memory.Place(0, 0, TypeEnum.FixedPointer, Memory.GetTile(0, 3));
            Memory.Place(2, 0, TypeEnum.Int, 2);
            Memory.Place(4, 0, TypeEnum.Int, 0);


            Memory.Place(10, 0, TypeEnum.Int, 0);



            int xBegin = 2;

            Memory.Place(xBegin, 3, TypeEnum.TileIdx, 0);


            int x = 4;
            Memory.Place(x, 3, TypeEnum.Instruction, ModeEnum.WriteConsole);
            Memory.Place(x, 4, TypeEnum.FixedPointer, Memory.GetTile(11, 0));
            Memory.Place(x, 6, TypeEnum.Int, 1);

            x += 2;
            Memory.Place(x, 3, TypeEnum.Instruction, ModeEnum.PlusInt);
            Memory.Place(x, 4, TypeEnum.FixedPointer, Memory.GetTile(11, 0));
            Memory.Place(x, 5, TypeEnum.Int, 1);
            Memory.Place(x, 6, TypeEnum.FixedPointer, Memory.GetTile(11, 0));

            x += 2;
            Memory.Place(x, 3, TypeEnum.Instruction, ModeEnum.If);
            Memory.Place(x, 4, TypeEnum.RelativePointer, Memory.GetTile(x, 8) - Memory.GetTile(x, 4));
            Memory.Place(x, 5, TypeEnum.Int, 1);
            Memory.Place(x, 6, TypeEnum.RelativePointer, Memory.GetTile(xBegin + 1, 3) - Memory.GetTile(x, 6));

            Memory.Place(x, 8, TypeEnum.Instruction, ModeEnum.SuperiorOrEqualInt);
            Memory.Place(x, 9, TypeEnum.Int, 5);
            Memory.Place(x, 10, TypeEnum.FixedPointer, Memory.GetTile(11, 0));
            Memory.Place(x, 11, TypeEnum.Int, -1);


            x += 2;
            Memory.Place(x, 3, TypeEnum.Instruction, ModeEnum.StopCpu);

            Memory.AddCpu(0, 0);
            Memory.PrintPretty();

            while (Memory.Cpu.Count != 0)
            {
                Console.ReadLine();
                
                Memory.ExecuteStep();
                Memory.PrintPretty();
                //Memory.PrintBase10();
                Memory.ExtraConsole.WriteToConsole();
            }

            /*
            Memory.Execute();
            Memory.PrintPretty();
            Memory.ExtraConsole.WriteToConsole();
            */
        }

        public static void TestLoopAndStop4()
        {
            // SS to do:
            // ++ and -- for int
            var Memory = new Memory();

            Memory.Reset(18, 12);

            int programX = 0;
            int programY = 3;


            Memory.Place(0, 0, TypeEnum.FixedPointer, Memory.GetTile(programX, programY));
            Memory.Place(2, 0, TypeEnum.Int, 2);
            Memory.Place(4, 0, TypeEnum.Int, 0);

            int LoopX = 8;
            int LoopY = 0;
            Memory.Place(LoopX, LoopY, TypeEnum.Int, 0);

            Memory.Place(0, 3, TypeEnum.Instruction, (int)ModeEnum.WriteConsole);
            Memory.Place(0, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(0, 5, TypeEnum.Int, 0);
            Memory.Place(0, 6, TypeEnum.Int, 1);

            int xBegin = 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, 17);
            Memory.Place(xBegin, 5, TypeEnum.FixedPointer, Memory.GetTile(xBegin, 8));
            Memory.Place(xBegin, 6, TypeEnum.Int, 0);

            Memory.Place(xBegin, 8, TypeEnum.Instruction, (int)ModeEnum.MultiplyInt);
            Memory.Place(xBegin, 9, TypeEnum.Int, 2002);
            Memory.Place(xBegin, 10, TypeEnum.Int, 9);
            Memory.Place(xBegin, 11, TypeEnum.Int, 0);

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(xBegin, 5, TypeEnum.Int, 1);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.SuperiorOrEqualInt);
            Memory.Place(xBegin, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(xBegin, 5, TypeEnum.Int, 3);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 3, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.MultiplyInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);
            Memory.Place(xBegin, 5, TypeEnum.Int, 16);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 3, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);
            Memory.Place(xBegin, 5, TypeEnum.Int, Memory.GetTile(programX, programY));
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 5, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.FixedPointer, Memory.GetTile(0, 3));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.Goto);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);

            Memory.Place(16, 3, TypeEnum.Instruction, (int)ModeEnum.StopCpu);

            Memory.AddCpu(0, 0);
            Memory.PrintPretty();

            
            while (Memory.Cpu.Count != 0)
            {
                Console.ReadLine();
                Memory.ExecuteStep();
                Memory.PrintPretty();
                //Memory.PrintBase10();
                Memory.ExtraConsole.WriteToConsole();
            }
            
            /*
            Memory.Execute();
            Memory.PrintPretty();
            Memory.ExtraConsole.WriteToConsole();
            */

        }

        public static void TestLoopAndStop3()
        {
            // SS to do:
            // ++ and -- for int
            var Memory = new Memory();

            Memory.Reset(18, 12);

            int programX = 0;
            int programY = 3;


            Memory.Place(0, 0, TypeEnum.FixedPointer, Memory.GetTile(programX, programY));
            Memory.Place(2, 0, TypeEnum.Int, 2);
            Memory.Place(4, 0, TypeEnum.Int, 0);

            int LoopX = 8;
            int LoopY = 0;
            Memory.Place(LoopX, LoopY, TypeEnum.Int, 0);

            Memory.Place(0, 3, TypeEnum.Instruction, (int)ModeEnum.WriteConsole);
            Memory.Place(0, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(0, 5, TypeEnum.Int, 0);
            Memory.Place(0, 6, TypeEnum.Int, 1);

            int xBegin = 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, 17);
            Memory.Place(xBegin, 5, TypeEnum.FixedPointer, Memory.GetTile(xBegin, 8));
            Memory.Place(xBegin, 6, TypeEnum.Int, 0);

            Memory.Place(xBegin, 8, TypeEnum.Instruction, (int)ModeEnum.MultiplyInt);
            Memory.Place(xBegin, 9, TypeEnum.Int, 2002);
            Memory.Place(xBegin, 10, TypeEnum.Int, 9);
            Memory.Place(xBegin, 11, TypeEnum.Int, 0);

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(xBegin, 5, TypeEnum.Int, 1);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.SuperiorOrEqualInt);
            Memory.Place(xBegin, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(xBegin, 5, TypeEnum.Int, 3);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 3, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.MultiplyInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);
            Memory.Place(xBegin, 5, TypeEnum.Int, 16);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 3, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);
            Memory.Place(xBegin, 5, TypeEnum.Int, Memory.GetTile(programX, programY));
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 5, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.SetConsoleColor);
            Memory.Place(xBegin, 4, TypeEnum.Int, 11);
            Memory.Place(xBegin, 5, TypeEnum.Int, -1);

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.Goto);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);

            Memory.Place(16, 3, TypeEnum.Instruction, (int)ModeEnum.StopCpu);

            Memory.AddCpu(0, 0);
            Memory.PrintPretty();

            while (Memory.Cpu.Count != 0)
            {
                Console.ReadLine();
                Memory.ExecuteStep();
                Memory.PrintPretty();
                Memory.ExtraConsole.WriteToConsole();
            }
        }


        public static void TestLoopAndStop2()
        {
            // SS to do:
            // ++ and -- for int
            var Memory = new Memory();

            Memory.Reset(18, 9);

            int programX = 0;
            int programY = 3;


            Memory.Place(0, 0, TypeEnum.FixedPointer, Memory.GetTile(programX, programY));
            Memory.Place(2, 0, TypeEnum.Int, 2);
            Memory.Place(4, 0, TypeEnum.Int, 0);

            int LoopX = 8;
            int LoopY = 0;
            Memory.Place(LoopX, LoopY, TypeEnum.Int, 0);

            Memory.Place(0, 3, TypeEnum.Instruction, (int)ModeEnum.WriteConsole);
            Memory.Place(0, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(0, 5, TypeEnum.Int, 0);
            Memory.Place(0, 6, TypeEnum.Int, 1);

            int xBegin = 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, 17);
            Memory.Place(xBegin, 5, TypeEnum.Int, 9);
            Memory.Place(xBegin, 6, TypeEnum.Int, 0);

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(xBegin, 5, TypeEnum.Int, 1);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.SuperiorOrEqualInt);
            Memory.Place(xBegin, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(xBegin, 5, TypeEnum.Int, 3);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 3, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.MultiplyInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);
            Memory.Place(xBegin, 5, TypeEnum.Int, 16);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 3, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);
            Memory.Place(xBegin, 5, TypeEnum.Int, Memory.GetTile(programX, programY));
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 3, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.Goto);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);

            Memory.Place(16, 3, TypeEnum.Instruction, (int)ModeEnum.StopCpu);

            Memory.AddCpu(0, 0);
            Memory.PrintPretty();

            while (Memory.Cpu.Count != 0)
            {
                Console.ReadLine();
                Memory.ExecuteStep();
                Memory.PrintPretty();
                Memory.ExtraConsole.WriteToConsole();
            }

        }

        public static void TestLoopAndStop1()
        {
            // SS to do:
            // ++ and -- for int
            var Memory = new Memory();

            Memory.Reset(18, 20);

            int programX = 0;
            int programY = 3;

            Memory.Place(0, 0, TypeEnum.FixedPointer, Memory.GetTile(programX, programY));
            Memory.Place(2, 0, TypeEnum.Int, 2);
            Memory.Place(4, 0, TypeEnum.Int, 0);

            int LoopX = 8;
            int LoopY = 0;
            Memory.Place(LoopX, LoopY, TypeEnum.Int, 0);

            int PointerStoreX = 12;
            int PointerStoreY = 0;
            Memory.Place(PointerStoreX, PointerStoreY, TypeEnum.FixedPointer, Memory.GetTile(0, 8));

            Memory.Place(0, 3, TypeEnum.Instruction, (int)ModeEnum.Copy);
            Memory.Place(0, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(0, 5, TypeEnum.FixedPointer, Memory.GetTile(1, 8));


            int xBegin = 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, 17);
            Memory.Place(xBegin, 5, TypeEnum.Int, 9);
            Memory.Place(xBegin, 6, TypeEnum.Int, 0);

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(xBegin, 5, TypeEnum.Int, 1);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.SuperiorOrEqualInt);
            Memory.Place(xBegin, 4, TypeEnum.FixedPointer, Memory.GetTile(LoopX + 1, LoopY));
            Memory.Place(xBegin, 5, TypeEnum.Int, 3);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 3, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.MultiplyInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);
            Memory.Place(xBegin, 5, TypeEnum.Int, 16);
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 3, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.PlusInt);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);
            Memory.Place(xBegin, 5, TypeEnum.Int, Memory.GetTile(programX, programY));
            Memory.Place(xBegin, 6, TypeEnum.FixedPointer, Memory.GetTile(xBegin + 3, 4));

            xBegin += 2;
            Memory.Place(xBegin, 3, TypeEnum.Instruction, (int)ModeEnum.Goto);
            Memory.Place(xBegin, 4, TypeEnum.Int, -1);

            Memory.Place(16, 3, TypeEnum.Instruction, (int)ModeEnum.StopCpu);


            Memory.Place(6, 10, TypeEnum.Letter, '=');
            Memory.Place(8, 10, TypeEnum.Letter, ')');
            Memory.PlaceChars(0, 11, "Hello World !");

            Memory.PlaceChars(0, 15, "Made by Thomas / Mewily !\0");





            Memory.AddCpu(0, 0);
            Memory.PrintPretty();

            while (Memory.Cpu.Count != 0)
            {
                Console.ReadLine();
                Memory.ExecuteStep();
                Memory.PrintPretty();
            }

        }


        public static void Add17and9() 
        {
            var Memory = new Memory();
            Memory.Reset(12, 20);


            Memory.Place(4, 5, TypeEnum.Instruction, (int)ModeEnum.PlusInt + 2 /*up*/);
            Memory.Place(4, 4, TypeEnum.Int, 17);
            Memory.Place(4, 3, TypeEnum.Int, 9);
            Memory.Place(4, 2, TypeEnum.FixedPointer, Memory.GetTile(7, 9));

            Memory.Place(6, 9, TypeEnum.Int);

            Memory.Place(6, 5, TypeEnum.Instruction, ModeEnum.StopCpu);

            Memory.Place(0, 0, TypeEnum.FixedPointer, Memory.GetTile(2, 5));
            Memory.Place(2, 0, TypeEnum.Int, 2);
            Memory.Place(4, 0, TypeEnum.Int, 0);

            Memory.AddCpu(0, 0);
            Memory.PrintPretty();

            Memory.Execute();
            Memory.PrintPretty();
        }

        public static void Test3()
        {
            var Memory = new Memory();
            Memory.Reset(10, 11);

            Memory.Place(4, 5, TypeEnum.Instruction, (int)ModeEnum.PlusInt + 2 /*up*/);
            Memory.Place(4, 4, TypeEnum.Int, 17);
            Memory.Place(4, 3, TypeEnum.Int, 9);
            Memory.Place(4, 2, TypeEnum.Int, Memory.GetTile(7, 9));

            Memory.Place(6, 9, TypeEnum.Int);

            Memory.Place(6, 5, TypeEnum.Instruction, ModeEnum.StopCpu);

            Memory.Place(0, 0, TypeEnum.FixedPointer, Memory.GetTile(2, 5));
            Memory.Place(2, 0, TypeEnum.Int, 2);
            Memory.Place(4, 0, TypeEnum.Int, 0);

            Memory.AddCpu(0, 0);
            Memory.PrintPretty();

            //Memory.Execute();
            //Memory.PrintPretty();

            while(Memory.Cpu.Count != 0) 
            {
                Memory.ExecuteStep();
                Memory.PrintPretty();

            }


        }



        public static void Test2()
        {
            var Memory = new Memory();
            Memory.Reset(12, 20);


            Memory.Place(4, 4, TypeEnum.Instruction, ModeEnum.PlusInt);
            Memory.Place(4, 5, TypeEnum.Int, 17);
            Memory.Place(4, 6, TypeEnum.Int, 9);
            Memory.Place(4, 7, TypeEnum.Int, Memory.GetTile(5, 9));

            Memory.Place(4, 9, TypeEnum.Int);

            Memory.Place(6, 4, TypeEnum.Instruction, ModeEnum.Copy);
            Memory.Place(6, 5, TypeEnum.Int, 0);
            Memory.Place(6, 6, TypeEnum.CpuTileIdx, 1);

            Memory.Place(2, 0, TypeEnum.FixedPointer, Memory.GetTile(2, 4));
            Memory.Place(4, 0, TypeEnum.Int, 2);
            Memory.Place(6, 0, TypeEnum.Int, 0);

            Memory.AddCpu(2, 0);
            Memory.PrintPretty();

            Memory.Execute();
            Memory.PrintPretty();

            /*
            while (true) 
            {
                Console.WriteLine(">");
                Console.ReadLine();
            }
            */
        }
    }
}
