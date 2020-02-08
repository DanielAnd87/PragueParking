using System;

namespace MenuLibrary
{
    public static class MenuUtils
    {



        public static long AskForNumber(string questionText)
        {
            Console.Clear();
            Console.WriteLine(questionText);
            long response = 1;
            bool isANumber = false;
            while (!isANumber || response < 0)
            {
                isANumber = long.TryParse(Console.ReadLine(), out response);
            }
            return response;
        }
        public static string AskForString(string questionText)
        {
            Console.Clear();
            Console.WriteLine(questionText);
            string response = Console.ReadLine();
            return response;

         
        }
        
        public static string AskForStringWithoutSpecialChar(string questionText)
        {
            Console.Clear();
            Console.WriteLine(questionText);
            string response = Console.ReadLine();
            string forbiddenChars = "!#¤%&//()=?@£$€{[]}½§´'";
            bool noForbiddenChar = true;

            foreach (char forbidden in forbiddenChars)
            {
                if (response.Contains(forbidden.ToString()))
                {
                    noForbiddenChar = false;
                }
            }
            if (noForbiddenChar || response.Length > 10)
            {
                return response;
            }
            else
            {
                return AskForStringWithoutSpecialChar(questionText);
            }
        }

        /// <summary>
        /// This method makes it possible to choose from alternetives using the keyboard up and down arrows.
        /// </summary>
        /// <param name="currentSelected">Starting position in the list</param>
        /// <param name="alternetives">A string array with alternetives to choose from.</param>
        /// <param name="questionText">The reason to choose anything at all.</param>
        /// <returns></returns>
        public static int AlternetivesMenu(int currentSelected, string[] alternetives, string questionText)
        {
            Console.Clear();
            return AlternetivesMenuNoClean(currentSelected, alternetives, questionText);
        }

        public static int AlternetivesMenuNoClean(int currentSelected, string[] alternetives, string questionText)
        {
            Console.WriteLine(questionText);
            if (alternetives.Length == 0)
            {
                Console.WriteLine("Nothing to show, press a button to return.");
                Console.ReadKey();
                return -1;
            }
            CreatePages(currentSelected, alternetives);

            ConsoleKeyInfo input = Console.ReadKey();
            switch (input.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentSelected == 0)
                    {
                        return AlternetivesMenu(alternetives.Length - 1, alternetives, questionText);
                    }
                    else
                    {
                        return AlternetivesMenu(currentSelected - 1, alternetives, questionText);
                    }
                case ConsoleKey.DownArrow:
                    if (currentSelected == alternetives.Length - 1)
                    {
                        return AlternetivesMenu(0, alternetives, questionText);
                    }
                    else
                    {
                        return AlternetivesMenu(currentSelected + 1, alternetives, questionText);
                    }
                case ConsoleKey.Enter:
                    return currentSelected;
                default:
                    return AlternetivesMenu(currentSelected, alternetives, questionText);
            }
        }

        public static DateTime AskForDate(string welcomeText)
        {
            string date = AskForString($"{welcomeText}\n\nFyll i datumet i denna formen: yyyy-MM-dd");
            try
            {
                return DateTime.Parse(date);
            }
            catch
            {
                return AskForDate(welcomeText);
            }
        }



        /// <summary>
        /// Present a list of alternetives. The list will be cut down into pages if to long.
        /// </summary>
        /// <param name="currentSelected"></param>
        /// <param name="alternetives"></param>
        private static void CreatePages(int currentSelected, string[] alternetives)
        {
            int length = alternetives.Length;
            int leftover = length % 10;
            int pointerLeftover = currentSelected % 10;
            int start;
            int stop;
            bool isLongList = length > 10;
            if (isLongList)
            {
                if (currentSelected >= alternetives.Length - leftover)
                {
                    start = alternetives.Length - leftover;
                    stop = alternetives.Length;
                }
                else
                {
                    start = currentSelected - pointerLeftover;
                    stop = start + 10;
                }
            }
            else
            {
                start = 0;
                stop = alternetives.Length;
            }
            for (int i = start; i < stop; i++)
            {
                if (currentSelected == i)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine(alternetives[i]);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nSelect with up and down arrows and press enter when done.");
            if (isLongList)
            {
                Console.WriteLine("This is a long list! Keep scrolling for more pages.");

            }
        }

        public static void PauseUntilFeedback(string message)
        {
            Console.WriteLine(message);
            Console.ReadKey();
        }
    }
}
