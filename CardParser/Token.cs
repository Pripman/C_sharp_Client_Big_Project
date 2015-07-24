using System;

namespace Actioncards.CardParser
{
	public enum TokenType {IMG, TEXT, SECTION, ITEM, LINK, PHONENR}

	public class Token
	{
		public Token (TokenType type)
		{
			Type = type; 
		}
		public TokenType Type { get; set; }
		public string Data { get; set;}
	}
}

