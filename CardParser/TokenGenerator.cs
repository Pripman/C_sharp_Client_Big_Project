using System;
using System.IO;
using System.Collections.Generic;

namespace Actioncards.CardParser
{
	public class TokenGenerator:ITokenGenerator
	{
		private List<Token> _cardItems;
		private char[] _reader;
		private int _currentIndex;
		private string _textBuffer;
		private int _readerLength;
		public List<Token> ScanMetaText(string metaText)
		{
			_cardItems = new List<Token> ();
			_reader = metaText.ToCharArray ();
			_readerLength = _reader.Length;
			_currentIndex = 0;
			_textBuffer = "";

			try{
				while (_currentIndex < _readerLength) 
				{
					switch(GetCurrent())
					{
					case("@"):

						TypeMaker ();
						break;

					default:
						_textBuffer += GetCurrent ();
						if (_currentIndex + 1 < _readerLength)
							Read ();
						else 
						{
							BufferToToken();
							_currentIndex++;
						}
							break;
					}	
				}
			}
			//TODO:This should be specialized
			catch(Exception ex){
				HelperMethods.ReportError ("There was a problem scanning meta test to a card..... exception:" + ex);
			
			}

			return _cardItems;
		} 
			
		private void TypeMaker()
		{
			if (_textBuffer != "")
				BufferToToken ();
			var readType = ReadUntilQute ();
			switch (readType) 
			{
			case("img "):
				MakeToken (TokenType.IMG);
				break;
			case("item "):
				MakeToken (TokenType.ITEM);
				break;
			case("sec "):
				MakeToken(TokenType.SECTION);
				break;
			default:
				break;
			}
		}

		private void MakeToken(TokenType type)
		{
			Token t = new Token (type);
			t.Data = ReadUntilQute ();
			_cardItems.Add (t);
			Read ();
		}

		public void BufferToToken()
		{
			Token t = new Token (TokenType.TEXT);
			t.Data = _textBuffer;
			_cardItems.Add (t);
			_textBuffer = "";
		}
			
		private string ReadUntilQute()
		{
			string data = "";
			string currentChar = "";

			while(Read() != "\"") 
			{
				currentChar = GetCurrent();
				data += currentChar;
			} 
			return data;
		}

		private string GetCurrent()
		{
			return _reader [_currentIndex].ToString();	
		}

		private string Peek()
		{
			return _reader [_currentIndex + 1].ToString();	
		}

		private string Read()
		{
			_currentIndex++;
			if(_readerLength > _currentIndex)
				return (string)_reader [_currentIndex].ToString();
			else return "ø";
		}
	}
}

