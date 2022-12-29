namespace mastermind
{
    internal class Program
    {
        const int turns = 10;
        static readonly char[] validCharacters = new char[] { 'r', 'y', 'g', 'c', 'b', 'm' };
        static readonly ConsoleColor[] characterColors = new ConsoleColor[] {
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.Cyan,
            ConsoleColor.Blue,
            ConsoleColor.Magenta,
        };

        static bool showAnswer = false;
        static int turnsRemaining = turns;
        static string answer = string.Empty;
        static readonly string[] guesses = new string[turns];

        static void Main(string[] args)
        {
            while (true)
            {
                InitGame();
                while (turnsRemaining > 0)
                {
                    PrintBoardState();
                    int currentGuess = turns - turnsRemaining;
                    string guess = AskPlayerForGuess();
                    guesses[currentGuess] = guess;

                    turnsRemaining--;
                    bool guessIsCorrect = guess == answer;
                    if (guessIsCorrect)
                    {
                        showAnswer = true;
                        PrintBoardState();
                        Console.WriteLine("You win!");
                        Console.WriteLine("Press any key to play again.");
                        showAnswer = false;
                        Console.ReadLine();
                        break;
                    }
                    else if (turnsRemaining == 0)
                    {
                        showAnswer = true;
                        PrintBoardState();
                        Console.WriteLine("You lose!");
                        Console.WriteLine("Press any key to play again.");
                        showAnswer = false;
                        Console.ReadLine();
                        break;
                    }
                }
            }
        }

        public static void InitGame()
        {
            turnsRemaining = turns;
            for (int i = 0; i < guesses.Length; i++)
                guesses[i] = string.Empty;
            answer = string.Empty;
            //showAnswer = false;

            var dt = DateTime.Now;
            Random random = new Random((int)dt.ToBinary());
            for (int i = 0; i < 4; i++)
            {
                int randomIndex = random.Next(validCharacters.Length);
                answer += validCharacters[randomIndex];
            }
        }
        public static string AskPlayerForGuess()
        {
            Console.WriteLine();
            Console.Write("Type your next guess. Use characters: ");
            ConsoleColor fgColor = Console.ForegroundColor;
            for (int i = 0; i < validCharacters.Length; i++)
            {
                Console.ForegroundColor = characterColors[i];
                Console.Write($"{validCharacters[i]}, ");
            }
            Console.WriteLine("\b\b."); // double backspace
            Console.ForegroundColor = fgColor;

            while (true)
            {
                var guess = Console.ReadLine();

                bool isInvalidText = string.IsNullOrEmpty(guess);
                if (isInvalidText)
                    continue;

                guess = guess.ToLower();

                bool isInvalidLength = guess.Length != 4;
                if (isInvalidLength)
                    continue;

                bool isValidGuess = true;
                for (int i = 0; i < guess.Length; i++)
                {
                    char g = guess[i];
                    bool isValidCharacter = false;
                    foreach (var validCharacter in validCharacters)
                    {
                        if (g == validCharacter)
                        {
                            isValidCharacter = true;
                            break;
                        }
                    }
                    if (!isValidCharacter)
                    {
                        isValidGuess = false;
                        break;
                    }
                }

                if (!isValidGuess)
                    continue;

                return guess;
            }
        }
        public static void PrintBoardState()
        {
            Console.Clear();
            Console.WriteLine("---------------");
            Console.WriteLine("| MASTER MIND |");
            Console.WriteLine("|-------------|");
            if (showAnswer)
            {
                PrintGuess(answer);
            }
            else
            {
                Console.WriteLine("|             |");
            }

            Console.WriteLine("|-------------|");

            for (int i = 0; i < turns - turnsRemaining; i++)
            {
                string guess = guesses[i];
                PrintGuess(guess);
                Console.WriteLine("|             |");
            }
            for (int i = 0; i < turnsRemaining; i++)
            {
                Console.WriteLine("|             |");
                Console.WriteLine("|             |");
            }

            Console.WriteLine("---------------");
        }
        public static void PrintGuess(string guess)
        {
            ConsoleColor fgColor = Console.ForegroundColor;
            ConsoleColor bgColor = Console.BackgroundColor;

            Console.Write("| ");
            foreach (var g in guess)
            {
                for (int vcIndex = 0; vcIndex < validCharacters.Length; vcIndex++)
                {
                    bool isValid = g == validCharacters[vcIndex];
                    if (isValid)
                    {
                        ConsoleColor color = characterColors[vcIndex];
                        Console.BackgroundColor = color;
                        Console.Write("  ");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                        break;
                    }
                }
            }
            Console.Write("| ");
            Console.Write($"Black:{GuessorrectRightPlace(guess)}, ");
            Console.Write($"White:{GuessCorrectWrongPlace(guess)}");
            Console.WriteLine();

            Console.ForegroundColor = fgColor;
            Console.BackgroundColor = bgColor;
        }

        public static int GuessorrectRightPlace(string guess)
        {
            int correctColor = 0;
            for (int x = 0; x < guess.Length; x++)
            {
                char g = guess[x];
                char a = answer[x];
                bool isCorrect = g == a;
                if (isCorrect)
                    correctColor++;
            }
            return correctColor;
        }
        public static int GuessCorrectWrongPlace(string guess)
        {
            int wrongColorButContains = 0;

            // Record which ones are correct
            bool[] isCorrect = new bool[guess.Length];
            for (int i = 0; i < guess.Length; i++)
            {
                char g = guess[i];
                char a = answer[i];
                isCorrect[i] = g == a;
            }
            // See which ones are INcorrect
            for (int currIndex = 0; currIndex < guess.Length; currIndex++)
            {
                if (isCorrect[currIndex])
                    continue;

                for (int altIndex = 0; altIndex < isCorrect.Length; altIndex++)
                {
                    // If correct at an index, skip it
                    if (isCorrect[altIndex])
                        continue;

                    // otherwise compare it
                    char a = answer[currIndex];
                    char g = guess[altIndex];
                    bool isCorrectSomewhere = a == g;
                    if (isCorrectSomewhere)
                    {
                        wrongColorButContains++;
                        break;
                    }
                }
            }
            return wrongColorButContains;
        }
    }
}