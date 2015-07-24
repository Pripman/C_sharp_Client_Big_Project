using System;
using Actioncards.BL;
using System.Diagnostics;
using Actioncards.SAL;
using System.Globalization;
using HtmlAgilityPack;
using System.IO;
using HtmlAgilityPack;

using Actioncards.Instantiater;


namespace Actioncards
{
	public class HelperMethods
	{
		public static Actioncard MakeActioncard(string title, string stitle, string parent, string meta, string version, string id, string date)
		{
			Actioncard card = new Actioncard();
			card.ID = id;
			card.Title = title;
			card.Stitle = stitle;
			card.ParentCatID = parent;
			card.meta = meta;
			card.Version = version;
			card.TimeStamp = DateTime.Parse(date, CultureInfo.InvariantCulture);
			return card;
		}

		public static Category MakeCategory(String title, string catId, int color, bool isRoot = false)
		{
			Category cat = new Category ();
			cat.Title = title;
			cat.ID = catId;
			cat.Color = color;
			cat.IsRoot = isRoot;
			return cat;
		}

		public static CardListElement MakeListElement(string id, string version)
		{
			var element = new CardListElement ();
			element.Id = id;
			element.Version = version;
			return element;
		}
			

		public static void ReportError(string msg)
		{
			Report (msg, Constants.ErrorLogUrl);	
		}

		public static void ReportInfo(string msg)
		{
			Report (msg, Constants.InfoLogUrl);	
		}

		public static void Report(string msg, string url)
		{
			if (Constants.ProductionMode) {
				IOnlineManager _om = ServiceProvider.Instanceses.GetOnlineManager ();
				_om.reportToLog (msg, url);
			} 
			else 
			{
				Console.WriteLine (msg);
			}
		}

		public static string GetRandomCacheParameter()
		{
			var random = new Random ();
			var num = random.Next ().ToString ();
			return "?cache=" + num;
		}
	}
}

