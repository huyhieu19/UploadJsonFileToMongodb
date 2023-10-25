using MongoDB.Bson;
using MongoDB.Driver;

var connectionString = "mongodb+srv://<username>:<password>@cluster0.htpyu9l.mongodb.net/?retryWrites=true&w=majority";

var client = new MongoClient(connectionString);
var database = client.GetDatabase("test");

string text = System.IO.File.ReadAllText(@"..\..\..\dataDict.json");

IEnumerable<ListData>? listData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ListData>>(text);

// Now you have your JSON data deserialized into a list of ListData objects.
//foreach (var item in listData)
//{
//    Console.WriteLine($"Word: {item.word}, Spell: {item.spell}");
//    foreach (var dataItem in item.data)
//    {
//        Console.WriteLine($"Part of Speech: {dataItem.partOfSpeech}");
//        foreach (var meaningItem in dataItem.meanings)
//        {
//            Console.WriteLine($"Definition: {meaningItem.definition}");
//            foreach (var detail in meaningItem.details)
//            {
//                Console.WriteLine($"Details: {detail}");
//            }
//        }
//    }
//}
var collection = database.GetCollection<BsonDocument>("ditionaryvien_models");

IEnumerable<BsonDocument> listInsert = Ok.ConvertListDataToBson(listData!);

await collection.InsertManyAsync(listInsert);

Console.WriteLine("Done");

public class Ok
{
    public static IEnumerable<BsonDocument> ConvertListDataToBson(IEnumerable<ListData> listData)
    {
        var bsonDocuments = new List<BsonDocument>();

        foreach (var dataItem in listData)
        {
            var bsonDoc = new BsonDocument
        {
            { "word", dataItem.word },
            { "spell", dataItem.spell },
            { "data", new BsonArray(dataItem.data.Select(d => new BsonDocument
                {
                    { "partOfSpeech", d.partOfSpeech },
                    { "meanings", new BsonArray(d.meanings.Select(m => new BsonDocument
                        {
                            { "definition", m.definition },
                            { "details", new BsonArray(m.details.Select(detail => new BsonDocument("detail", detail))) }
                        }))
                    }
                }))
            }
        };

            bsonDocuments.Add(bsonDoc);
        }

        return bsonDocuments;
    }
}

public class ListData
{
    public string word { get; set; }
    public string spell { get; set; }
    public List<datalist> data { get; set; }

}
public class datalist
{
    public string partOfSpeech { get; set; }
    public List<datalist2> meanings { get; set; }

}
public class datalist2
{
    public string definition { get; set; }
    public List<string> details { get; set; }

}