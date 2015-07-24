using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Actioncards.BL;
using Actioncards.DAL;
using Actioncards.SAL;

namespace Actioncards.SAL
{
	public class WebClientMaker: IWebClientMaker
	{
		public event EventHandler DownloadFailed;
		public event EventHandler UploadFailed;


		public void MakeJsonWebClient(Action<string> action, string url)
		{
			var webClient = new WebClient ();
			webClient.DownloadStringCompleted += (sender, e) =>
			{
				try 
				{
					Debug.WriteLine(string.Format("jsonString is {0}!!!!!!!!", e.Result));
					var jsonString = (string)e.Result;
					action(jsonString);
				}

				catch (System.Reflection.TargetInvocationException  ex) 
				{
					HelperMethods.ReportError("Error -- Maybe problem with internet connection " + ex);
					OnDownloadFailed(EventArgs.Empty);
				}

				catch(JsonNullException ex){
					HelperMethods.ReportError("Error -- Maybe a property was null  : " + ex);
					OnDownloadFailed(EventArgs.Empty);
				}
				catch(Newtonsoft.Json.JsonReaderException ex){
					HelperMethods.ReportError("Error -- Maybe the downloaded JSON is corrupted: " +  ex);
					OnDownloadFailed(EventArgs.Empty);
				}
			};

			webClient.Encoding = System.Text.Encoding.UTF8;
			webClient.DownloadStringAsync (new Uri(url));
		}

		public void MakeImageWebClient(Action<byte[], string> action, string filename, string url)
		{
			var webClient = new WebClient ();
			webClient.DownloadDataCompleted += (sender, e) =>
			{
				try 
				{
					var bytes = e.Result;
					action(bytes, filename);
					Debug.WriteLine(filename + " downloaded and assigned to folder!!");
				}
				catch (Exception ex) 
				{
					HelperMethods.ReportError ("ERROR getting downloaded image: " + filename  + ex);
					OnDownloadFailed(new ImageEventArgs(filename));
				}
			};

			webClient.DownloadDataAsync (new Uri(url + filename));
		}

 
		public void MakeLogWebClient(string url, string msg)
		{
			using(WebClient webClient = new WebClient ())
			{

				try{
					System.Collections.Specialized.NameValueCollection reqparm = new System.Collections.Specialized.NameValueCollection();
					reqparm.Add("msg", msg);
					byte[] responsebytes = webClient.UploadValues(url, "POST", reqparm);
					string responsebody = Encoding.UTF8.GetString(responsebytes);
					if(responsebody == "ok")					
						Debug.WriteLine("Error: " + msg + "  ... Reported in the online log");
				}
				catch(System.Net.WebException){
					Debug.WriteLine ("No internet to send log");
				}
			}
		}

		public bool MakeJsonPostRequest(string url, object postObj)
		{


			var serializedJsonRequest = JsonConvert.SerializeObject (postObj);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
			request.Method = "Post";
			request.ContentType = "text/json";
			HttpWebResponse res;



			try
			{
				using (var writer = new StreamWriter (request.GetRequestStream ())) 
				{
					writer.Write (serializedJsonRequest);
					writer.Flush ();
				}
			}
			catch (WebException webEx)
			{
				HelperMethods.ReportError(string.Format("Posting feedback resulted in error:{0} Check internet connection", webEx));
				return false;
			}

			try
			{
				res = (HttpWebResponse)request.GetResponse();
			}
			catch (WebException webEx)
			{
				res = (HttpWebResponse)webEx.Response;
				HelperMethods.ReportError("Posting feedback resulted in: " + res.StatusCode.ToString());
				res.Close();
				return false;
			}

			return true;
		}



		private void OnDownloadFailed(EventArgs e){
			if (DownloadFailed != null)
				DownloadFailed (this, e);
		}
	}
}

