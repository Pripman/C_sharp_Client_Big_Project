using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Globalization;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Actioncards.BL;
using Actioncards;

using Actioncards.DAL;

namespace Actioncards.SAL
{
	public class Deserializer:IDeserializer
	{

		public double DeserializeAppVersion (string onlineVersion)
		{
			return double.Parse(onlineVersion, CultureInfo.InvariantCulture);
		}

		public Actioncard DeserializeActionCard (string Json)
		{
			Debug.WriteLine (string.Format("Trying to deserialize actioncard string: {0}", Json));
			Actioncard card = null;
			var obj = JObject.Parse(Json);
			var title = makeProperty ("title", obj);
			var stitle = makeProperty ("stitle", obj);
			var cid = makeProperty ("cid", obj);
			var meta = makeProperty ("meta", obj);
			var version = makeProperty ("version", obj);
			var id = makeProperty ("_id", obj);
			var date = makeProperty ("date", obj);
			card = HelperMethods.MakeActioncard(title, stitle, cid, meta, version, id, date); 
			Debug.WriteLine ("Card was deserialized: " + card.ID);			
			return card;
		}

		public List<Category> DeserializeCategories(string Json)
		{
			Debug.WriteLine ("Trying to deserialize categories string: {0}", Json);
			List<Category> cats = null;
			var objs = JArray.Parse(Json);
			cats = new List<Category>();
			foreach(JObject obj in objs)
			{
				var title = makeProperty ("title", obj);
				var cid = makeProperty ("_id", obj);
				var c = makeProperty ("color", obj);
				var color = int.Parse(c, System.Globalization.NumberStyles.HexNumber);	
				cats.Add(HelperMethods.MakeCategory(title, cid, color, true));
			} 

			return cats;
			
		}

		public List<CardListElement> DeserializeCardList(string Json)
		{
			Debug.WriteLine ("Trying to deserialize cardlist string: {0}", Json);
			List<CardListElement> elements = null;

			var objs = JArray.Parse (Json);
			elements = new List<CardListElement> ();
			foreach(JObject obj in objs)
			{
				var version = makeProperty ("version", obj);
				var id = makeProperty ("_id", obj);
				elements.Add(HelperMethods.MakeListElement(id, version));
			}

			return elements;
		}


		private string makeProperty(string property, JObject obj)
		{
			var value = (string)obj[property];
			if (value == null) {
				HelperMethods.ReportError("ERROR deserializing json property: " + property);
				throw new System.NullReferenceException("ERROR deserializing json property: " + property);
			}
			return value;
		}
	}
}

