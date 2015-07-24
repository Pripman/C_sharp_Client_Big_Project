using System;
using Actioncards.BL;
using System.Collections.Generic;
using Actioncards.DAL;
using Actioncards.DL;
using Actioncards.DL.SQLite;
using HtmlAgilityPack;
using System.Linq;

namespace Actioncards.BL
{
	public class Actioncard : IBusinessEntity, ISearchable
	{
		[PrimaryKey]
		public string ID {get; set;}
		public string ParentCatID { get; set; }
		public string Title { get; set; }
		public string Stitle { get; set;}
		public string meta { get; set; }
		public string Version{ get; set; }
		public DateTime TimeStamp{ get; set;}
		public bool Bookmark { get; set;}
		public void AddObjectToDatabase(string parentID, IDatabase db)
		{
			this.ParentCatID = parentID;
			db.AddActionCard (this);
		}

		string IBusinessEntity.GetType ()
		{
			return "Card";
		}

		[Ignore]
		public List<CardElement> CardSections { get; private set;}
		[Ignore]
		public List<CardElement> Cardelements { get; private set;}
		HtmlDocument doc;
		List<string> cardDetailedElements;
		List<string> cardSectionElements;

		public Actioncard ()
		{
			this.meta = meta;
			doc = new HtmlDocument ();
			cardDetailedElements = new List<string>{ "p", "li" };
			cardSectionElements = new List<string>{ "h4", "h3", "h2", "h1" };
		}

		public List<CardElement> findAndAddSections ()
		{
			if (CardSections != null)
				return CardSections;
			doc.LoadHtml (meta);
			CardSections = new List<CardElement> ();
			foreach (var elem in doc.DocumentNode.Descendants()) {
				if (cardSectionElements.Any(x => x == elem.Name)) {
					var uid = Guid.NewGuid ().ToString ();
					elem.SetAttributeValue("id", uid);
					CardSections.Add (new CardElement (elem.InnerHtml, uid, this, elem.Name));
				}
			}
			meta = doc.DocumentNode.OuterHtml; 
			return CardSections;
		}

		public List<CardElement> FindAndAddAllElemtents(){
			doc.LoadHtml (meta);
			Cardelements = new List<CardElement> ();
			foreach (var elem in doc.DocumentNode.Descendants()) {
				if (cardDetailedElements.Any (x => x == elem.Name)) {
					var uid = Guid.NewGuid ().ToString ();
					elem.SetAttributeValue ("id", uid);
					Cardelements.Add (new CardElement (elem.InnerHtml, uid, this, elem.Name));
				}
			}
			meta = doc.DocumentNode.OuterHtml;
			return Cardelements;

		}

		public List<CardElement> SectionsContainsSearchString(string text){
			var query = text.ToLower ();
			if (CardSections == null)
				findAndAddSections ();
			return CardSections.Where(x => x.Title.ToLower().Contains(query)).ToList();
		}

		public List<CardElement> CardContainsSearchString(string text){
			var query = text.ToLower ();
			if (Cardelements == null)
				FindAndAddAllElemtents ();
			return Cardelements.Where (x => x.Title.Contains (query)).ToList();
		}

	}

	public class CardElement:ISearchable{
		public Actioncard Card { get;  set;} 
		public string Title{ get; set;}
		public string ID{ get; set;}
		public string ExtraText{ get; set;}
		public string ElementType{ get; set;}

		public CardElement(string title, string uid, Actioncard card, string elementType){
			this.Title = title;
			this.ID = uid;
			this.Card = card;
			this.ElementType = elementType;
		}

	}
}

