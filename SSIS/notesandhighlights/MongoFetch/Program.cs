using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Reflection;

namespace MongoFetch
{
	class Program
	{
		static void Main(string[] args)
		{
			var Fetcher = new MongoFetch("admin", "192.168.203.66", 27017, "dbteam", "ff81eced92e75efdf976");
			Fetcher.CreateCursor("techbook2", "highlight", 100, "59FC810A0000000000000000", 10);
			while (Fetcher.IsCursorOpen)
			{
				Highlight h = new Highlight();
				BsonDocument d = Fetcher.GetNextDocument();
				Fetcher.MapDocument(d, h);
				Console.WriteLine(h._id);
			}

			//Console.WriteLine(Fetcher.GetNextDocument().ToString());

			/*
			List<MongoCredential> credentials = new List<MongoCredential>();

			MongoCredential credential = MongoCredential.CreateCredential("admin", "dbteam", "ff81eced92e75efdf976");
			credentials.Add(credential);

			MongoClientSettings settings = new MongoClientSettings();
			settings.ReadPreference = ReadPreference.SecondaryPreferred;
			settings.Server = new MongoServerAddress("192.168.203.66", 27017);
			settings.ConnectionMode = ConnectionMode.Direct;
			settings.MaxConnectionLifeTime = new TimeSpan(1, 0, 0, 0);
			settings.Credentials = credentials;
			
			MongoClient client = new MongoClient(settings);
			
			IMongoDatabase db = client.GetDatabase("techbook2");
			IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>("highlight");
			//IMongoCollection<Highlight> collection = db.GetCollection<Highlight>("highlight");

			var filter = Builders<BsonDocument>.Filter.Empty;
			//var filter = Builders<BsonDocument>.Filter.Gte("_id", new ObjectId("50e43b850cf2128b86eab979"));
			//var filter = Builders<BsonDocument>.Filter.Gte("_id", new ObjectId("50e43b850cf2128b86eab97a"));

			var options = new FindOptions
			{
				
			};

			using (var cursor = collection.Find(filter, options).Limit(100).ToCursor())
			{
				while (cursor.MoveNext())
				{
					foreach (var doc in cursor.Current)
					{
						var o = BsonDocumentToObject<Highlight>(doc);
						Console.WriteLine(o._id);
						Console.WriteLine(o.createdOn);
						//Console.WriteLine(o.MissingFields == null ? "" : string.Join(", ", o.MissingFields));
						//string v = doc.ToString();
						//Console.WriteLine(doc.ToString());
						//Console.WriteLine(doc._id);
					}
				}
			}

			collection = db.GetCollection<BsonDocument>("note");
			//IMongoCollection<Highlight> collection = db.GetCollection<Highlight>("highlight");

			filter = new BsonDocument();
			options = new FindOptions
			{
			};

			using (var cursor = collection.Find(filter, options).Limit(100).ToCursor())
			{
				while (cursor.MoveNext())
				{
					foreach (var doc in cursor.Current)
					{
						var o = BsonDocumentToObject<Note>(doc);
						Console.WriteLine(o._id);
						Console.WriteLine(o.MissingFields == null ? "" : string.Join(", ", o.MissingFields));
						//string v = doc.ToString();
						//Console.WriteLine(doc.ToString());
						//Console.WriteLine(doc._id);
					}
				}
			}

	*/
			Console.ReadLine();
		}

		private static T BsonDocumentToObject<T>(BsonDocument doc) where T : new()
		{
			Dictionary<string, object> d = doc.ToDictionary();
			var t = new T();
			var properties = t.GetType().GetProperties();
			List<string> MissingFields = new List<string>();
			PropertyInfo MissingFieldProperty = null;
			foreach (PropertyInfo property in properties)
			{
				if (string.Compare("MissingFields", property.Name, true) == 0)
				{
					MissingFieldProperty = property;
				}
				else if (d.ContainsKey(property.Name))
				{
					SetValue<T>(ref t, property, doc[property.Name]);
				}
				else
				{
					MissingFields.Add(property.Name);
				}
			}
			
			if (MissingFieldProperty != null)
			{
				MissingFieldProperty.SetValue(t, MissingFields);
			}

			return t;
		}
		private static void SetValue<T>(ref T o, PropertyInfo property, object value)
		{
			object v;

			switch (property.PropertyType.Name.ToLower()) 
			{
				case "int32":
					v = int.Parse(value.ToString());
					break;
				case "string":
					v = value.ToString();
					break;
				case "guid":
					v = new Guid(value.ToString());
					break;				
				default:
					v = value;
					break;
			}
			property.SetValue(o, v, null);
		}
	}
}
