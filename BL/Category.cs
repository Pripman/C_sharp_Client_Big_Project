using System;
using System.Collections.Generic;
using Actioncards.DAL;
using Actioncards.DL;
using Actioncards.DL.SQLite;
using System.Linq;

namespace Actioncards.BL
{
	public class Category : IBusinessEntity, ISearchable
	{

		[PrimaryKey]
		public string ID { get; set;}
		public bool IsRoot{ get; set; }
		public string ParentCatID{ get; set; }
		public string Title { get; set; }
		public int Color { get; set; }
		private List<IBusinessEntity> _children;
		[Ignore]
		public List<IBusinessEntity> Children 
		{ 
			get{ _children = _children.OrderBy (x => x.Title).ToList(); return _children; } 
			set{_children = value; } 
		}

		public Category ()
		{
			Children = new List<IBusinessEntity> ();
		}

		public void AddObjectToDatabase(string parentID, IDatabase db)
		{
			this.ParentCatID = parentID;
			db.AddCategory (this);
		}

		string IBusinessEntity.GetType ()
		{
			return "Category";
		}
	}
}

