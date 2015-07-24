using System;
using Actioncards.DAL;
using Actioncards.SAL;
using Actioncards.DL;
using Actioncards.BL;
using Actioncards.CardParser;

namespace Actioncards.Instantiater
{
	public interface IServiceProvider
	{
		IDataManager GetDataManager ();
		IOnlineManager GetOnlineManager ();
		ITokenGenerator GetTokenGenerator ();
		
	}
}

