using System;
//using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Actioncards.BL;
using Actioncards.DL;
using System.Linq;

namespace Actioncards.DAL
{
	public class DataManager:IDataManager
	{
		private IDatabase _db { get; set; }

		public DataManager(IDatabase db)
		{
			this._db = db;
		}

		public void AddCategory(Category cat)
		{
				_db.AddCategory (cat);
		}

		public void DeleteAndAddCats(List<Category> newCats)
		{
			ClearCategoryTable ();
			AddCategories (newCats);
		}

		public void AddCategories(List<Category> cats)
		{
			foreach (Category cat in cats)
				AddCategory (cat);
		}
			
		public Category GetCategoryById(string id)
		{
			return _db.GetCategoryById (id);
		}

		public IEnumerable<Category> GetCategories()
		{
			return _db.GetCategories ().OrderBy(x => x.Title);
		}

		public List<Category> GetRootCats()
		{
			return GetCategories().Where(x => x.IsRoot == true).ToList();
		}

		public Category GetCategory(string title)
		{
			return _db.GetCategory (title);
		}

		public void AddActionCard(Actioncard card)
		{
			_db.AddActionCard (card);
		}

		public void deleteActionCard (Actioncard card)
		{
			_db.deleteActionCard (card);
		}
			
		public Actioncard GetActionCardById(string id)
		{
			return _db.GetActionCardById (id);
		}

		public IEnumerable<Actioncard> GetActionCards()
		{
			return _db.GetActionCards().OrderBy(x => x.Title);
		}

		public Actioncard GetActionCard(string title)
		{
			return _db.GetActionCard (title);
		}

		public void ClearCategoryTable ()
		{
			_db.ClearCategoryTable ();
		}

		public void ClearActionCardsTable ()
		{
			_db.ClearActionCardsTable ();
		}

		public string GetImage(string filename)
		{
			var documentsPath = Constants.ImageFolderPath;
			var path = System.IO.Path.Combine (documentsPath, filename);
			if (System.IO.File.Exists (path))
				return path;
			else 
				return null;
		}

		public void ClearImageFolder()
		{
			foreach (var img in Directory.GetFiles(Constants.ImageFolderPath)) 
			{
				File.SetAttributes(img, FileAttributes.Normal);
				File.Delete(img);
			}
		}

		public IEnumerable<IBusinessEntity> GetAllItems(){
			var items = new List<IBusinessEntity> ();
			items.AddRange (GetCategories ());
			items.AddRange (GetActionCards ());
			return items.OrderBy(x => x.Title);
		}

		public void AddBookmark (Actioncard card)
		{
			card.Bookmark = true;
			 _db.AddActionCard (card);
		}

		public void RemoveBookmark (Actioncard card)
		{
			card.Bookmark = false;
			_db.AddActionCard (card);		
		}

		public void AddNote(Note note){
			var noteid = _db.AddNote (note);
			HelperMethods.ReportInfo(string.Format("Note added to card{0}, with nodeid:{1}", note.ID, noteid));
		}

		public void RemoveNote (Note note)
		{
			_db.RemoveNote (note);
		}

		public IEnumerable<Note> GetNotes ()
		{
			return _db.GetNotes ();
		}


	}
}

