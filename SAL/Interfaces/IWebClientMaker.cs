using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Actioncards.BL;
using Actioncards.DAL;


namespace Actioncards.SAL
{
	public interface IWebClientMaker
	{
		void MakeJsonWebClient (Action<string> action, string url);
		void MakeImageWebClient (Action<byte[], string> action, string filename, string url);
		void MakeLogWebClient (string url, string msg);
		bool MakeJsonPostRequest (string url, object postObj);
		event EventHandler DownloadFailed;
	}
}

