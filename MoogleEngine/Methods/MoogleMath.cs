using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoogleEngine;

namespace MoogleEngine.Methods
{
    internal class MoogleMath
    {
        static public int StringDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            //Step 1, if a word is empty the minimun number of changes is the word that isn't empty
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }

            //Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {

            }
            for (int j = 0; j <= m; d[0, j] = j++)
            {

            }

            //Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    //Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    //Step 6
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j -1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            //Step 7
            return d[n, m];

        }
        static public string[] NewQuery(Dictionary<string, double> DictionaryOfWords, string[] Query, Dictionary<string, Dictionary<string, double>> DocumentsDictionary)
        {
            string[] newQuery = new string[Query.Length];
            int i = 0;
            string tempWord = "";

            foreach (string word in Query)
            {
                int minLevenshtein = int.MaxValue;

                if (!DictionaryOfWords.ContainsKey(word))
                {
                    foreach (string worrd in DictionaryOfWords.Keys)
                    {
                        int levenshteinDist = StringDistance(word, worrd);
                        if (minLevenshtein > levenshteinDist)
                        {
                            minLevenshtein = StringDistance(word, worrd);
                            tempWord = worrd;
                            newQuery[i] = worrd;

                        }
                    }
                }
                else
                {
                    newQuery[i] = word;
                }
                i++;
            }
            MoogleEngine.Moogle.Sugestion = string.Join(" ", newQuery);
            return newQuery;
        }
        //Compares words with the words of in the query and if the words are similar with Levensh... the value of the word in the document increases 
        static public void SimilarWordsIncreaser(Dictionary<string, Dictionary<string, double>> DocumentDictionary, string Query)
        {
            string[] cleanQuery = Indexing.CleaninQuery(Query);

            foreach (string document in DocumentDictionary.Keys)
            {
                foreach (string word in DocumentDictionary[document].Keys)
                {
                    int minLevesh = int.MaxValue;
                    string similarLevesh = "";

                    foreach (string queryWord in cleanQuery)
                    {
                        int temporalMin = StringDistance(queryWord, word);

                        if ((temporalMin < minLevesh) && (temporalMin < 3) && (temporalMin > 0))
                        {
                            minLevesh = temporalMin;
                            similarLevesh = queryWord;
                        }
                    }

                    if (similarLevesh == "") break;
                    else
                    {
                        if (!DocumentDictionary[document].ContainsKey(similarLevesh)) DocumentDictionary[document][similarLevesh] = 0;
                        DocumentDictionary[document][similarLevesh] +=  DocumentDictionary[document][word] / (2 + minLevesh);
                    }

                }
            }
        }

        //Same method when the order doesnt matter but results are worst
        static public double NearWordsValueNotOrder(string[] cleanText, string[] closeWords)
        {
            double result = int.MaxValue;
            int[] wordIndex = new int[closeWords.Length];

            for (int i = 0; i < closeWords.Length; i++)
            {
                wordIndex[i] = -1;
            }

            for (int i = 0; i < cleanText.Length; i++)
            {
                string word = cleanText[i];

                //if the word is the close word added it and calculate is the result is better
                if (closeWords.Contains(word))
                {

                    wordIndex[Array.IndexOf(closeWords, word)] = i;
                    result = CalculatingBestResult(result, wordIndex);

                }
            }
            return 1.0 / result;
        }

        //Method to calculate the best result possible
        static public double CalculatingBestResult(double Result, int[] IndexArray)
        {
            double newResult = 0;
            if (IndexArray.Contains(-1)) return Result;

            for (int i = 0; i < IndexArray.Length - 1; i++)
            {
                newResult += IndexArray[i] - IndexArray[i + 1];
            }

            if (Math.Abs(newResult) < Result) return Math.Abs(newResult);
            return Result;
        }


        //Method to know how much close are some words between eachother, can be use to the modifiers ~ or even to the query words 
        static public double NearWordsValue(string[] cleanText, string[] closeWords)
        {
            double result = 0;
            // Last Word Added -> LWA
            string LWA = "";
            int[] wordIndex = new int[closeWords.Length];

            for (int i = 0; i < closeWords.Length; i++)
            {
                wordIndex[i] = -1;
            }

            for (int i = 0; i < cleanText.Length; i++)
            {
                string word = cleanText[i];

                //If the word is one of the ones i'm looking for to be close (o ar similar one)
                if (closeWords.Contains(word))
                {
                    //if i have added anything before, or i finished adding the last sentence
                    if (closeWords.FirstOrDefault() == word && LWA == "")
                    {
                        wordIndex[0] = i;
                        LWA = word;

                        // If the order doesnt matter this is the way to improve
                        // result = CalculatingBestResult(result, wordIndex);


                    }
                    //If the word i'm adding is the next one in the words order 
                    if (word == closeWords[Array.IndexOf(closeWords, LWA) + 1])
                    {
                        //If the word is the last one as a spacial case to make the math 
                        if (word == closeWords[closeWords.Length - 1])
                        {
                            LWA = "";
                            wordIndex[closeWords.Length - 1] = i;

                            result = CalculatingBestResult(result, wordIndex);
                        }
                        //In any other case just add the word and move on
                        else
                        {
                            wordIndex[Array.IndexOf(closeWords, LWA) + 1] = i;
                            LWA = word;
                        }
                    }
                }
            }

            return 1.0 / result;
        }

        static public string[] SynonymsResults(Dictionary<string, List<string>> synonymDictio, string[] query, Dictionary<string, double> DictionaryOfWords)
        {
            List<string> result = new List<string>();
            result = query.ToList();

            foreach (string word in query)
            {
                int i = 0;
                if (synonymDictio.ContainsKey(word))
                {
                    foreach (string synonym in synonymDictio[word])
                    {
                        if (DictionaryOfWords.ContainsKey(synonym))
                        {

                            if (i < 3)
                            {
                                result.Add(synonym);
                                i++;
                            }
                            else continue;
                        }
                    }

                }
            }

            string[] newQuery = result.ToArray();

            return newQuery;
        }

        static public Dictionary<string, double> SynonymScore(Dictionary<string, double> score, string[] newQuery)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            Dictionary<string, double> newScore = new Dictionary<string, double>();

            newScore = VectorialModel.MaindMethodOfVectorialModel(MoogleEngine.Moogle.Queri, newQuery, MoogleEngine.Moogle.MainStructure, MoogleEngine.Moogle.IDF, MoogleEngine.Moogle.WeightValueWords);

            foreach (string document in score.Keys)
            {
                result[document] = score[document] + (newScore[document] * 0.5);
            }

            return result;
        }

        static public string RealSnipet(string[] textCleaned)
        {
            int l = 0;
            int r = 50;
            int bestl = l;
            double result = 0;
            double best = 0;


            for (int i = 0; i<50; i++)
            {
                if (MoogleEngine.Moogle.WInQuery.ContainsKey(textCleaned[i]))
                {
                    result+=MoogleEngine.Moogle.WInQuery[textCleaned[i]];
                }
            }

            for (int i = 51; i < textCleaned.Length; i++)
            {
                if (MoogleEngine.Moogle.WInQuery.ContainsKey(textCleaned[i]))
                {
                    result+=MoogleEngine.Moogle.WInQuery[textCleaned[i]];
                }
                if (MoogleEngine.Moogle.WInQuery.ContainsKey(textCleaned[i-50]))
                {
                    result-=MoogleEngine.Moogle.WInQuery[textCleaned[i-50]];
                }

                l++;


                if (result > best)
                {
                    bestl=l;
                    best=result;
                }

            }

            string[] sResult = new string[50];

            for(int i=0; i<50; i++)
            {
                sResult[i] = textCleaned[bestl+i];
            }

            return String.Join(" ",sResult);
        }
    }
}
