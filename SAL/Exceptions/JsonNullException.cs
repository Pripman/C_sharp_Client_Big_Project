using System;

namespace Actioncards.SAL
{
	public class JsonNullException:NullReferenceException
	{
		public JsonNullException (string msg):base(msg)
		{
		}
	}
}

