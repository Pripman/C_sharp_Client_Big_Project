using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace Actioncards.CardParser
{
	public class HtmlParser:ITokenGenerator
	{
		private List<Token> _cardItems;

		public HtmlParser ()
		{

		}
			
		public List<Token> ScanMetaText (string metaText)
		{
			_cardItems = new List<Token> ();

			var doc = new HtmlDocument ();
			doc.LoadHtml (metaText);
			foreach (var elem in doc.DocumentNode.Descendants()) {
				CheckTypeAndMakeToken (elem);
			}
			return _cardItems;
		}

		private void CheckTypeAndMakeToken(HtmlNode elem){
			switch (elem.Name) {
			case "p":
				{
					var data = elem.InnerHtml;
					var dataWithLineBreak = data.Replace ("<br>", "\n");

					MakeToken (TokenType.TEXT, dataWithLineBreak);
//					foreach (var child in elem.Descendants())
//						if (child.Name == "br")
//							MakeToken (TokenType.TEXT, "\n" );
					break;
				}
			case "h3":
				MakeToken (TokenType.SECTION, elem.InnerHtml);
				break;
			case "ul":
				{
					foreach (var li in elem.Descendants())
						if (li.Name == "li")
							MakeToken (TokenType.ITEM, li.InnerHtml);
				}
				break;
			case "img":
				MakeToken (TokenType.IMG, elem.InnerHtml);
				break;
			case "a":
				{
					var hrefString = elem.Attributes ["href"].Value;
					if (hrefString.StartsWith ("tel"))
						MakeToken (TokenType.PHONENR, hrefString);
					else if (hrefString.StartsWith ("http"))
						MakeToken (TokenType.LINK, hrefString);
					else
						HelperMethods.ReportInfo ("A link (<a>) has no valid href, the value was: " + hrefString);
				
				}
				break;
			}
			
		}

		private void MakeToken(TokenType type, string cardData)
		{
			Token t = new Token (type);
			t.Data = cardData;
			_cardItems.Add (t);
		}
	}
}

