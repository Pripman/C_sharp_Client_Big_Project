using System;
using Actioncards.BL;
using System.Collections.Generic;

namespace Actioncards.DAL
{
	public interface IDataManager
	{
		void AddCategory(Category cat);
		void AddCategories (List<Category> cats);
		void DeleteAndAddCats (List<Category> newCats);
		Category GetCategoryById(string id);
		IEnumerable<Category> GetCategories();
		List<Category> GetRootCats ();
		Category GetCategory (string title);
		void AddActionCard (Actioncard card);
		void deleteActionCard (Actioncard card);
		Actioncard GetActionCardById (string id);
		Actioncard GetActionCard(string title);
		IEnumerable<Actioncard> GetActionCards();
		void ClearCategoryTable ();
		void ClearActionCardsTable ();
		String GetImage (string filename);
		void ClearImageFolder();
		IEnumerable<IBusinessEntity> GetAllItems ();
		void AddBookmark (Actioncard card);
		void RemoveBookmark (Actioncard card);
		void AddNote (Note note);
		void RemoveNote (Note note);
		IEnumerable<Note> GetNotes ();

	}
}

