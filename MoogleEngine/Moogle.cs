using MoogleEngine.Methods;
using System.Diagnostics;
using System.Text.Json;

namespace MoogleEngine;


public static class Moogle
{
    public static Dictionary<string, Dictionary<string, double>> MainStructure = new Dictionary<string, Dictionary<string, double>>();
    public static Dictionary<string, double> DictionaryOfWords = new Dictionary<string, double>();
    public static Dictionary<string, double> IDF = new Dictionary<string, double>();
    public static Dictionary<string, Dictionary<string, double>> TF = new Dictionary<string, Dictionary<string, double>>();
    public static Dictionary<string, Dictionary<string, double>> WeightValueWords = new Dictionary<string, Dictionary<string, double>>();
    public static Dictionary<string, double> Modifiers = new Dictionary<string, double>();
    public static Dictionary<string, double> VectModel = new Dictionary<string, double>();
    public static Dictionary<string, double> WInQuery = new Dictionary<string, double>();
    public static Dictionary<string, List<string>> SynonymousDictionary = new Dictionary<string, List<string>>();
    public static Dictionary<string, string[]> TextsCleaned = new Dictionary<string, string[]>();
    public static string Sugestion = "";
    public static string Queri = "";
    public static SearchResult Query(string query)
    {
        // Modifique este método para responder a la búsqueda
        Stopwatch timeMeasure = new Stopwatch();
        Queri = query;
        
        Modifiers = Indexing.Modifiers(query);
        string[] tempQuery = Indexing.CleaninQuery(query);
        tempQuery = MoogleMath.NewQuery(DictionaryOfWords, tempQuery, MainStructure);
        VectModel = VectorialModel.MaindMethodOfVectorialModel(query, tempQuery, MainStructure, IDF, WeightValueWords);
        VectModel = VectorialModel.ResolveModifiers(Modifiers, MainStructure, VectModel, TextsCleaned);
        tempQuery = MoogleMath.SynonymsResults(SynonymousDictionary, tempQuery, DictionaryOfWords);
        VectModel = MoogleMath.SynonymScore(VectModel, tempQuery);


        SearchItem[] items = new SearchItem[10];

        List<Tuple<double, string>> score = new List<Tuple<double, string>>();
        int cont = 0;

        foreach (string document in VectModel.Keys)
        {
            Tuple<double, string> temporalVar = new Tuple<double, string>(VectModel[document], document);
            score.Add(temporalVar);
            //Console.WriteLine(temporalVar.Item1 + " " + temporalVar.Item2);
        }

        score.Sort();
        score.Reverse();
        Tuple<double, string>[] new_score = new Tuple<double, string>[10];
        score.CopyTo(0, new_score, 0, 10);

        for (int i = 0; i < new_score.Length; i++)
        {
            string temp = new_score[i].Item2;
            string topic = new_score[i].Item2.Substring(42);
            topic=topic.Substring(0, topic.Length-3);
            items[cont]=new SearchItem(topic, MoogleMath.RealSnipet(TextsCleaned[new_score[i].Item2]), new_score[i].Item1);
            cont++;
        }

        return new SearchResult(items, Sugestion);
    }

    public static void Awake()
    {
        Tuple<Dictionary<string, Dictionary<string, double>>, Dictionary<string, double>> theTuple = Indexing.DocumentsDictionary();
        MainStructure = theTuple.Item1;
        DictionaryOfWords = theTuple.Item2;
        IDF = VectorialModel.InverseDocumentFrequency(MainStructure, DictionaryOfWords);
        TF = VectorialModel.NormalizedFrequency(IDF, MainStructure);
        WeightValueWords = VectorialModel.WeightValueWord(TF, IDF);
        string dotJsonSyn = File.ReadAllText("..//Synonymous\\Synonymous.json");
        SynonymousDictionary = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(dotJsonSyn)!;

    }

}
