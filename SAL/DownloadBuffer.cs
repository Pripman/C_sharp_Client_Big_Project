using System;
using System.Diagnostics;
using System.Linq;
using Actioncards.BL;
using Actioncards.DAL;

using System.Collections.Generic;

namespace Actioncards.SAL
{
	public class DownloadBuffer:IDownloadBuffer
	{
		public List<CardListElement> cardList{ get; set;}

		private List<Category> _catsBuffer;
		private List<Actioncard> _cardBuffer;
		private IDataManager _dm;

		public DownloadBuffer (IDataManager dm)
		{
			_dm = dm;	
			_cardBuffer = new List<Actioncard> ();
			_catsBuffer = new List<Category> ();
		}

		public void ClearBuffer(){
			_cardBuffer.Clear ();
			_catsBuffer.Clear ();
		}

		public void addBufferToDatabase()
		{
			foreach (var card in _cardBuffer) {
				_dm.AddActionCard (card);
			}
			_dm.DeleteAndAddCats(_catsBuffer);
			RemoveCardsNotInCardList ();
		}

		public void AddCardToBuffer (Actioncard card)
		{
			_cardBuffer.Add(card);
		}

		public void AddCategoriesToBuffer (List<Category> cats)
		{
			_catsBuffer = cats;
		}

		public bool CardIsInBuffer(string id)
		{
			foreach (var card in _cardBuffer) 
				if (card.ID == id)
					return true;
			return false;
		}

		public void ResetDownloadBuffer()
		{
			_cardBuffer = new List<Actioncard> ();
			_catsBuffer = new List<Category> ();
		}

		private void RemoveCardsNotInCardList()
		{

			List<string> cardsToBeDeleted = new List<string> ();
			foreach (var card in _dm.GetActionCards()) 
			{
				if (cardList.Any (x => x.Id == card.ID) == false)
					cardsToBeDeleted.Add (card.ID);
			}

			foreach (var cardId in cardsToBeDeleted){
				var card = _dm.GetActionCardById (cardId);
				_dm.deleteActionCard (card);
			}
		}
	}
}

