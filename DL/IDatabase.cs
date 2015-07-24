using System;
using System.Collections.Generic;
using Actioncards.BL;

namespace Actioncards.DL
{
	public interface IDatabase
	{
		Category GetCategoryById(string id);
		Category GetCategory(string title);
		Category GetCategory(Category cat);
		IEnumerable<Category> GetCategories();
		void AddCategory(Category cat);
		Actioncard GetActionCardById(string id);
		Actioncard GetActionCard(string title);
		void deleteActionCard (Actioncard card);
		IEnumerable<Actioncard> GetActionCards ();
		void AddActionCard(Actioncard card);
		void ClearCategoryTable ();
		void ClearActionCardsTable ();
		int AddNote (Note note);
		void RemoveNote (Note note);
		IEnumerable<Note> GetNotes ();
	}
}

