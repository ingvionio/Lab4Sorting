﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab4Sorting
{
    // Это читатель снова
    public class FileReader
    {
        public List<string> ReadListFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            string text = File.ReadAllText(filePath);
            string[] words = Regex.Split(text, @"\W+");

            // Filter out empty strings that might result from consecutive delimiters
            List<string> wordList = words
                .Where(word => !string.IsNullOrEmpty(word))
                .Select(word => word.ToLower())
                .ToList();

            return wordList;
        }
    }
}
