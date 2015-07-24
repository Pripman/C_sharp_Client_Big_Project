using System;

namespace Actioncards.BL
{
	public class Feedback
	{
		public string cardid { get; set;}
		public string from { get; set;}
		public string email { get; set;}
		public string phone { get; set;}
		public string message { get; set;}

		public Feedback (string id, string from, string email, string phonenumber, string message)
		{
			this.cardid = id;
			this.from = from;
			this.email = email;
			this.phone = phonenumber;
			this.message = message;
		}
	}
}

