using System;
using Actioncards.DL.SQLite;


namespace Actioncards.BL
{
	public class Note: ISearchable
	{
		[PrimaryKey, AutoIncrement]
		public int NoteID{ get; set;}
		public string text{ get; set;}
		public string ID { get; set;}
		public string Title{ get; set;}
	}
}

