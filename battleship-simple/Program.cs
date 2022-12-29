namespace battleship_simple
{
    internal class Program
    {
        static readonly GameBoard playerBoard = new GameBoard();
        static readonly GameBoard enemyBoard = new GameBoard();
        static readonly Random rng = new Random((int)DateTime.Now.ToBinary());

        static void Main(string[] args)
        {
            while (true)
            {
                InitializeGame();
                while (true)
                {
                    playerBoard.DrawGameBoard();
                    Console.WriteLine();
                    enemyBoard.DrawGameBoard();
                    PlayerGuess();
                    bool enemyDefeated = enemyBoard.IsBoardDefeated();
                    Console.WriteLine($"Is enemey defeated? {enemyDefeated}");
                    EnemyGuess();
                    bool playerDefeated = playerBoard.IsBoardDefeated();
                    Console.WriteLine($"Is player defeated? {playerDefeated}");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        static void InitializeGame()
        {
            playerBoard.InitializeBoard();
            enemyBoard.InitializeBoard();
            enemyBoard.IsPlayer = false;

            playerBoard.SetShip(0, 0, 0, 4);
            enemyBoard.SetShip(0, 0, 0, 4);
        }

        static void ParsePlayerAttack(out string guess, out int x, out int y)
        {
            Console.WriteLine("Attack which cell? (ex: b4)");
            while (true)
            {
                string input = Console.ReadLine();
                if (input == null)
                    continue;
                if (input.Length != 2)
                    continue;
                input = input.ToLower();
                char letter = input[0];
                string number = input[1].ToString();
                int letterAsNumber = (letter - 97);
                bool success = int.TryParse(number, out int numberAsNumber);
                if (!success)
                    continue;
                // Assign out values
                x = letterAsNumber;
                y = numberAsNumber;
                guess = input;
                break;
            }
        }

        static bool Guess(int col, int row, GameBoard gameBoard, out bool hitShip)
        {
            Cell cell = gameBoard.GetCell(row, col);
            if (cell.HasGuessed)
            {
                hitShip = false;
                return false;
            }

            cell.HasGuessed = true;
            hitShip = cell.HasShip;
            return true;
        }

        static bool PlayerGuess()
        {
            while (true)
            {
                ParsePlayerAttack(out string guess, out int row, out int col);
                bool success = Guess(row, col, enemyBoard, out bool hitShip);
                if (!success)
                {
                    Console.WriteLine($"Already guessed {guess}. Guess another cell.");
                    continue;
                }

                if (hitShip)
                {
                    Console.WriteLine($"{guess} - It's a hit!");
                }
                else
                {
                    Console.WriteLine($"{guess} - It's a miss!");
                }
                return hitShip;
            }
        }

        static bool EnemyGuess()
        {
            while (true)
            {
                int row = rng.Next(GameBoard.boardSize);
                int col = rng.Next(GameBoard.boardSize);
                bool success = Guess(row, col, playerBoard, out bool hitShip);
                if (!success)
                    continue;

                char c = (char)(col + 97);
                if (hitShip)
                {
                    Console.WriteLine($"({c}{row}) - It's a hit!");
                }
                else
                {
                    Console.WriteLine($"({c}{row}) - It's a miss!");
                }
                return hitShip;
            }
        }



        public class Cell
        {
            public bool HasGuessed { get; set; }
            public bool HasShip { get; set; }
            public string DisplayCharacter { get; set; } = "  ";
        }

        public class GameBoard
        {
            public const int boardSize = 10;
            public readonly Cell[,] Cells = new Cell[boardSize, boardSize];

            public bool IsPlayer { get; set; } = true;


            public Cell GetCell(int row, int col)
            {
                return Cells[row, col];
            }

            public void SetCell(int row, int col, string display, bool hasShip = true)
            {
                Cell cell = Cells[row, col];
                cell.DisplayCharacter = display;
                cell.HasShip = hasShip;
            }

            public void InitializeBoard()
            {
                for (int row = 0; row < boardSize; row++)
                    for (int col = 0; col < boardSize; col++)
                        Cells[row, col] = new Cell();
            }

            public bool IsBoardDefeated()
            {
                foreach (var cell in Cells)
                    if (cell.HasShip && !cell.HasGuessed)
                        return false;
                return true;
            }

            public void DrawGameBoard()
            {
                Console.WriteLine("  a b c d e f g h i j");
                for (int row = 0; row < boardSize; row++)
                {
                    Console.Write(row);
                    Console.Write(" ");
                    for (int col = 0; col < boardSize; col++)
                    {
                        var cell = GetCell(row, col);
                        SetDrawColor(cell);

                        if (IsPlayer)
                            Console.Write(cell.DisplayCharacter);
                        else
                            Console.Write("  ");
                    }
                    Console.ResetColor();
                    Console.WriteLine();
                }
                Console.ResetColor();
            }

            public void SetDrawColor(Cell cell)
            {
                if (IsPlayer)
                {
                    if (cell.HasGuessed)
                    {
                        if (cell.HasShip)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                        }
                    }
                    else
                    {
                        if (cell.HasShip)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                    }
                }
                else // enemy
                {
                    if (cell.HasGuessed)
                    {
                        if (cell.HasShip)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                        }
                    }
                    else
                    {
                        Console.ResetColor();
                    }
                }
            }

            public bool SetShip(int x0, int y0, int x1, int y1)
            {
                bool isInSameCol = x0 == x1;
                bool isInSameRow = y0 == y1;
                bool isValid = isInSameCol ^ isInSameRow;
                if (!isValid)
                    return false;

                int minX = Math.Min(x0, x1);
                int maxX = Math.Max(x0, x1);
                int minY = Math.Min(y0, y1);
                int maxY = Math.Max(y0, y1);

                int lengthX = maxX - minX;
                int lengthY = maxY - minY;
                int length = lengthX + lengthY;
                bool isHorizontal = lengthY == 0;
                //bool isVertical = lengthX == 0;

                int index = 0;
                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        string display = GetShipDisplay(isHorizontal, index, length);
                        SetCell(y, x, display, true);
                        index++;
                    }
                }

                return true;
            }

            private string GetShipDisplay(bool isHorizontal, int index, int length)
            {
                if (isHorizontal)
                {
                    if (index == 0)
                        return "<";
                    else if (index == length)
                        return ">";
                    else
                        return "=";
                }
                else
                {
                    if (index == 0)
                        return @"/\";
                    else if (index == length)
                        return @"\/";
                    else
                        return @"||";
                }
            }
        }
    }
}