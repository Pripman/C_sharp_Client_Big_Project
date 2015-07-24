//using System;
using System.IO;
using Actioncards.Instantiater;

namespace Actioncards
{
	public class Constants
	{
		private static string _databaseId;
		public static string DatabaseId{get{return _databaseId; } 
			set{
				_databaseId = value;
				assignUrls ();
			}
		}

		public static string BaseUrl;
		public static string ActionCardUrl;
		public static string CategoriesUrl;
		public static string CardListUrl;
		public static string ImageUrl;
		public static string ImageFolderPath;
		public static string ErrorLogUrl;
		public static string InfoLogUrl;
		public static string VersionUrl;
		public static string FeedbackUrl;
		public static string AdBlockId;
		public static double AppVersion;



		private static void assignUrls(){

			//resetSQLLite DB
			var _db = ServiceProvider.Instanceses.GetDataManager ();
//			_db.ClearActionCardsTable ();
//			_db.ClearCategoryTable ();

			//if run in IOS
			//BaseUrl = "http://localhost:5005";
			//if run in android
			//BaseUrl = "http://10.0.3.2:5005";
			//if run in in production
			BaseUrl = "http://radubyte.com";
			//if run in on local machine(use the local machines ip-address)
//			BaseUrl = "http://192.168.87.100:5005";

			//Peters
			//BaseUrl = "http://192.168.87.109:9000";

			ActionCardUrl = BaseUrl + "/api/db/" + _databaseId + "/cards/";
			CategoriesUrl = BaseUrl + "/api/db/" + _databaseId + "/categories";
			CardListUrl = BaseUrl + "/api/db/" + _databaseId + "/cardlist";
			ErrorLogUrl = BaseUrl + "/api" + "/logs/ERROR";
			InfoLogUrl = BaseUrl + "/api" + "/logs/INFO";
			ImageUrl = BaseUrl + "/api/img/";
			VersionUrl = BaseUrl + "/api/version";
			FeedbackUrl = BaseUrl + "/api/db/" + _databaseId + "/feedback";

			AdBlockId = "ca-app-pub-2786575588931954/9798387427";
			AppVersion = 0.9;
		}


		public static bool ProductionMode = false;
	}
}



