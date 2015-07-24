using System;
using Actioncards.DAL;
using Actioncards.SAL;
using Actioncards.DL;
using Actioncards.BL;
using Actioncards.CardParser;


namespace Actioncards.Instantiater
{
	public class ServiceProvider:IServiceProvider
	{
		private IDataManager _dm;
		private IOnlineManager _om;
		private IDatabase _db;
		private ITokenGenerator _tg;


		public static ServiceProvider Instanceses{ get; private set;}

		static ServiceProvider ()
		{
			Instanceses = new ServiceProvider ();
			Instanceses._db = Database.me;
			Instanceses._dm = new DataManager (Instanceses._db);
			Instanceses._om = new OnlineManager (
				new DownloadBuffer(Instanceses._dm), 
				Instanceses._dm, 
				new Deserializer(), 
				new WebClientMaker());
			Instanceses._tg = new HtmlParser ();
		}


		public IDataManager GetDataManager()
		{
			return Instanceses._dm;
		}

		public IOnlineManager GetOnlineManager()
		{
			return Instanceses._om;
		}

		public ITokenGenerator GetTokenGenerator ()
		{
			return Instanceses._tg;
		}


	}
}

