using System;
using System.Collections.Generic;
using System.Linq;
using Actioncards.DL.SQLite;
using Actioncards.BL;
using System.IO;
using System.Diagnostics;
using Actioncards.SAL;



namespace Actioncards.DL
{
	public class Database : SQLiteConnection, IDatabase
	{
		public static Database me = null;
		protected static string dbLocation;
		static object locker = new object ();

		/// Initializes a new instance of the <see cref="SQLite.DL.ActioncardDatabase"/> ActioncardDatabase. 
		/// if the database doesn't exist, it will create the database and all the tables.

		protected Database (string path) : base (path)
		{
			// create the tables

			CreateTable<Category> ();
			CreateTable<Actioncard> ();
			CreateTable<Note> ();
		}
			

		static Database ()
		{
			// set the db location
			dbLocation = DatabaseFilePath;

			// instantiate a new db
			me = new Database (dbLocation);
		}



		public static string DatabaseFilePath 
		{
			get { 
				#if SILVERLIGHT
				var path = "ActioncardDB.db3";
				#else

				#if __ANDROID__
				string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
				#else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "../Library/");
				#endif
				var path = Path.Combine (libraryPath, "ActioncardDB.db3");
				#endif		
				return path;	
			}
		}
		#region CRUD 


		public static T GetItem<T> (string id) where T : BL.IBusinessEntity, new()
		{
			lock (locker) 
			{
				return me.Table<T> ().FirstOrDefault (x => x.ID == id);
			}
		}

		public static IEnumerable<T> GetItems<T> () where T : BL.IBusinessEntity, new()
		{
			lock (locker) 
			{
				return (from i in me.Table<T> ()
					select i).ToList (); 
			}
		}

		public static int SaveItem<T> (T item) where T : BL.IBusinessEntity, new()
		{


				lock (locker) 
				{
					if (me.Table<T>().FirstOrDefault(x => x.ID == item.ID) != null) 
					{
						return me.Update (item);
						 
					}
					else 
						return me.Insert (item);
				}
		}

		public static void SaveItems<T> (IEnumerable<T> items) where T : BL.IBusinessEntity, new()
		{
			lock (locker)
			{
				me.BeginTransaction ();
				foreach (T item in items)
					SaveItem<T> (item);
				me.Commit ();
			}
		}


		public static void DeleteItem<T> (string id) where T : BL.IBusinessEntity, new()
		{
			lock (locker) 
			{
				//TODO:Possibly not working
				//me.Delete<T> (new T () { ID = id });
				me.Execute (string.Format ("delete from\"{0}\" where \"ID\" = \"{1}\"", typeof(T).Name, id));

			}
		}
			

		public static void ClearTable<T>() where T : BL.IBusinessEntity, new()
		{
			lock (locker) 
			{
				me.Execute (string.Format ("delete from \"{0}\"", typeof(T).Name));
			}
		}

		public IEnumerable<Ch> GetChildren<Ch> (Category parent) where Ch : BL.IBusinessEntity, new()
		{
			try
			{
				return GetItems<Ch> ().Where (x => x.ParentCatID == parent.ID).ToList();
			}catch(NullReferenceException ex){
				HelperMethods.ReportError (ex.ToString ());
				return null;
			}
		}

		public Category GetCategoryById(string id)
		{
			var cat = GetItem<Category> (id);
			return GetCategory (cat);
		}

		public Category GetCategory(string title)
		{
			var cat = GetItems<Category> ().Where (x => x.Title == title).SingleOrDefault();
			return GetCategory (cat);
		}

		public Category GetCategory(Category cat)
		{
			var cards = (GetChildren<Actioncard> (cat)).Cast<IBusinessEntity>();
			var cats = (GetChildren<Category> (cat)).Cast<IBusinessEntity>();
			cat.Children.AddRange(cats);
			cat.Children.AddRange (cards);
			return cat;
		}

		public IEnumerable<Category> GetCategories()
		{
			return GetItems<Category> ().ToList();
		}
			
		public void AddCategory(Category cat)
		{
			foreach (IBusinessEntity child in cat.Children)
				child.AddObjectToDatabase (cat.ID, this);

			Database.SaveItem<Category> (cat);				
		}

		public Actioncard GetActionCardById(string id)
		{
			var card = Database.GetItem<Actioncard>(id);
			return card;
		}

		public Actioncard GetActionCard(string title)
		{
			var card = GetItems<Actioncard> ().Where (x => x.Title == title).SingleOrDefault();
			return card;
		}

		public IEnumerable<Actioncard> GetActionCards()
		{
			return GetItems<Actioncard> ().ToList ();
		}

		public void AddActionCard(Actioncard card)
		{
			SaveItem<Actioncard> (card);
		}

		public void deleteActionCard (Actioncard card)
		{
			DeleteItem<Actioncard> (card.ID);
		}

		public void ClearCategoryTable()
		{
			ClearTable<Category> ();
		}

		public void ClearActionCardsTable()
		{
			ClearTable<Actioncard> ();
		}


		public int AddNote (Note note)
		{
			lock (locker) 
			{
				if (me.Table<Note>().FirstOrDefault(x => x.NoteID == note.NoteID) != null) 
				{
					return me.Update (note);

				}
				else 
					return me.Insert (note);
			}
		}

		public void RemoveNote (Note note)
		{
			me.Execute (string.Format ("delete from\"{0}\" where \"noteID\" = {1}", typeof(Note).Name, note.NoteID));
		}

		public IEnumerable<Note> GetNotes ()
		{
			lock (locker) 
			{
				return (from i in me.Table<Note> ()
					select i).ToList (); 
			}
		}
			
		#endregion
	}
}

