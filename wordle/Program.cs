// Teach:   continue and break keywords
//          ToUpper / ToLower
//          static / not static -> they will try to call func and it doesn't work (not static)
//          enums
//          lists

namespace wordle
{
    internal class Program
    {
        static readonly string[] validWords = LoadWordleWordListByFileName("valid-wordle-words.txt");
        static readonly string[] guesses = new string[6];
        static readonly Random random = new Random();
        static string answer = "";
        static int guessIndex = 0;

        static readonly GuessState[] usedLetters = new GuessState[26];

        static void Main(string[] args)
        {
            // Loop forever - console window does not self-terminate.
            while (true)
            {
                InitializeGame();
                DrawGameBoard();

                // Wordle guess loop
                while (true)
                {
                    var guess = GetPlayerGuess();
                    guesses[guessIndex] = guess;
                    guessIndex++;
                    UpdateUsedLetters();

                    DrawGameBoard();

                    bool isGuessCorrect = guess == answer;
                    if (isGuessCorrect)
                    {
                        Console.WriteLine("You win!");
                        Console.ReadLine();
                        break;
                    }

                    bool isGameOver = guessIndex == guesses.Length;
                    if (isGameOver)
                    {
                        Console.WriteLine($"Game over! The word was {answer}.");
                        Console.ReadLine();
                        break;
                    }
                }
            }
        }

        public static void InitializeGame()
        {
            // Reset number of guesses taken
            guessIndex = 0;

            // Pick new random word answer
            answer = PickRandomWord();

            // Reset guess to be blank (5 characters each).
            for (int i = 0; i < guesses.Length; i++)
                guesses[i] = "     ";
            
            // Update the letter grid usage
            UpdateUsedLetters();
        }

        public static void DrawGameBoard()
        {
            Console.Clear();

            Console.WriteLine($"{answer}");
            Console.WriteLine();

            for (int i = 0; i < guesses.Length; i++)
            {
                DrawWord(guesses[i]);
                Console.WriteLine();
            }
            DrawLetterGrid();
            Console.WriteLine();
        }

        public static void DrawWord(string guessWord)
        {
            var guessValidity = Validate(guessWord, answer);

            for (int i = 0; i < guessWord.Length; i++)
            {
                GuessState validity = guessValidity[i];
                SetColor(validity);

                char letter = guessWord[i];
                Console.Write($" {letter} ");

                Console.ResetColor();
                Console.Write(" ");
            }
            Console.WriteLine();
        }
        public static void SetColor(GuessState guess)
        {
            switch (guess)
            {
                case GuessState.Wrong:
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Gray;
                    break;

                case GuessState.RightLetterWrongPosition:
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    break;

                case GuessState.RightLetterRightPosition:
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Green;
                    break;

                case GuessState.NoGuess:
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    break;
            }
        }

        public static GuessState[] Validate(string guess, string answer)
        {
            List<char> characters = new List<char>(answer);
            GuessState[] validity = new GuessState[characters.Count];

            // If guess is whitespace, return default validity (no guess).
            if (string.IsNullOrWhiteSpace(guess))
                return validity;

            // Initialize array with assumption each character guess is wrong.
            for (int i = 0; i < validity.Length; i++)
                validity[i] = GuessState.Wrong;

            // Check for all character guesses taht are correct.
            // NOTE: loop is in reverse order.
            for (int i = answer.Length - 1; i >= 0; i--)
            {
                char cGuess = guess[i];
                char cAnswer = answer[i];
                bool isCorrect = cGuess == cAnswer;
                if (isCorrect)
                {
                    validity[i] = GuessState.RightLetterRightPosition;
                    characters.RemoveAt(i);
                }
            }
            // Check for letters which are used elsewhere in the word (right letter wrong spot).
            for (int i = 0; i < characters.Count; i++)
            {
                // Skip letters we know to be correct
                bool isCorrect = validity[i] == GuessState.RightLetterRightPosition;
                if (isCorrect)
                    continue;
                // Check to see if word contains character we guessed. If it exists,
                // we got a correct guess in an incorrect spot.
                char cGuess = guess[i];
                bool containsCharacter = characters.Contains(cGuess);
                if (containsCharacter)
                {
                    validity[i] = GuessState.RightLetterWrongPosition;
                    characters.Remove(cGuess);
                }
            }
            return validity;
        }


        public static string GetPlayerGuess()
        {
            // Keep trying to get player guess until one guess is valid.
            while (true)
            {
                Console.WriteLine("Guess a 5 letter word.");
                string input = Console.ReadLine()!;

                // Weed out bad input and try guessing again.
                if (string.IsNullOrEmpty(input))
                    continue;
                if (input.Length != 5)
                    continue;

                input = input.ToLower();
                bool isValidWordleWord = validWords.Contains(input);
                if (!isValidWordleWord)
                    continue;

                input = input.ToUpper();
                return input;
            }
        }

        public static string PickRandomWord()
        {
            int index = random.Next(0, validWords.Length);
            string word = validWords[index];
            word = word.ToUpper();
            return word;
        }

        public static string[] LoadWordleWordListByFilePath(string filePath)
        {
            // Check to see if the specified file exists. Throw an error if it doesn't.
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            // Rad all lines from file (one word per line), convert to array, and return.
            string[] validWords = File.ReadLines(filePath).ToArray();
            return validWords;
        }

        public static string[] LoadWordleWordListByFileName(string fileName)
        {
            // Get the Current Working Directory (CWD)
            string cwd = Directory.GetCurrentDirectory();
            // Get the folder's name. (Ex: "net7.0")
            string dirName = Path.GetFileName(cwd);
            // If null of empty, return an empty array.
            if (string.IsNullOrEmpty(dirName))
                return new string[0];

            // Check to see if we are running a debug build. This is a bit hacky,
            // but checking for "net" is a fine way to check for "net.7.0" and other
            // .NET versions of build output.
            bool isDebugBuild = dirName.Contains("net", StringComparison.Ordinal);
            if (isDebugBuild)
            {
                // Step up directory 3 times and record new location.
                Directory.SetCurrentDirectory(@"..\..\..\");
                cwd = Directory.GetCurrentDirectory();
            }

            // Build the file path and load the file
            string filePath = Path.Combine(cwd, fileName);
            string[] validWords = LoadWordleWordListByFilePath(filePath);

            return validWords;
        }

        public static void UpdateUsedLetters()
        {
            for (int i = 0; i < usedLetters.Length; i++)
                usedLetters[i] = GuessState.NoGuess;

            foreach (var guess in guesses)
            {
                bool isWhitespace = string.IsNullOrWhiteSpace(guess);
                if (isWhitespace)
                    continue;

                for (int i = 0; i < guess.Length; i++)
                {
                    char cGuess = guess[i];
                    char cAnswer = answer[i];
                    int cIndex = cGuess - 'A';
                    bool isCorrect = cGuess == cAnswer;
                    if (isCorrect)
                    {
                        usedLetters[cIndex] = GuessState.RightLetterRightPosition;
                    }
                    else if (answer.Contains(cGuess))
                    {
                        usedLetters[cIndex] = GuessState.RightLetterWrongPosition;
                    }
                    else
                    {
                        usedLetters[cIndex] = GuessState.Wrong;
                    }
                }
            }
        }

        public static void DrawLetterGrid()
        {
            Console.WriteLine();
            //Console.WriteLine("Used Letters");
            for (int i = 0; i < 26; i++)
            {
                char c = (char)('A' + i);
                SetColor(usedLetters[i]);
                Console.Write($" {c} ");
                Console.ResetColor();
                Console.Write(" ");

                if (i % 9 == 8)
                {
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        public enum GuessState
        {
            NoGuess,
            Wrong,
            RightLetterWrongPosition,
            RightLetterRightPosition,
        }

    }
}