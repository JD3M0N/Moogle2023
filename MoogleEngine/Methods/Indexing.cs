using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoogleEngine;

namespace MoogleEngine.Methods
{
    internal class Indexing
    {
        //getting the contest path
        static public string ContentPath()
        {
            string cPath = Directory.GetCurrentDirectory();
            cPath = cPath.Remove(cPath.LastIndexOf('M'));
            cPath += "/Content/";

            return cPath;
        }

        //getting content paths
        static public string[] ContentPaths(string cPath)
        {
            string[] cPaths = Directory.GetFiles(cPath, "*.txt");

            return cPaths;
        }

        //getting the text of a doc
        static public string[] ReadingDocs(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            string[] textCleaned = CleaninText(text);

            return textCleaned;
        }


        //cleanin' out my closet 
        public static string[] CleaninText(string textToClean)
        {
            textToClean = textToClean.ToLower();

            string[] textCleaned;

            textCleaned = textToClean.Split("~^*!@#$%^&()_+-=` ,./;\'<|>?:[]{}?".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return textCleaned;
        }

        public static string[] CleaninQuery(string queryToClean)
        {
            string[] query;

            queryToClean = queryToClean.ToLower();
            query = queryToClean.Split("@#$%^&()_+-=` ,./;\'<|>?:[]{}?".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return query;
        }

        //Makin' the big boss
        static public Tuple<Dictionary<string, Dictionary<string, double>>, Dictionary<string, double>> DocumentsDictionary( /* string[] paths, string[] texts */ )
        {
            Dictionary<string, Dictionary<string, double>> DocumentsDictionary = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, double> DictionaryOfWords = new Dictionary<string, double>();
            Tuple<Dictionary<string, Dictionary<string, double>>, Dictionary<string, double>> BothDictionaries = new Tuple<Dictionary<string, Dictionary<string, double>>, Dictionary<string, double>>(DocumentsDictionary, DictionaryOfWords);


            foreach (string path in ContentPaths(ContentPath()))
            {
                DocumentsDictionary.Add(path, new Dictionary<string, double>());
                MoogleEngine.Moogle.TextsCleaned.Add(path, ReadingDocs(path));
                
                foreach (string word in ReadingDocs(path))
                {
                    if (DocumentsDictionary[path].ContainsKey(word))
                    {
                        DocumentsDictionary[path][word]++;
                    }
                    else
                    {
                        if (DictionaryOfWords.ContainsKey(word))
                        {
                            DictionaryOfWords[word]++;
                        }
                        else
                        {
                            DictionaryOfWords[word] = 1;
                        }
                        DocumentsDictionary[path][word] = 1;
                    }
                }
            }

            return BothDictionaries;
        }

        //Modifier method 
        static public Dictionary<string, double> Modifiers(string Query)
        {
            Dictionary<string, double> modifiers = new Dictionary<string, double>();
            string[] cleanQuery = CleaninQuery(Query);

            foreach (string word in cleanQuery)
            {
                int count = 0;

                foreach (char c in word)
                {
                    if (c == '!')
                    {
                        modifiers[word.Remove(0, 1)] = 0;
                       // Console.WriteLine(Modifiers(word));
                    }
                    else if (c == '^')
                    {
                        modifiers[word.Remove(0, 1)] = -1;
                        //Console.WriteLine(Modifiers(word));
                    }
                    else if (c == '~')
                    {
                        modifiers[word.Remove(0, 1)] = -2;
                    }
                    else if (c == '*')
                    {
                        count++;
                    }
                    else break;
                }
                if (count > 0)
                {
                    modifiers[word.Remove(0, count)] = count;
                    //Console.WriteLine(Modifiers(word));
                }
            }



            return modifiers;
        }



    }
}
