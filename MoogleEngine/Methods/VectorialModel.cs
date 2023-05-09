using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine.Methods
{
    internal class VectorialModel
    {
        // [I]nverse[D]ocument[F]requency : Total of documents / Total amount of times that the word (in query) appears in any document 
        static public Dictionary<string, double> InverseDocumentFrequency(Dictionary<string, Dictionary<string, double>> DocumentsDictionary, Dictionary<string, double> DictionaryOfWords)
        {
            Dictionary<string, double> IDF = new Dictionary<string, double>();
            Dictionary<string, double> HMTDAWAIAD = new Dictionary<string, double>();

            foreach (string word in DictionaryOfWords.Keys)
            {
                IDF[word] = Math.Log(DocumentsDictionary.Count / DictionaryOfWords[word]);
            }

            return IDF;
        }

        //Now doing tf
        static public Dictionary<string, Dictionary<string, double>> NormalizedFrequency(Dictionary<string, double> IDF, Dictionary<string, Dictionary<string, double>> DocumentDictionary)
        {
            // Normalized frequency: how many tinmes does a word appears in a document normalized 
            Dictionary<string, Dictionary<string, double>> NormalizedFrequency = new Dictionary<string, Dictionary<string, double>>();

            foreach (string document in DocumentDictionary.Keys)
            {
                double temp = DocumentDictionary[document].Values.Max();
                NormalizedFrequency[document] = new Dictionary<string, double>();
                foreach (string word in IDF.Keys)
                {
                    if (DocumentDictionary[document].ContainsKey(word))
                    {
                        NormalizedFrequency[document][word] = (double)((double)DocumentDictionary[document][word] / (double)temp);
                    }
                    else
                    {
                        NormalizedFrequency[document][word] = 0;
                    }
                }
            }

            return NormalizedFrequency;
        }

        //TF * IDF
        static public Dictionary<string, Dictionary<string, double>> WeightValueWord(Dictionary<string, Dictionary<string, double>> TF, Dictionary<string, double> IDF)
        {
            Dictionary<string, Dictionary<string, double>> TFxIDF = new Dictionary<string, Dictionary<string, double>>();

            foreach (string document in TF.Keys)
            {
                Dictionary<string, double> docTFIDF = new Dictionary<string, double>();

                foreach (string word in TF[document].Keys)
                {
                    docTFIDF.Add(word, TF[document][word] * IDF[word]);
                }


                TFxIDF.Add(document, docTFIDF);
            }

            return TFxIDF;
        }
        //Word weight in query
        static public Dictionary<string, double> WWIQ(Dictionary<string, double> IDF, string[] Query)
        {
            Dictionary<string, double> WWIQ = new Dictionary<string, double>();
            Dictionary<string, double> QueryFrequency = new Dictionary<string, double>();

            foreach (string word in Query)
            {
                if (!QueryFrequency.ContainsKey(word))
                {
                    QueryFrequency[word] = 1;

                }
                else QueryFrequency[word]++;
            }

            double maxWord = QueryFrequency.Values.Max();
            double alpha = 0.4;

            foreach (string word in Query)
            {
                WWIQ[word] =(double)((double)alpha + (double)(1.0 - alpha)* (double)(QueryFrequency[word] / (double)maxWord)) * IDF[word];
            }

            MoogleEngine.Moogle.WInQuery = WWIQ;
            return WWIQ;
        }

        //Ranking method 

        static public Dictionary<string, double> MaindMethodOfVectorialModel(string query, string[] Query, Dictionary<string, Dictionary<string, double>> DocumentDictionary, Dictionary<string, double> IDF, Dictionary<string, Dictionary<string, double>> WeightVW)
        {
            Dictionary<string, double> Score = new Dictionary<string, double>();
            Dictionary<string, double> WordWeightInQquery = WWIQ(IDF, Query);

            foreach (string document in DocumentDictionary.Keys)
            {
                Score[document] = 0;

                double temportalSum1 = 0;
                double temportalSum2 = 0;   
                double temportalSum3 = 0;

                foreach (string word2 in DocumentDictionary[document].Keys)
                {
                    temportalSum2 += Math.Pow(WeightVW[document][word2], 2);
                }

                foreach (string word in Query)
                {
                    temportalSum3 += Math.Pow(WordWeightInQquery[word], 2);

                    if (WeightVW[document].ContainsKey(word))
                    {
                        temportalSum1 += (WeightVW[document][word] * WordWeightInQquery[word]);

                    }
                }
                Score[document] = temportalSum1 / (Math.Sqrt(temportalSum2) * Math.Sqrt(temportalSum3));

            }

            Score = ResolveModifiers(Indexing.Modifiers(query), DocumentDictionary, Score, MoogleEngine.Moogle.TextsCleaned);

            return Score;
        }

        //Method to resolve the modifier problem
        static public Dictionary<string, double> ResolveModifiers(Dictionary<string, double> Modifiers, Dictionary<string, Dictionary<string, double>> DocumentsDisctionary, Dictionary<string, double> score, Dictionary <string, string[]> TextoCleaned)
        {

            //calculating how many near words
            if (Modifiers.ContainsValue(-2))
            {
                List<string> nearWords = new List<string>();

                foreach (string word in Modifiers.Keys)
                {
                    if (Modifiers[word] == -2)
                    {
                        nearWords.Add(word);
                    }
                }

                string[] closeWords = nearWords.ToArray();

                foreach (string document in DocumentsDisctionary.Keys)
                {
                    score[document] = score[document] + MoogleMath.NearWordsValueNotOrder(TextoCleaned[document], closeWords);
                }
            }

            if (Modifiers != null)
            {

                //For each word with a modifier 
                foreach (string word in Modifiers.Keys)
                {
                    //if the modifier is <= 0 means that its 0(!) or -1(^) and its not positive in the * modifier case
                    if (Modifiers[word] <= 0)
                    {
                        //to look in every document to know if contains(dot not contain) 
                        foreach (string document in DocumentsDisctionary.Keys)
                        {
                            //If the value is 0 the word can't appear in the document and the document is unnecesary
                            if (Modifiers[word] == 0)
                            {
                                //Well if the document contains the word, then eliminate it 
                                if (DocumentsDisctionary[document].ContainsKey(word))
                                {
                                    score[document] = 0;
                                    //DocumentsDisctionary.Remove(path);
                                }
                            }
                            //In the case of the value -1, the word needs to necessary appears
                            else if (Modifiers[word] == -1)
                            {
                                //If the document doesn't contains the word then eliminate it because its not necesary
                                if (!DocumentsDisctionary[document].ContainsKey(word))
                                {
                                    score[document] = 0;
                                    //DocumentsDisctionary.Remove(path);
                                }
                            }
                        }
                    }
                    //this means that the mofiers is >1(*)
                    else
                    {
                        //to searh in every document
                        foreach (string path in DocumentsDisctionary.Keys)
                        {
                            //if the document cointains the word then multiplicate the value of the word to get a higher value 
                            if (DocumentsDisctionary[path].ContainsKey(word))
                            {
                                score[path] = DocumentsDisctionary[path][word]*(0.75*Modifiers[word]);
                                //DocumentsDisctionary[path][word] = DocumentsDisctionary[path][word]*(1 + (0.5*Modifiers[word]));
                            }
                        }
                    }
                }
            }

            return score;
        }
    }
}
