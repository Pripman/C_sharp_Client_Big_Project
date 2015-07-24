//using System;
//using System.Collections.Generic;
//using HtmlAgilityPack;
//using Actioncards.BL;
//using System.Linq;
//
//namespace Actioncards.CardParser
//{
//	public class HtmlTools
//	{
//		public Actioncard card { get; private set; }
//		public List<CardElement> CardSections { get; private set;}
//		public List<CardElement> Cardelements { get; private set;}
//		HtmlDocument doc;
//
//		public HtmlTools (Actioncard card)
//		{
//			this.card = card;
//			doc = new HtmlDocument ();
//			doc.LoadHtml (card.meta);
//		}
//			
//		public List<CardElement> findAndAddSections ()
//		{
//			CardSections = new List<CardElement> ();
//			foreach (var elem in doc.DocumentNode.Descendants()) {
//				if (elem.Name == "h3") {
//					var uid = Guid.NewGuid ().ToString ();
//					var id = elem.SetAttributeValue("id", uid);
//					CardSections.Add (new CardElement (elem.InnerHtml, uid));
//				}
//			}
//			card.meta = doc.DocumentNode.OuterHtml; 
//			return CardSections;
//		}
//
//		public List<CardElement> FindAndAddAllElemtents(){
//			foreach (var elem in doc.DocumentNode.Descendants()) {
//				var uid = Guid.NewGuid ().ToString ();
//				var id = elem.SetAttributeValue("id", uid);
//				Cardelements.Add(new CardElement(elem.InnerHtml, uid){extraText = elem.ParentNode.InnerHtml});
//			}
//			card.meta = doc.DocumentNode.OuterHtml;
//			return Cardelements;
//		
//		}
//
//		public List<CardElement> SectionsContainsSearchString(string text){
//			var query = text.ToLower ();
//			if (CardSections == null)
//				findAndAddSections ();
//			return CardSections.Where(x => x.text.Contains(query)).ToList();
//		}
//
//		public List<CardElement> CardContainsSearchString(string text){
//			var query = text.ToLower ();
//			if (CardSections == null)
//				FindAndAddAllElemtents ();
//			return Cardelements.Where (x => x.text.Contains (query)).ToList();
//		}
//			
//	}
//
//	public class CardElement{
//		public string text{ get; set;}
//		public string uid{ get; set;}
//		public string extraText{ get; set;}
//
//		public CardElement(string text, string uid){
//			this.text = text;
//			this.uid = uid;
//		}
//	}
//}
//
