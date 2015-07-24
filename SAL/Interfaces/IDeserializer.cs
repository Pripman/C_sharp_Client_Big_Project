using System;
using Actioncards.BL;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace Actioncards.SAL
{
	public interface IDeserializer
	{
		Actioncard DeserializeActionCard (string Json);
		List<Category> DeserializeCategories(string Json);
		List<CardListElement> DeserializeCardList (string Json);
		double DeserializeAppVersion (string Json);
	}
}

