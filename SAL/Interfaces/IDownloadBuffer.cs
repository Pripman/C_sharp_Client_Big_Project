using System;
using Actioncards.DAL;
using Actioncards.BL;
using System.Collections.Generic;

namespace Actioncards.SAL
{
	public interface IDownloadBuffer
	{
		List<CardListElement> cardList{ get; set;}
		void addBufferToDatabase();
		void AddCardToBuffer (Actioncard card);
		void AddCategoriesToBuffer (List<Category> cats);
		bool CardIsInBuffer(string id);
		void ResetDownloadBuffer();

	}
}

