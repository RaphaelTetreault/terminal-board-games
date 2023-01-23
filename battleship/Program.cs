// Coding time estimate: 4h15m

namespace battleship
{
    internal class Program
    {
        const bool debug_seeOpponentShips = false;
        const int boardWidth = 10;
        const int boardHeight = 10;
        static int playerShipsSunk = 0;
        static int opponentShipsSunk = 0;
        static readonly Cell[,] playerBoard = new Cell[boardWidth, boardHeight];
        static readonly Cell[,] opponentBoard = new Cell[boardWidth, boardHeight];
        static readonly Piece[] pieces = new Piece[]
        {
            new Piece(){ id = 5, name = "Carrier", shipSize = 5 },
            new Piece(){ id = 3, name = "Battleship", shipSize = 4 },
            new Piece(){ id = 4, name = "Battleship", shipSize = 4 },
            new Piece(){ id = 2, name = "Submarine", shipSize = 3 },
            new Piece(){ id = 1, name = "Destroyer", shipSize = 2 },
        };
        static readonly List<int> aiAttackes = new List<int>();
        static readonly Random rng = new Random((int)DateTime.Now.ToBinary());

        static void Main(string[] args)
        {
            while (true)
            {
                InitializeGame();
                DrawGameBoard();
                AskPlayerSetBoard();

                while (true)
                {
                    DrawGameBoard();
                    AskPlayerAttack();
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();

                    bool opponentDefeated = IsDefeated(opponentBoard);
                    if (opponentDefeated)
                    {
                        DrawGameBoard();
                        Console.WriteLine();
                        Console.WriteLine("You win!");
                        Console.WriteLine("Press any key to play again.");
                        Console.ReadLine();
                        break;
                    }

                    DrawGameBoard();
                    AIAttack();
                    Console.WriteLine("Press enter to continue.");
                    Console.ReadLine();

                    //
                    bool playerDefeated = IsDefeated(playerBoard);
                    if (playerDefeated)
                    {
                        DrawGameBoard();
                        Console.WriteLine();
                        Console.WriteLine("You lose!");
                        Console.WriteLine("Press any key to play again.");
                        Console.ReadLine();
                        break;
                    }


                }
            }
        }

        static void InitializeGame()
        {
            // Clear game boards
            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    playerBoard[y, x] = new Cell();
                    opponentBoard[y, x] = new Cell();
                }
            }

            // Get AI board set up
            AIInitBoard();

            // Reset the list of guesses for AI
            aiAttackes.Clear();
            for (int x = 0; x < boardWidth; x++)
                for (int y = 0; y < boardHeight; y++)
                    aiAttackes.Add(x + y * boardHeight);

            //
            playerShipsSunk = 0;
            opponentShipsSunk = 0;
        }

        static void DrawSunkPeg(bool shipIsSunk)
        {
            if (shipIsSunk)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write("O");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(" ");
            }
        }

        static void DrawGameBoard()
        {
            Console.Clear();

            // Draw letters on top row
            Console.WriteLine("          YOU                      ENEMY        ");

            // Draw sunk ship pegs (when ships are sunk
            Console.Write("        [");
            for (int i = 0; i < pieces.Length; i++)
            {
                bool shipIsSunk = i < playerShipsSunk;
                DrawSunkPeg(shipIsSunk);
            }
            Console.ResetColor();
            Console.Write("]                   [");
            for (int i = 0; i < pieces.Length; i++)
            {
                bool shipIsSunk = i < opponentShipsSunk;
                DrawSunkPeg(shipIsSunk);
            }
            Console.WriteLine("]");
            Console.ResetColor();

            for (int i = 0; i < 2; i++)
            {
                Console.Write($"  ");
                for (int x = 0; x < boardWidth; x++)
                {
                    char column = (char)(x + 97);
                    Console.Write($"{column} ");
                }
                Console.Write($"    ");
            }
            Console.WriteLine();

            for (int y = 0; y < boardHeight; y++)
            {
                Console.Write($"{y} ");
                for (int x = 0; x < boardWidth; x++)
                {
                    var cell = playerBoard[y, x];
                    DrawPlayerBoardCell(cell);
                }
                Console.ResetColor();
                Console.Write($"    {y} ");
                for (int x = 0; x < boardWidth; x++)
                {
                    var cell = opponentBoard[y, x];
                    DrawOpponentBoardCell(cell);
                }
                Console.WriteLine();
                Console.ResetColor();
            }
            Console.WriteLine();
        }
        static void SetBoardColor(CellState state)
        {
            switch (state)
            {
                case CellState.no_guess:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case CellState.guess_wrong:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    break;
                case CellState.guess_right:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
            }
        }
        static string GetBoardCharacter(CellState state)
        {
            string character = state switch
            {
                CellState.no_guess => "[]",
                CellState.guess_wrong => "()",
                CellState.guess_right => "()",
                _ => "Er", // error
            };
            return character;
        }
        static void DrawOpponentBoardCell(Cell cell)
        {
            SetBoardColor(cell.guessState);
            string text = GetBoardCharacter(cell.guessState);
            // Debug enemy ships
            if (debug_seeOpponentShips && cell.isOccupied)
                text = "qb";
            Console.Write(text);
        }
        static void DrawPlayerBoardCell(Cell playerCell)
        {
            // Draw player piece slightly lighter
            bool highlightPlayerPiece = (playerCell.guessState == CellState.no_guess) && (playerCell.piece.id > 0);
            if (highlightPlayerPiece)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkGray;
            }
            else
            {
                SetBoardColor(playerCell.guessState);
            }

            Console.Write(playerCell.text);
        }

        static (int x, int y) ParseCellCall(string messagePrompt, out string guess)
        {
            while (true)
            {
                Console.WriteLine(messagePrompt);
                string input = Console.ReadLine();
                bool isInvalid = string.IsNullOrEmpty(input) || input.Length != 2;
                if (isInvalid)
                {
                    WriteLineTryAgain("Could not parse input.");
                    DrawGameBoard();
                    continue;
                }
                input = input.ToLower();
                // parse letter
                char letter = input[0];
                int letterIndex = ((byte)letter) - 97;
                bool isValidLetter = letterIndex >= 0 && letterIndex < boardWidth;
                //Console.WriteLine($"{leadingLetter}: {letterIndex}");
                char number = input[1];
                bool parseNumberSuccess = int.TryParse($"{number}", out int numberIndex);
                bool isValidNumber = parseNumberSuccess && numberIndex >= 0 && numberIndex < boardHeight;

                bool isInvalidParse = !isValidLetter || !isValidNumber;
                if (isInvalidParse)
                {
                    WriteLineTryAgain("Input is invald. Try again.");
                    DrawGameBoard();
                    continue;
                }
                //Console.WriteLine($"DEBUG ({letterIndex},{numberIndex})");
                Console.WriteLine();
                guess = input;
                return (letterIndex, numberIndex);
            }
        }
        static void AskPlayerSetBoard()
        {
            var pieces = new List<Piece>(global::battleship.Program.pieces);
            //var pieces = new List<Piece>();
            while (pieces.Count > 0)
            {
                DrawGameBoard();

                Console.WriteLine($"Pieces you can place: ");
                foreach (var piece in pieces)
                    Console.WriteLine($"Length {piece.shipSize}, {piece.name}");

                (int x0, int y0) = ParseCellCall("Place piece starting cell (ex: a0).", out _);
                (int x1, int y1) = ParseCellCall("Place piece ending cell (ex: a4).", out _);
                bool isInSameRow = y0 == y1;
                bool isInSameCol = x0 == x1;
                bool isInSameCell = isInSameRow && isInSameCol;
                bool isDiagonal = !isInSameRow && !isInSameCol;
                bool isValid = isInSameRow ^ isInSameCol;
                if (!isValid)
                {
                    if (isInSameCell)
                    {
                        WriteLineTryAgain("Selected cells overlap.");
                    }
                    else if (isDiagonal)
                    {
                        WriteLineTryAgain("Selected cells create invalid diagonal.");
                    }
                    else
                    {
                        WriteLineTryAgain("Undefined error.");
                    }
                    continue;
                }
                else
                {
                    int shipSize = isInSameRow ? Math.Abs(x0 - x1) : Math.Abs(y0 - y1);
                    shipSize += 1;

                    bool canPlace = false;
                    Piece placedPiece = new();
                    foreach (var piece in pieces)
                    {
                        if (piece.shipSize == shipSize)
                        {
                            canPlace = true;
                            placedPiece = piece;
                        }
                    }

                    if (canPlace)
                    {
                        int posX = isInSameRow ? Math.Min(x0, x1) : x0;
                        int posY = isInSameCol ? Math.Min(y0, y1) : y0;

                        if (!isInSameRow && !isInSameCol)
                            throw new NotImplementedException("Undefined success.");

                        bool success = PlacePiece(playerBoard, placedPiece, posX, posY, isInSameRow);
                        if (!success)
                        {
                            WriteLineTryAgain("Cannot place piece overtop another!");
                            continue;
                        }
                        // 
                        pieces.Remove(placedPiece);
                    }
                    else
                    {
                        WriteLineTryAgain($"Can't place a ship of length {shipSize}.");
                        continue;
                    }
                }
            }
        }
        static void AskPlayerAttack()
        {
            while (true)
            {
                (int x, int y) = ParseCellCall("Attack which cell?", out string guess);
                var cell = opponentBoard[y, x];
                bool canGuessThatCell = cell.guessState == CellState.no_guess;
                if (!canGuessThatCell)
                {
                    WriteLineTryAgain($"You already guessed this cell ({guess}).");
                    DrawGameBoard();
                    continue;
                }

                Console.Write($"You called {guess}. ");

                if (cell.isOccupied)
                {
                    cell.guessState = CellState.guess_right;
                    Console.WriteLine($"It's a hit!");
                    bool defeatedShip = HasDefeatedID(opponentBoard, cell);
                    if (defeatedShip)
                    {
                        opponentShipsSunk++;
                        Console.WriteLine($"{cell.piece.name} sunk!");
                    }
                }
                else
                {
                    cell.guessState = CellState.guess_wrong;
                    Console.WriteLine($"It's a miss!");
                }
                break;
            }
        }

        static void WriteLineTryAgain(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Press enter and try again.");
            Console.ReadLine();
        }

        static bool IsDefeated(Cell[,] board)
        {
            foreach (var cell in board)
            {
                if (cell.isOccupied)
                    if (cell.guessState == CellState.no_guess)
                        return false;
            }
            return true;
        }
        static bool HasDefeatedID(Cell[,] board, Cell hitCell)
        {
            int hits = 0;
            foreach (var cell in board)
            {
                if (cell.piece.id == hitCell.piece.id)
                    if (cell.guessState == CellState.guess_right)
                        hits++;
            }

            if (hits == hitCell.piece.shipSize)
            {
                return true;
            }
            else if (hits > hitCell.piece.shipSize)
            {
                throw new ArgumentException("Invalid board state");
            }
            else
            {
                return false;
            }
        }

        static void AIInitBoard()
        {
            for (int i = 0; i < pieces.Length; i++)
            {
                var piece = pieces[i];
                // coin flip
                bool isHorizontal = rng.Next(2) > 0;
                int maxX = isHorizontal ? boardWidth - piece.shipSize : boardWidth;
                int maxY = isHorizontal ? boardHeight : boardHeight - piece.shipSize + 1;

                // Keep trying to place pieces until it fits somewhere.
                // Not great, but works well enough for my needs.
                while (true)
                {
                    int posX = rng.Next(maxX);
                    int posY = rng.Next(maxY);
                    bool success = PlacePiece(opponentBoard, piece, posX, posY, isHorizontal);
                    if (success)
                        break;
                }
            }
        }
        static void AIAttack()
        {
            int index = rng.Next(aiAttackes.Count);
            int cellIndex = aiAttackes[index];
            aiAttackes.RemoveAt(index);
            int x = cellIndex % boardWidth;
            int y = cellIndex / boardHeight;
            var cell = playerBoard[y, x];

            char character = (char)(x + 97);
            Console.Write($"Enemy called {character}{y}. ");

            // Code below could be modular between players
            bool canGuessThatCell = cell.guessState == CellState.no_guess;
            if (!canGuessThatCell)
                throw new ArgumentException();

            if (cell.isOccupied)
            {
                cell.guessState = CellState.guess_right;
                Console.WriteLine($"It's a hit!");
                bool defeatedShip = HasDefeatedID(playerBoard, cell);
                if (defeatedShip)
                {
                    opponentShipsSunk++;
                    Console.WriteLine($"{cell.piece.name} sunk!");
                }
            }
            else
            {
                cell.guessState = CellState.guess_wrong;
                Console.WriteLine($"It's a miss!");
            }
        }

        static bool PlacePiece(Cell[,] board, Piece piece, int x, int y, bool isHorizontal)
        {
            if (isHorizontal)
            {
                int min = x;
                int max = min + piece.shipSize;

                // Can place in row?
                for (int ix = min; ix < max; ix++)
                    if (board[y, ix].isOccupied)
                        return false;

                for (int _x = min; _x < max; _x++)
                {
                    board[y, _x].text = "==";
                    board[y, _x].isOccupied = true;
                    board[y, _x].piece = piece;
                }
            }
            else
            {
                int min = y;
                int max = min + piece.shipSize;

                // Can place in column?
                for (int iy = min; iy < max; iy++)
                    if (board[iy, x].isOccupied)
                        return false;

                for (int _y = min; _y < max; _y++)
                {
                    board[_y, x].text = "||";
                    board[_y, x].isOccupied = true;
                    board[_y, x].piece = piece;
                }
            }
            return true;
        }

        public enum CellState
        {
            no_guess,
            guess_wrong,
            guess_right,
        }

        public class Cell
        {
            public CellState guessState;
            public string text = "[]";
            public bool isOccupied;
            public Piece piece;
        }

        public struct Piece
        {
            public int id;
            public int shipSize;
            public string name;
        }
    }
}