using System;
using System.Collections.Generic;

namespace Actioncards.CardParser
{
	public interface ITokenGenerator
	{
		List<Token> ScanMetaText(string metaText);
	}
}

