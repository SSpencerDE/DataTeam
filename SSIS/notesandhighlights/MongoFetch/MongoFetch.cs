using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Bson;
using System.Reflection;
namespace MongoFetch
{
	public class MongoFetch
	{
		private string StartDatabase;
		private string ServerAddress;
		private int Port;
		private string Username;
		private string Password;
		private IAsyncCursor<BsonDocument> Cursor;
		private Queue<BsonDocument> CursorQueue;
		public bool IsCursorOpen { get; set; }

		private List<MongoCredential> Credentials
		{
			get
			{
				List<MongoCredential> creds = new List<MongoCredential>();
				MongoCredential c = MongoCredential.CreateCredential(this.StartDatabase, this.Username, this.Password);
				creds.Add(c);
				return creds;
			}
		}
		private MongoClientSettings ClientSettings
		{
			get
			{
				MongoClientSettings settings = new MongoClientSettings();

				settings.ReadPreference = ReadPreference.SecondaryPreferred;
				settings.Server = new MongoServerAddress(this.ServerAddress, this.Port);
				settings.ConnectionMode = ConnectionMode.Direct;
				settings.MaxConnectionLifeTime = new TimeSpan(1, 0, 0, 0);
				settings.Credentials = this.Credentials;
				
				return settings;
			}
		}

		private MongoClient Client;

		public MongoFetch(string startDatabase, string serveraddress, int port, string username, string password)
		{
			this.StartDatabase = startDatabase;
			this.ServerAddress = serveraddress;
			this.Port = port;
			this.Username = username;
			this.Password = password;
		}

		public void CreateCursor(string databaseName, string collectionName, int limit, string startObjectID, int batchSize = 1000)
		{
			if (this.Client == null)
			{
				this.Client = new MongoClient(this.ClientSettings);
			}

			var database = this.Client.GetDatabase(databaseName);
			var collection = database.GetCollection<BsonDocument>(collectionName);
			var filter = Builders<BsonDocument>.Filter.Gt("_id", new ObjectId(startObjectID));
			var options = new FindOptions { BatchSize = batchSize };
			this.Cursor = collection.Find(filter, options).Limit(limit).ToCursor();
			this.CursorQueue = new Queue<BsonDocument>();
			this.IsCursorOpen = true;
			this.QueueBatch();
		}

		public BsonDocument GetNextDocument()
		{
			BsonDocument result = null;
			if (!this.IsCursorOpen)
			{
				throw new InvalidOperationException("There is not currently an open cursor so we cannot fetch a document for you.");
			}

			result = this.CursorQueue.Dequeue();

			if (this.CursorQueue.Count == 0)
				this.QueueBatch();

			return result;
		}

		public void MapDocument(BsonDocument sourceDocument, object o)
		{
			Dictionary<string, object> d = sourceDocument.ToDictionary();
			var properties = o.GetType().GetProperties();

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
					SetValue(ref o, property, d[property.Name]);
				}
				else
				{
					MissingFields.Add(property.Name);
				}
			}

			if (MissingFieldProperty != null)
			{
				MissingFieldProperty.SetValue(o, MissingFields);
			}
		}

		public void CloseCursor()
		{
			this.Cursor.Dispose();
			this.IsCursorOpen = false;
		}

		private void QueueBatch()
		{
			if (this.CursorQueue.Count > 0)
			{
				return;
			}

			if (this.Cursor.MoveNext())
			{
				if (this.Cursor.Current.Count() == 0)
				{
					this.CloseCursor();
					return;
				}
				else
				{
					foreach (var document in Cursor.Current)
						this.CursorQueue.Enqueue(document);
				}
			}
			else
			{
				this.CloseCursor();
			}
		}


		private static void SetValue(ref object o, PropertyInfo property, object value)
		{
			object v;
			Type PType = property.PropertyType;
			switch (property.PropertyType.Name.ToLower())
			{
				case "int32":
					int i = -1;
					int.TryParse(value.ToString(), out i);
					v = i;
					break;
				case "guid":
					v = new Guid(value.ToString());
					break;
				case "string":
					v = value.ToString();
					break;
				default:
					v = value;
					break;
			}
			property.SetValue(o, v, null);
		}
	}
}
