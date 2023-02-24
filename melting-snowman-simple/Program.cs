// 12:07 to 12:30
// 03:50 to 3:53
// 03:53 to 4:05 - nicify
// Total time: ~38min

namespace MeltingSnowmanSimple
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // INIT
            string[] answers = new string[] { "my guess" };
            Random random = new Random();
            int index = random.Next(answers.Length);
            string answer = answers[index];
            int guessesRemaining = 7;
            char[] displayChars = new char[answer.Length];

            // Set default string value for display
            for (int i = 0; i < answer.Length; i++)
            {
                char c = answer[i];
                if (c == ' ')
                {
                    displayChars[i] = ' ';
                }
                else
                {
                    displayChars[i] = '_';
                }
            }

            // COPY PASTED
            for (int i = 0; i < displayChars.Length; i++)
                Console.Write(" " + displayChars[i]);
            Console.WriteLine();

            // GAME LOOP
            while (true)
            {
                Console.WriteLine("Guess letter:");
                string guess = Console.ReadLine();
                guess = guess.ToLower();

                // CHECK: single character input
                bool isOneCharacter = guess.Length == 1;
                if (!isOneCharacter)
                {
                    Console.WriteLine("Guess must be 1 character long!");
                    continue;
                }

                // CHECK: is a lower case letter a-z
                char character = guess[0];
                bool isLetter = character >= 'a' && character <= 'z'; // 97, 122
                if (!isLetter)
                {
                    Console.WriteLine("Guess must be a letter!");
                    continue;
                }

                // CHECK: right or wrong?
                bool guessIsRight = answer.Contains(character);
                if (guessIsRight)
                {
                    for (int i = 0; i < answer.Length; i++)
                    {
                        bool isRight = answer[i] == character;
                        if (isRight)
                        {
                            displayChars[i] = character;
                        }
                    }
                    Console.WriteLine($"'{character}' is correct!");
                }
                else
                {
                    guessesRemaining--;
                    Console.WriteLine($"'{character}' is wrong. Remaining guesses: {guessesRemaining}.");
                }

                // Print to screen
                for (int i = 0; i < displayChars.Length; i++)
                    Console.Write(" " + displayChars[i]);
                Console.WriteLine();

                // CHECK: has guessed word/sentence?
                string known = "";
                for (int i = 0; i < displayChars.Length; i++)
                    known += displayChars[i];
                bool isCorrect = answer == known;
                if (isCorrect)
                {
                    Console.WriteLine("You win!");
                    break;
                }

                // CHECK: player out of guesses?
                bool playerLose = guessesRemaining == 0;
                if (playerLose)
                {
                    Console.WriteLine("You lose!");
                    break;
                }
            }

            // Close nicely
            Console.WriteLine("Press any key to end game");
            Console.Read();
        }

    }
}