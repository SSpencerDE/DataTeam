using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoFetch
{
	public class Highlight
	{
		public ObjectId _id { get; set; }
		public Guid guidConceptId { get; set; }
		public Guid guidAssetId { get; set; }
		public string node1 { get; set; }
		public string node2 { get; set; }
		public int offset1 { get; set; }
		public int offset2 { get; set; }
		public string color { get; set; }
		public int page { get; set; }
		public string selectedText { get; set; }
		public Guid username { get; set; }
		public DateTime createdOn { get; set; }
		public DateTime updatedOn { get; set; }
		public int _version { get; set; }
		public Guid courseGuid { get; set; }
		public Guid subdisciplineGuid { get; set; }
		public Guid unitGuid { get; set; }

		public Highlight()
		{

		}

		public List<string> MissingFields { get; set; }
		
	}
}
