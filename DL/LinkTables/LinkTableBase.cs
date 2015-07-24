using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actioncards.DL.SQLite;

namespace Actioncards.DL.LinkTables
{
	public abstract class LinkTableBase :ILinkTable
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set;}
		public int ParentId{ get; set;}
		public int ChildId{ get; set;}

	}
}

