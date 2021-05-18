using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LowLevel.Game.CType;
using static LowLevel.Game.Mode;

namespace LowLevel.Game
{
    public class CPU 
    {
        public Memory Memory;

        /// <summary>
        /// Aka idx register beginning
        /// </summary>
        public int RegisterTile
        {
            get => Memory.GetTile(RegisterX, RegisterY);
            set 
            {
                RegisterX = Memory.GetTileX(value);
                RegisterY = Memory.GetTileY(value);
            }
        }
        public int RegisterX = -1;
        public int RegisterY = -1;

        public int this[int tile]
        {
            get => this[Memory.GetTileX(tile), Memory.GetTileY(tile)];
            set => this[Memory.GetTileX(tile), Memory.GetTileY(tile)] = value;
        }
        /// <summary>
        /// Register Value Relative
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int this[int x, int y]
        {
            get => Memory[RegisterX + x, RegisterY + y];
            set => Memory[RegisterX + x, RegisterY + y] = value;
        }

        #region Instruction
        public int InstructionTile
        {
            get => this[1, 0];
            set => this[1, 0] = value;
        }

        public int InstructionX => Memory.GetTileX(InstructionTile);
        public int InstructionY => Memory.GetTileY(InstructionTile);
        public void SetInstructionTile(int x, int y) => InstructionTile = Memory.GetTile(x, y);
        #endregion

        #region Move
        #region MoveX
        public int MoveX_Type_X => RegisterX + 2;
        public int MoveX_Type_Y => RegisterY + 0;
        public int MoveX_Type_Tile => Memory.GetTile(MoveX_Type_X, MoveX_Type_Y);
        public int MoveX_Type => Memory[MoveX_Type_X, MoveX_Type_Y];

        public int MoveX_Value_X => MoveX_Type_X + 1;
        public int MoveX_Value_Y => MoveX_Type_Y;
        public int MoveX_Value_Tile => Memory.GetTile(MoveX_Value_X, MoveX_Value_Y);
        public int MoveX_Value => Memory[MoveX_Value_X, MoveX_Value_Y];

        /// <summary>
        /// Following the pointer etc...
        /// </summary>
        public int MoveX_Final_Value => GetFinalValue(MoveX_Type_X, MoveX_Type_Y);
        #endregion

        #region MoveY
        public int MoveY_Type_X => RegisterX + 4;
        public int MoveY_Type_Y => RegisterY + 0;
        public int MoveY_Type_Tile => Memory.GetTile(MoveY_Type_X, MoveY_Type_Y);
        public int MoveY_Type => Memory[MoveY_Type_X, MoveY_Type_Y];

        public int MoveY_Value_X => MoveY_Type_X + 1;
        public int MoveY_Value_Y => MoveY_Type_Y;
        public int MoveY_Value_Tile => Memory.GetTile(MoveY_Value_X, MoveY_Value_Y);
        public int MoveY_Value => Memory[MoveY_Value_X, MoveY_Value_Y];

        /// <summary>
        /// Following the pointer etc...
        /// </summary>
        public int MoveY_Final_Value => GetFinalValue(MoveY_Type_X, MoveY_Type_Y);
        #endregion
        #endregion

        public int GetTypeTile(int tile) => tile - tile % 2;

        public int GetFinalValue(int xType, int yType) => GetFinalValue(Memory.GetTile(xType, yType));
        public int GetFinalValue(int tileType)
        {
            tileType = GetTypeTile(tileType);
            switch (Memory[tileType])
            {
                    //Pointer
                case (int)TypeEnum.FixedPointer:
                case (int)TypeEnum.DirectFixedPointer:
                case (int)TypeEnum.RelativePointer:
                case (int)TypeEnum.DirectRelativePointer:
                case (int)TypeEnum.CpuPointer:
                case (int)TypeEnum.DirectCpuPointer:
                    //return Memory[GetFinalPointer(tileType)];
                    //TODO TEST
                    return GetFinalValue(GetFinalPointer(tileType));
                //Tile :
                case (int)TypeEnum.TileIdx:
                    return tileType + Memory[tileType + 1];
                case (int)TypeEnum.CpuTileIdx:
                    return RegisterTile + Memory[tileType + 1];
                    //return GetFinalPointer(tileType);
                default:
                    return Memory[tileType + 1];
            }
        }

        /// <summary>
        /// Can only point to a type
        /// Can't point to a type, 
        /// </summary>
        /// <param name="xType"></param>
        /// <param name="yType"></param>
        /// <returns></returns>
        public int GetCpuFinalTileType(int xType, int yType) => GetCpuFinalTileType(Memory.GetTile(xType, yType));
        public int GetCpuFinalTileType(int tileType)
        {
            tileType = GetTypeTile(tileType);
            switch (Memory[tileType])
            {
                //Pointer
                case (int)TypeEnum.FixedPointer:
                case (int)TypeEnum.RelativePointer:
                case (int)TypeEnum.CpuPointer:
                //Tile :
                case (int)TypeEnum.TileIdx:
                case (int)TypeEnum.CpuTileIdx:
                    return Memory[GetFinalPointer(tileType)];
                default:
                    return Memory[tileType];
            }
        }


        /// <summary>
        /// Return the final pointed tile
        /// </summary>
        /// <returns></returns>
        public int GetFinalPointer(int xType, int yType) => GetFinalPointer(Memory.GetTile(xType, yType));
        public int GetFinalPointer(int tile)
        {
            int tileType = GetTypeTile(tile);
            return Memory[tileType] switch
            {
                //Pointer
                (int)TypeEnum.FixedPointer => GetFinalPointer(Memory[tileType + 1]),
                (int)TypeEnum.DirectFixedPointer => Memory[tileType + 1],
                (int)TypeEnum.RelativePointer => GetFinalPointer(tileType + Memory[tileType + 1]),
                (int)TypeEnum.DirectRelativePointer => tileType + Memory[tileType + 1],
                (int)TypeEnum.CpuPointer => GetFinalValue(this[Memory[tileType + 1]]),
                (int)TypeEnum.DirectCpuPointer => this[Memory[tileType + 1]],
                //Tile :
                (int)TypeEnum.TileIdx => tileType + Memory[tileType + 1] /* 0 in general */,
                (int)TypeEnum.CpuTileIdx => RegisterTile + Memory[tileType + 1],
                //Instruction :
                (int)TypeEnum.Instruction => GetSubInstructionResult(tile), //GetFinalPointer(GetSubInstructionResult(tile)),
                //Unknow
                _ => tile
            };
        }

        /// <summary>
        /// Execute the instruction, and return the result tile value
        /// </summary>
        public int GetSubInstructionResult(int tileType)
        {
            ExecuteInstructionStep(tileType, false);
            int instructionValue = GetFinalValue(tileType);
            int Result_Final_Pointer = GetFinalPointer(Memory.GetTileX(tileType) + 6 * GetDirectionX(instructionValue) + 1, Memory.GetTileY(tileType) + 3 * GetDirectionY(instructionValue));
            return Result_Final_Pointer;
        }

        public CPU(Memory memory, int registerTileIdx) 
        {
            Memory = memory;
            RegisterTile = registerTileIdx;
        }

        public void Execute() { while (ExecuteStep()) ; }
        public bool ExecuteStep() 
        {
            if (RegisterTile == InstructionTile) //Overflow. Stop the thread 
            {
                Unload();
                return false;
            }
            ExecuteStep(InstructionTile);
            return true;
        }


        public void Unload() { Memory.Cpu.Remove(this); }

        public void ExecuteStep(int tileType) => ExecuteStep(Memory.GetTileX(tileType), Memory.GetTileY(tileType));
        public void ExecuteStep(int instructionTypeX, int instructionTypeY) 
        {
            if (GetCpuFinalTileType(instructionTypeX, instructionTypeY) == (int)TypeEnum.Instruction)
            {
                ExecuteInstructionStep(instructionTypeX, instructionTypeY);
            }
            else
            {
                MoveCpu();
            }
        }

        public void ExecuteInstructionStep(int tileType, bool moveCpu = true) => ExecuteInstructionStep(Memory.GetTileX(tileType), Memory.GetTileY(tileType), moveCpu);
        public void ExecuteInstructionStep(int instructionTypeX, int instructionTypeY, bool moveCpu = true) 
        {
            int instructionValue = GetFinalValue(instructionTypeX, instructionTypeY);
            int instructionDirectionX = GetDirectionX(instructionValue);
            int instructionDirectionY = GetDirectionY(instructionValue);

#pragma warning disable CS8321 // La fonction locale est déclarée mais jamais utilisée

            // Value return the final value of the tile
            // Pointer return the final tile idx of the value tile

            int A_Final_Value() => GetFinalValue(instructionTypeX + 2 * instructionDirectionX, instructionTypeY + instructionDirectionY);
            int A_Final_Pointer() => GetFinalPointer(instructionTypeX + 2 * instructionDirectionX + 1, instructionTypeY + instructionDirectionY);

            int B_Final_Value() => GetFinalValue(instructionTypeX + 4 * instructionDirectionX, instructionTypeY + 2 * instructionDirectionY);
            int B_Final_Pointer() => GetFinalPointer(instructionTypeX + 4 * instructionDirectionX + 1, instructionTypeY + 2 * instructionDirectionY);

            int Result_Final_Value() => GetFinalValue(instructionTypeX + 6 * instructionDirectionX, instructionTypeY + 3 * instructionDirectionY);
            int Result_Final_Pointer() =>   GetFinalPointer(instructionTypeX + 6 * instructionDirectionX + 1, instructionTypeY + 3 * instructionDirectionY);
#pragma warning restore CS8321

            switch (GetWithoutDirection(instructionValue))
            {
                case (int)ModeEnum.Copy:
                    Memory[B_Final_Pointer()] = A_Final_Value();
                    break;
                case (int)ModeEnum.Goto:
                    InstructionTile = A_Final_Value()+ B_Final_Value() /* generally 0. Can be used for relative pointer */;
                    return; //To force the cpu to not move just after the goto (see the end of this function)
                case (int)ModeEnum.If:
                    {
                        if(A_Final_Value() == B_Final_Value() /* generally 0. Can be used for relative pointer */)
                        {
                            InstructionTile = Result_Final_Value();
                            //To force the cpu to not move just after the goto (see the end of this function)
                            return;
                        }
                    }
                    break;
                case (int)ModeEnum.Swap:
                    {
                        int aValue = A_Final_Value();
                        int bValue = B_Final_Value();
                        int a = Memory[aValue];
                        Memory[aValue] = Memory[bValue];
                        Memory[bValue] = a;
                    }
                    break;
                case (int)ModeEnum.CpuDirection:
                    {
                        int speed = Math.Abs(MoveX_Final_Value) + Math.Abs(MoveY_Final_Value);
                        int aValue = A_Final_Value();
                        if (aValue < 0 || aValue >= 4) { throw new Exception("Rotation value (" + aValue + ") must be in [0;3]"); }
                        Memory[RegisterX] = speed * GetDirectionX(aValue);
                        Memory[RegisterY] = speed * GetDirectionY(aValue);
                    }
                    break;
                case (int)ModeEnum.StopCpu:
                    this[RegisterTile+1] = RegisterTile;
                    return;
                // Console :
                case (int)ModeEnum.WriteConsole:
                    if(Result_Final_Value() == 0) 
                    {
                        Memory.ExtraConsole.Write(B_Final_Value() != 0 ? Useful.ToBase10(A_Final_Value()) : Memory.GetStringRepresentation(A_Final_Pointer()));
                    }
                    else 
                    {
                        Memory.ExtraConsole.WriteLine(B_Final_Value() != 0 ? Useful.ToBase10(A_Final_Value()) : Memory.GetStringRepresentation(A_Final_Pointer()));
                    }
                    break;
                case (int)ModeEnum.ReadConsole:
                    {
                        //Todo

                        //int nb value (0 = unlimited)
                        //int value type
                        //int pointer where to store the value
                        int nbValue = A_Final_Value();
                        int valueType = B_Final_Value();
                        int finalPointer = Result_Final_Pointer();

                        switch (valueType)
                        {
                            case (int)TypeEnum.Letter:
                                {
                                    Memory[finalPointer] = Console.ReadKey().KeyChar;
                                }
                                break;
                            default:
                                {
                                    //Read a int
                                    int value = 0;
                                    while (int.TryParse(Console.ReadLine(), out value) == false) ;
                                    Memory[finalPointer] = value;
                                }
                                break;
                        }
                        //Memory.ExtraConsole
                    }
                    break;
                case (int)ModeEnum.SetConsoleColor:
                    {
                        int a = A_Final_Value();
                        if (a != -1) 
                        {
                            Memory.ExtraConsole.ColorForeground = (ConsoleColor)(a % 16);
                        }
                        int b = B_Final_Value();
                        if (b != -1)
                        {
                            Memory.ExtraConsole.ColorBackground = (ConsoleColor)(b % 16);
                        }
                    }
                    break;
                case (int)ModeEnum.ClearConsole:
                    {
                        int nbLineToRemove = A_Final_Value();
                        if (nbLineToRemove == 0) 
                        {
                            Memory.ExtraConsole.Clear();
                        }
                        else for (int i = 0; i < nbLineToRemove; i++) //Legit !
                        {
                            Memory.ExtraConsole.DeleteLastLine();
                        }
                    }
                    break;
                //Int :
                case (int)ModeEnum.PlusInt: Memory[Result_Final_Pointer()] = A_Final_Value() + B_Final_Value(); break;
                case (int)ModeEnum.MinusInt: Memory[Result_Final_Pointer()] = A_Final_Value() - B_Final_Value(); break;
                case (int)ModeEnum.MultiplyInt: Memory[Result_Final_Pointer()] = A_Final_Value() * B_Final_Value(); break;
                case (int)ModeEnum.DivideInt: Memory[Result_Final_Pointer()] = A_Final_Value() / B_Final_Value(); break;
                case (int)ModeEnum.ModuloInt: Memory[Result_Final_Pointer()] = A_Final_Value() % B_Final_Value(); break;
                case (int)ModeEnum.RShiftInt: Memory[Result_Final_Pointer()] = A_Final_Value() >> B_Final_Value(); break;
                case (int)ModeEnum.LShiftInt: Memory[Result_Final_Pointer()] = A_Final_Value() << B_Final_Value(); break;
                case (int)ModeEnum.AbsInt: Memory[Result_Final_Pointer()] = Math.Abs(A_Final_Value()); break;
                case (int)ModeEnum.PowInt: Memory[Result_Final_Pointer()] = (int)Math.Pow(A_Final_Value(), B_Final_Value()); break;
                case (int)ModeEnum.SqrtInt: Memory[Result_Final_Pointer()] = (int)Math.Pow(A_Final_Value(), 1d/B_Final_Value()); break;

                case (int)ModeEnum.EqualInt: Memory[Result_Final_Pointer()] = A_Final_Value() << B_Final_Value(); break;
                case (int)ModeEnum.DifferentInt: Memory[Result_Final_Pointer()] = (A_Final_Value() == B_Final_Value()) ? 1 : 0; break;
                case (int)ModeEnum.SuperiorInt: Memory[Result_Final_Pointer()] = (A_Final_Value() > B_Final_Value()) ? 1 : 0; break;
                case (int)ModeEnum.SuperiorOrEqualInt: Memory[Result_Final_Pointer()] = (A_Final_Value() >= B_Final_Value()) ? 1 : 0; break;
                case (int)ModeEnum.InferiorInt: Memory[Result_Final_Pointer()] = (A_Final_Value() < B_Final_Value()) ? 1 : 0; break;
                case (int)ModeEnum.InferiorOrEqualInt: Memory[Result_Final_Pointer()] = (A_Final_Value() <= B_Final_Value()) ? 1 : 0; break;

                //Bool :
                case (int)ModeEnum.Lazy_OR: Memory[Result_Final_Pointer()] = (A_Final_Value() != 0 || B_Final_Value() != 0) ? 1 : 0; break;
                case (int)ModeEnum.OR: Memory[Result_Final_Pointer()] = (A_Final_Value() != 0 | B_Final_Value() != 0) ? 1 : 0; break;
                case (int)ModeEnum.Lazy_AND: Memory[Result_Final_Pointer()] = (A_Final_Value() != 0 && B_Final_Value() != 0) ? 1 : 0; break;
                case (int)ModeEnum.AND: Memory[Result_Final_Pointer()] = (A_Final_Value() != 0 & B_Final_Value() != 0) ? 1 : 0; break;
                case (int)ModeEnum.NOT: Memory[Result_Final_Pointer()] = (A_Final_Value() == 0) ? 1 : 0; break;
                default:
                    throw new Exception("Unknow instruction value " + instructionValue + " at tile (b10) " + InstructionTile);
            }

            if (moveCpu) 
            {
                MoveCpu();
            }
            return;
        }

        //Move the cpu to the next tile
        public void MoveCpu() => SetInstructionTile(InstructionX + MoveX_Final_Value, InstructionY + MoveY_Final_Value);
    }
}
