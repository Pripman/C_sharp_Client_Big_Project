using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actioncards.DAL;
using Actioncards.DL;

namespace Actioncards.BL
{
	public interface IBusinessEntity
	{
		string ParentCatID { get; set;}
		string Title{ get; set;}
		void AddObjectToDatabase(string parentID, IDatabase db);
		string GetType();
		string ID { get; set; }
	}
}

