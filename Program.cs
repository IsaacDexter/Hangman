using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualBasic.FileIO;


namespace Hangman
{
    class CSVParser
    {
        protected string m_path;
        protected List<List<string>> m_fields;
        protected int m_numberOfFields;
        protected char m_delimeter;
        protected Random rand;

        /// <summary>
        /// A csv parser that can load comma seperated files (or any other seperator, defaults to comma), used in this scenario to load the words dictionary.
        /// </summary>
        /// <param name="pathToLoad"></param>
        public CSVParser(string pathToLoad, int numberOfFields = 1, char delimiter = ',')
        {
            m_fields = new List<List<string>>();    //Initialise the 2D lists of strings
            m_path = pathToLoad;
            m_numberOfFields = numberOfFields;
            m_delimeter = delimiter;

            if (pathToLoad.Contains(".csv"))    //Make sure the file is .csv
            {
                for (int i = 0; i < m_numberOfFields; i++)  //Add fields for the number of fields.
                {
                    m_fields.Add(new List<string>());
                }

                rand = new Random();
                try
                {
                    LoadCSV();
                }
                catch (Exception)
                {
                    Console.WriteLine("File could not be loaded.");
                    m_fields[0].Add("Error");
                }

            }
            else
            {
                Console.WriteLine("Please enter the path to a valid .csv file.");
            }
        }

        /// <summary>
        /// Loads a csv using the parser built into dot net. saves into the private list, words. Loads from the path stored in path
        /// </summary>
        /// <param name="delimiter">the delimiter used in the file. defaults to comma. It is a csv, after all.</param>
        protected virtual void LoadCSV()
        {
            using (TextFieldParser csvParser = new TextFieldParser(m_path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { m_delimeter.ToString() });
                csvParser.HasFieldsEnclosedInQuotes = true;

                //Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    //Read current line fields, pointer moves to the next line
                    string[] fields = csvParser.ReadFields();
                    for (int i = 0; i < m_numberOfFields; i++)
                    {
                        string entry = fields[i];
                        m_fields[i].Add(entry);
                    }

                }
            }
        }
    }

    /// <summary>
    /// A child class of csv parser featuring methods to return a random value from either the words or definitions field.
    /// </summary>
    class Dictionary : CSVParser
    {
        public Dictionary(string pathToLoad, int numberOfFields = 1, char delimiter = ',') : base(pathToLoad, numberOfFields, delimiter)
        {

        }
        /// <returns>A random word from the words dictionary</returns>
        public string GetRandomWord()
        {
            return m_fields[0][rand.Next(m_fields[0].Count)];
        }

        /// <summary>
        /// Gets the description to the word passed in by passing through each entry in the words field and getting the location
        /// </summary>
        /// <param name="word">The word to get a description for</param>
        /// <returns>The description of the word</returns>
        public string GetDefinition(string word)
        {
            for (int i = 0; i < m_fields[0].Count; i++)
            {
                if (m_fields[0][i] == word)
                {
                    return m_fields[1][i];
                }
            }
            return "No definition could be found.";
        }
    }

    /// <summary>
    /// <para>9.	Create a game of Hangman, for this you will need to use your knowledge from all the other questions and the lecture. </para>
    /// <para>a.    Use this example project if you want some starter code - Hangman-Incomplete.zip<br/>
    /// b.  I would like to thank Manas Sharma and Notmi Namae for releasing a csv dictionary here: https://www.bragitoff.com/2016/03/english-dictionary-in-csv-format/ <br/>
    /// c.  I would also like to thank user Habeeb for his answer on stack overflow on using the .net csv parser. https://stackoverflow.com/a/33796861</para>
    /// </summary>
    public class HangmanGame
    {
        /// <summary>A reference to the csv parser used to load the dictionary csv and then generate a random word.</summary>
        Dictionary dictionary;
        string word;
        string definition;
        char[] printedWord;
        int lives;
        char guess;
        string guessedChars;
        bool victory;

        /// <summary>
        /// Converts the number of remaining lives to a pictographical representation.
        /// </summary>
        void LivesToPic()
        {
            Console.WriteLine("\n     |------+  ");
            Console.WriteLine("     |      |  ");
            Console.WriteLine("     |      " + (lives < 6 ? "O" : ""));
            Console.WriteLine("     |     " + (lives < 4 ? "/" : "") + (lives < 5 ? "|" : "") + (lives < 3 ? @"\" : ""));
            Console.WriteLine("     |     " + (lives < 2 ? "/" : "") + " " + (lives < 1 ? @"\" : ""));
            Console.WriteLine("     |         ");
            Console.WriteLine("===============");
        }
        public HangmanGame()
        {
            //Loads the dictionary
            dictionary = new Dictionary("dictionaryDefinitions.csv", 2, '|');

            //sets the random word
            word = dictionary.GetRandomWord();
            //Gets the definition for the word
            definition = dictionary.GetDefinition(word);
            word = word.ToLower();
            lives = 7;
            victory = false;
            printedWord = new char[word.Length * 2];
            guessedChars = "";

            int guessAppeared;
            string input;

            //Prepares the blanks.
            for (int i = 0; i < word.Length * 2; i += 2)
            {
                printedWord[i] = '_';
                printedWord[i + 1] = ' ';
            }

            Console.WriteLine("Welcome to Hangman! Would you like a hint? [y/n]");
            if (Console.ReadLine().ToLower() == "y")
            {
                Console.WriteLine(definition);
            }

            //Main gameplay loop.
            while (lives > 0 && !victory)
            {
                guessAppeared = 0;
                Console.WriteLine(printedWord);
                LivesToPic();
                Console.WriteLine("\nThe word cannot contain the following letters: " + guessedChars);
                input = Console.ReadLine().ToLower();

                if (input.Length == 1)
                {
                    guess = input[0];    //convert the input string to a single character
                    if (!guessedChars.Contains(guess))  //Prevents the user from entering the same leter multiple times
                    {
                        for (int i = 0; i < word.Length; i++)
                        {
                            if (word[i] == guess)   //If that letter was in the character, update the printed word to show its postion
                            {
                                guessAppeared++;
                                printedWord[2 * i] = guess;
                            }
                        }

                        //inform the user of the validity of their guess.
                        if (guessAppeared > 0)
                        {
                            Console.WriteLine("Correct! " + guess + " appeared " + guessAppeared.ToString() + " times.\n");
                        }
                        else
                        {
                            Console.WriteLine("Incorrect! " + guess + " did not appear in the word.\n");
                            guessedChars += (guess + ", ");
                            lives--;
                        }

                        //Checks to see if the user has won.
                        if (!printedWord.Contains('_'))
                        {
                            victory = true;
                        }
                    }
                    else    //Catch the user if they already entered thjat letter
                    {
                        Console.WriteLine("You already guessed that letter.\n");
                    }
                }
                else if (input.Length > 1)   //The user didnt enter a character, so they must have been guessing the word as a whole. if they were correct, they win! If not, they lose a life.
                {
                    if (input == word)
                    {
                        victory = true;
                    }
                    else
                    {
                        Console.WriteLine("The word you entered was incorrect. Try guessing some characters first!");
                        lives--;
                    }
                }
                else //The user didnt enter nothin. So stop that.
                {
                    Console.WriteLine("Please enter a character or word.");
                }
            }
            if (victory)    //Handle the win or loss.
            {
                Console.WriteLine("Congrats! You won, with " + lives + " to spare!");
            }
            else
            {
                Console.WriteLine("Unfortunately, you've lost. Better luck next time!");
            }
            Console.WriteLine("The word was " + word + ".");
        }
    }

    //Launch the game in the actual program
    internal class Program
    {
        static void Main(string[] args)
        {
            HangmanGame hangmanGame = new HangmanGame();

            Console.WriteLine("\nPress the enter key to exit...");
            Console.ReadLine(); //Waits for input. Is only here to prevent the window from closing immidiately.
        }
    }
}
