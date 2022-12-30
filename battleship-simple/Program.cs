// 2 hours, 3:32 to 5:22 + 0:18
// 

namespace battleship_simple
{
    internal class Program
    {
        static readonly GameBoard playerBoard = new GameBoard();
        static readonly GameBoard enemyBoard = new GameBoard();
        static readonly Random rng = new Random((int)DateTime.Now.ToBinary());
        static readonly List<int> enemyGuesses = new List<int>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                InitializeGame();

                PlayerSetBoard();

                while (true)
                {
                    // Draw
                    playerBoard.DrawGameBoard();
                    Console.WriteLine();
                    enemyBoard.DrawGameBoard();

                    // Player Guess
                    PlayerGuess();
                    bool enemyDefeated = enemyBoard.IsBoardDefeated();
                    if (enemyDefeated)
                    {
                        Console.WriteLine($"You win!");
                        Console.ReadLine();
                        break;
                    }

                    // Enemy guess
                    EnemyGuess();
                    bool playerDefeated = playerBoard.IsBoardDefeated();
                    if (playerDefeated)
                    {
                        Console.WriteLine($"You lose!");
                        Console.ReadLine();
                        break;
                    }

                    // Pause before clearing for next screen draw
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

            // Reset possible guesses
            enemyGuesses.Clear();
            int nGuesses = GameBoard.boardSize * GameBoard.boardSize;
            for (int i = 0; i < nGuesses; i++)
                enemyGuesses.Add(i);
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

        static bool PlayerGuess()
        {
            while (true)
            {
                ParsePlayerAttack(out string guess, out int row, out int col);
                bool success = enemyBoard.Guess(row, col, out bool hitShip);
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

        static void PlayerSetBoard()
        {
            while (true)
            {
                playerBoard.PrintPlaceableShips();
                Console.ReadLine();
            }
        }

        static bool EnemyGuess()
        {
            while (true)
            {
                int row = rng.Next(GameBoard.boardSize);
                int col = rng.Next(GameBoard.boardSize);
                bool success = playerBoard.Guess(row, col, out bool hitShip);
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

        static void EnemySetBoard()
        {

        }

        public class Ship
        {
            public string Name { get; set; } = string.Empty;
            public int Length { get; set; }
            public bool IsSunk { get; set; }
        }

        public class Cell
        {
            public bool HasGuessed { get; set; }
            public bool HasShip { get; set; }
            public string DisplayCharacter { get; set; } = "[]";
            public Ship? Ship { get; set; } = null;
        }

        public class GameBoard
        {
            public const int boardSize = 10;
            public readonly Cell[,] Cells = new Cell[boardSize, boardSize];
            public readonly List<Ship> PlaceableShips = new List<Ship>();
            public readonly Ship[] Ships = new Ship[]
            {
                new Ship(){ Name = "Carrier", Length = 5, },
                new Ship(){ Name = "Battleship", Length = 4, },
                new Ship(){ Name = "Battleship", Length = 4, },
                new Ship(){ Name = "Submarine", Length = 3, },
                new Ship(){ Name = "Destroyer", Length = 2, },
            };

            public bool IsPlayer { get; set; } = true;


            public Cell GetCell(int row, int col)
            {
                return Cells[row, col];
            }

            public void SetCell(int row, int col, string display, Ship ship)
            {
                Cell cell = Cells[row, col];
                cell.DisplayCharacter = display;
                cell.Ship = ship;
            }

            public void InitializeBoard()
            {
                for (int row = 0; row < boardSize; row++)
                    for (int col = 0; col < boardSize; col++)
                        Cells[row, col] = new Cell();

                PlaceableShips.Clear();
                PlaceableShips.AddRange(Ships);
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
                            Console.Write("[]");
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
                        else // no ship
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                        }
                    }
                    else // not guessed
                    {
                        if (cell.HasShip)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
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
                        else // no ship
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                        }
                    }
                    else // not guessed
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.BackgroundColor = ConsoleColor.Black;
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

                bool success = GetPlaceableShip(length, out Ship? ship);
                if (!success)
                    return false;

                int index = 0;
                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        string display = GetShipDisplay(isHorizontal, index, length);
                        SetCell(y, x, display, ship);
                        index++;
                    }
                }

                return true;
            }

            private bool GetPlaceableShip(int length, out Ship? ship)
            {
                foreach (var placeableShip in PlaceableShips)
                {
                    if (placeableShip.Length == length)
                    {
                        ship = placeableShip;
                        return true;
                    }
                }
                ship = null;
                return false;
            }

            public bool Guess(int col, int row, out bool hitShip)
            {
                Cell cell = GetCell(row, col);
                if (cell.HasGuessed)
                {
                    hitShip = false;
                    return false;
                }

                cell.HasGuessed = true;
                hitShip = cell.HasShip;
                return true;
            }

            public void PrintPlaceableShips()
            {
                foreach (var ship in PlaceableShips)
                {
                    Console.WriteLine($"Length:{ship.Length}, {ship.Name}");
                }
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