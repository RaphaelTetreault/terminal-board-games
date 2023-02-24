// Written from 4:15 to 5:15

namespace melting_snowman
{
    internal class Program
    {
        public static readonly Random random = new Random();
        public static readonly string[] Answers = new string[]
        {
            "Hello, World!",
            "To be or not to be",
            "supercalifragilisticexpialidocious",
        };

        static void Main(string[] args)
        {
            while (true)
            {
                RunGame();
            }
        }

        public static void RunGame()
        {
            bool[] guessedIndexes = new bool[26];
            int randomIndex = random.Next(Answers.Length);
            string answer = Answers[randomIndex];
            int guessesRemaining = 7;

            while (true)
            {
                // Print active guesses
                Console.Clear();
                PrintAnswerGuesses(guessedIndexes, answer);

                // Get player's guess
                char guess = GetPlayerGuess(guessedIndexes);

                // Reset screen, print guesses with newly guessed char (if applicable)
                Console.Clear();
                PrintAnswerGuesses(guessedIndexes, answer);

                // Show user if correct or not
                bool isCorrect = answer.ToLower().Contains(guess);
                if (isCorrect)
                {
                    Console.WriteLine($"'{guess}' is correct!");
                }
                else
                {
                    guessesRemaining--;
                    Console.WriteLine($"'{guess}' is incorrect!");
                }

                // Check for win
                bool isWinner = IsWinner(guessedIndexes, answer);
                if (isWinner)
                {
                    Console.WriteLine("You win!");
                    PromptPressEnter();
                    break;
                }
                // Check for lose
                bool isLoser = guessesRemaining <= 0;
                if (isLoser)
                {
                    Console.WriteLine("You lose!");
                    PromptPressEnter();
                    break;
                }

                // Tell user remaining guesses
                Console.WriteLine($"You have {guessesRemaining} more guess{(guessesRemaining != 1 ? "" : "es")}.");

                // Get ready for next cycle
                PromptPressEnter();
            }
        }

        public static bool IsLowerCaseLetter(char character)
        {
            bool isLetter = character >= 'a' && character <= 'z';
            return isLetter;
        }

        public static void PromptPressEnter()
        {
            Console.WriteLine("Press ENTER to continue.");
            Console.ReadLine();
        }

        public static void PrintAnswerGuesses(bool[] letterGuessed, string answer)
        {
            string lowerCaseAnswer = answer.ToLower();

            for (int i = 0; i < answer.Length; i++)
            {
                char cClean = lowerCaseAnswer[i];
                char cOriginal = answer[i];

                bool isLetter = IsLowerCaseLetter(cClean);
                if (!isLetter)
                {
                    PrintCharSpaced(cOriginal, ConsoleColor.Black);
                    continue;
                }

                int characterIndex = cClean - 'a';
                bool isGuessed = letterGuessed[characterIndex];
                if (isGuessed)
                {
                    PrintCharSpaced(cOriginal, ConsoleColor.DarkGreen);
                }
                else
                {
                    PrintCharSpaced(' ', ConsoleColor.DarkGray);
                }
            }
            Console.WriteLine();
        }

        public static void PrintCharSpaced(char character, ConsoleColor bgColor = ConsoleColor.Black)
        {
            Console.BackgroundColor = bgColor;
            Console.Write(' ');
            Console.Write(character);
            Console.Write(' ');
            Console.ResetColor();
            Console.Write(' ');
        }

        public static char GetPlayerGuess(bool[] guesses)
        {
            while (true)
            {
                Console.WriteLine("Guess a letter:");
                string input = Console.ReadLine();
                bool isNullOrEmpty = string.IsNullOrEmpty(input);
                if (isNullOrEmpty)
                {
                    Console.WriteLine("Answer cannot be empty.");
                    continue;
                }

                bool isNotExactly1Char = input.Length != 1;
                if (isNotExactly1Char)
                {
                    Console.WriteLine("Answer must be 1 character.");
                    continue;
                }

                char letter = input[0];
                bool isNotLetter = !IsLowerCaseLetter(letter);
                if (isNotLetter)
                {
                    Console.WriteLine("Answer must be a letter.");
                    continue;
                }

                int index = letter - 'a';
                bool hasBeenGuessed = guesses[index];
                if (hasBeenGuessed)
                {
                    Console.WriteLine($"You already guessed '{letter}'.");
                    continue;
                }

                // Set guess in array
                guesses[index] = true;
                return letter;
            }
        }

        public static bool IsWinner(bool[] guesses, string answer)
        {
            answer = answer.ToLower();
            for (int i = 0; i < answer.Length; i++)
            {
                char character = answer[i];
                bool isLetter = IsLowerCaseLetter(character);
                if (!isLetter)
                    continue;

                int index = character - 'a';
                bool isGuessed = guesses[index];
                if (!isGuessed)
                    return false;
            }

            return true;
        }
    }
}