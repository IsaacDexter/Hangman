# Hangman
Guess the computer's random word from the definition, one letter at a time.

## Function
Hangman is a classic party game where one player (the computer) picks a word and tells the second player how many letters are in that word by showing a sequence of blanks, like so: _ _ _ _ _ would represent a five letter word.
The second player then guesses a letter they think could be in the word, if it is, the first player will fill in the blanks with that letter wherever it appears in the word.
If the letter is not in the word, the first player instead draws a peice of the hanged man, representing the number of lives the player has left. 
If the stick figure is complete, the first player has won. Conversely, if the word is complete, the second player has won.

## How to play
This variant of hangman is slightly different, as the computer gives a hint to the player - the definition of the word they are trying to guess.
The player will then take turns guessing the letters they think are within the word. If they are correct, the position of that letter will be shown in the blanks. If not, another body part will be added to the hanged man.
At any point, the player can guess the whole word, which will either score them the victory if correct, or make them lose a life if incorrect.
An example of gameplay can be found below.
![image](https://user-images.githubusercontent.com/90466022/194879351-119909f0-c942-4a98-9e99-3a65368b83e6.png)

## The Dictionary
The game generates its words and descriptions from a delimiter seperated value (.csv) file. This is parsed through a slightly modified child of a csv parser class I created, built using the parser built into .NET.
The class has custom parameters for paths, number of fields and custom delimiter characters.
``` csharp
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
```
*The LoadCSV method in the CSVParser class, which handles the files.*
## Credits
I would like to thank Manas Sharma and Notmi Namae for releasing a csv dictionary here: https://www.bragitoff.com/2016/03/english-dictionary-in-csv-format/ 
I would also like to thank user Habeeb for his answer on stack overflow on using the .net csv parser: https://stackoverflow.com/a/33796861
