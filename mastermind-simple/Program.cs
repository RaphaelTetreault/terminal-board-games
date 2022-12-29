namespace mastermind_simple
{
    // coded in 20 minutes.
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Master Mind!");
                Console.WriteLine("How to play:");
                Console.WriteLine("Type 4 letters to guess the master mind's code.");
                Console.WriteLine("Valid characters include: r, y, g, b, c, m");
                Console.WriteLine();

                string answer = "rgbr";
                int guessesRemaining = 10;
                while (guessesRemaining > 0)
                {
                    Console.WriteLine($"Guesses remaining: {guessesRemaining}");
                    Console.WriteLine("Guess:");
                    string guess = Console.ReadLine();
                    if (string.IsNullOrEmpty(guess) || guess.Length != 4)
                    {
                        Console.WriteLine("Invalid input. Try again.");
                        continue;
                    }

                    bool invalidChar = false;
                    int correctBlack = 0;
                    guess = guess.ToLower();
                    for (int i = 0; i < guess.Length; i++)
                    {
                        char c = guess[i];
                        if (c == 'r' || c == 'y' || c == 'g' || c == 'b' || c == 'c' || c == 'm')
                        {
                            if (c == answer[i])
                                correctBlack++;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Try again.");
                            invalidChar = true;
                            break;
                        }
                    }
                    if (invalidChar)
                        continue;

                    // good input
                    if (correctBlack == guess.Length)
                    {
                        Console.WriteLine("Correct! You win!");
                        Console.WriteLine();
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Correct color and placement: {correctBlack}.");
                        Console.WriteLine();
                    }

                    guessesRemaining--;
                }

                if (guessesRemaining == 0)
                    Console.WriteLine($"You lose! Code was '{answer}'");

                Console.WriteLine($"Press any key to continue.");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }
}