using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab4Sorting
{
    // Это читатель
    public class FileReader
    {
        public string[] ReadWordsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            string text = File.ReadAllText(filePath);
            string[] words = Regex.Split(text, @"\W+");

            // Filter out empty strings that might result from consecutive delimiters
            return Array.FindAll(words, word => !string.IsNullOrEmpty(word));
        }
    }
}
