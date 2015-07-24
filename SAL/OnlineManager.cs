using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Actioncards.BL;
using Actioncards.DAL;
using Actioncards.Instantiater;


namespace Actioncards.SAL
{
	public delegate void logger(string msg);

	public class OnlineManager:IOnlineManager
	{
		private IDataManager _dm;
		private IDeserializer _ds;
		private IWebClientMaker _wm;
		private IDownloadBuffer _dbf;
		public event EventHandler<VersionEventArgs> AppUpdatedNeeded;
		public event EventHandler<VersionEventArgs> AppUpdatedOptional;
		public event EventHandler DownloadFinished;
		public event EventHandler DownloadFailed;
		public event EventHandler NewCardsAvailable;
		public event EventHandler<ImageEventArgs> ImageDownloadFinished;
		public bool CanEmptyBuffer { get; set;}
		public bool IsDownloading{ get; set;}
		static object locker = new object();
		private int _currentCardNumber;
		private int _NumberOfCards;
		private bool _cardsDownloaded;
		private bool _catsDownloaded;
		private bool _downloadError;
		private bool _pendingBuffer;
		logger log;


		public OnlineManager(IDownloadBuffer dbf,  IDataManager dm, IDeserializer ds, IWebClientMaker wm)
		{
			_dm = dm;
			_ds = ds;
			_wm = wm;
			_wm.DownloadFailed += OnDownloadFailed;
//			DownloadFinished += (object sender, EventArgs e) => {
//				if(!_downloadError)
//				{
//					_dbf.addBufferToDatabase ();
//					CheckDatabaseIntegrity ();
//				}
//			};

			_dbf = dbf;
			resetOnlineManager ();
			log = HelperMethods.ReportInfo;
		}



		public void CheckForNewCards(){
			if (!_downloadError) {
				_wm.MakeJsonWebClient (EvaluateIfExistsAndVersion, Constants.CardListUrl + HelperMethods.GetRandomCacheParameter());
			}

		}

		public void EvaluateIfExistsAndVersion(string jsonList){
			var list = _ds.DeserializeCardList (jsonList);
			bool newCardsAvailable;
			var cardsInDB = _dm.GetActionCards ();
			foreach (var item in list) {
				var card = cardsInDB.Where (x => x.ID == item.Id).FirstOrDefault();
				if (card == null || card.Version != item.Version){
					log(string.Format("New card available:{0}", item.Id));
					OnNewCardsAvailable (EventArgs.Empty);
					return;
				} 
			}
		}


		public void UpdateData()
		{
			if (!IsDownloading) 
			{
				resetOnlineManager ();
				IsDownloading = true;
				DownloadAppVersionAndStartDownload ();
			}
		}

		private void resetOnlineManager()
		{
			_dbf.ResetDownloadBuffer ();
			_catsDownloaded = false;
			_cardsDownloaded = false;
			_downloadError = false;
			_currentCardNumber = 0;
		}

		public void ResetDownloadEventlisteners(){
			AppUpdatedNeeded = null;
			AppUpdatedOptional = null;
			DownloadFinished = null;
			DownloadFailed = null;
			_wm.DownloadFailed += OnDownloadFailed;
//			DownloadFinished += (object sender, EventArgs e) => {
//				if(!_downloadError)
//				{
//					_dbf.addBufferToDatabase ();
//				}
//			};

		}

		/****
		* Public Service Methods.
		****/
		public void DownloadAppVersionAndStartDownload()
		{
			if (!_downloadError) {
				_wm.MakeJsonWebClient (CheckAppVersionAndDownload, Constants.VersionUrl + HelperMethods.GetRandomCacheParameter());
			}
		}

		public void DownloadActionCard(string id)
		{
			if (!_downloadError) {
				_wm.MakeJsonWebClient (AddCardAndIncrementCardNumber, Constants.ActionCardUrl + id + HelperMethods.GetRandomCacheParameter());
			}
		}

		public void DownloadCategories()
		{
			if (!_downloadError)
				_wm.MakeJsonWebClient (AddDownloadedCats, Constants.CategoriesUrl + HelperMethods.GetRandomCacheParameter());
		}

		public void DownloadCardList()
		{
			if (!_downloadError) {
				_wm.MakeJsonWebClient (DownloadRelevantCards, Constants.CardListUrl + HelperMethods.GetRandomCacheParameter());
			}
		}

		public void DownloadImage(string filename)
		{
			if (!_downloadError) {
				_wm.MakeImageWebClient (AddImageToFolder, filename, Constants.ImageUrl);
			}
		}

		public void reportToLog(string msg, string url)
		{
			_wm.MakeLogWebClient (url, msg);
		}

		public bool PostFeedback(Feedback feedback){

			return _wm.MakeJsonPostRequest (Constants.FeedbackUrl, feedback);
		}

		public void StartDownload()
		{
		
			DownloadCategories ();
			DownloadCardList ();
		}


			
		/******* 
		 * Different Actions for the generic MakeJsonWebClient method, used for different API-calls. 
		********/
		public void CheckAppVersionAndDownload(string jsonString){
			var onlineVersion = _ds.DeserializeAppVersion (jsonString);
			HelperMethods.ReportInfo(string.Format("Online verion fetched:{0}", onlineVersion));
			var updateAvailable = onlineVersion > Constants.AppVersion;
			var onlineMajorUpdateNum = Math.Floor (onlineVersion);
			var needsUpdate = onlineMajorUpdateNum > Constants.AppVersion;
			HelperMethods.ReportInfo(string.Format("Needs update:{0}", needsUpdate));

			if (needsUpdate) 
			{
				OnAppUpdateNeeded (new VersionEventArgs (onlineVersion));
			}
			else if (updateAvailable) 
			{
				OnAppUpdateOptional (new VersionEventArgs (onlineVersion));
				StartDownload ();
			}
			else  
			{
				StartDownload ();
			}
		}



		private void DownloadRelevantCards(string jsonString)
		{

			var cardList = _ds.DeserializeCardList(jsonString); 
			_dbf.cardList = cardList;
			_NumberOfCards = cardList.Count;
			if (_NumberOfCards == 0)
				CheckAllCardsDownloaded ();

			foreach (CardListElement element in cardList) 
			{
				Actioncard card = _dm.GetActionCardById (element.Id);
				var needsDownload = 
					(card == null || card.Version != element.Version) 
					&& !_dbf.CardIsInBuffer (element.Id);

				if (needsDownload) 
				{
					HelperMethods.ReportInfo(string.Format("Card with id:{0} needs to be download", element.Id));
					DownloadActionCard (element.Id);
				} 
				else
				{
					_currentCardNumber++;
					HelperMethods.ReportInfo(string.Format("Card with id:{0} was not downloaded", element.Id));

					CheckAllCardsDownloaded ();
				}
			}
		}

		private void AddDownloadedCats(string jsonString)
		{
			var cats = _ds.DeserializeCategories(jsonString);
			_dbf.AddCategoriesToBuffer(cats);
			lock (locker) 
			{
				_catsDownloaded = true;
				if (_cardsDownloaded == true)
					OnFinishedDownload (EventArgs.Empty);
			}
		}

		private void AddCardAndIncrementCardNumber(string jsonString)
		{
			var card = _ds.DeserializeActionCard(jsonString);
			_dbf.AddCardToBuffer(card);
			_currentCardNumber++;
			CheckAllCardsDownloaded ();
		}
			

		/******* 
		 * Different Actions for the generic MakeImageWebClient method, used for different API-calls. (Only one so far!)
		********/


		private void AddImageToFolder(byte[] bytes, string filename)
		{
			var documentsPath = Constants.ImageFolderPath;
			var path = System.IO.Path.Combine (documentsPath, filename);
			File.WriteAllBytes (path, bytes); 
			OnImageDownloadFinished (new ImageEventArgs(filename));
		}

		/******
		* Helper methods.
		******/

		public void CheckAllCardsDownloaded()
		{
			if(_currentCardNumber == _NumberOfCards)
			{
				lock(locker)
				{
					_cardsDownloaded = true;
					if (_catsDownloaded) {
						_pendingBuffer = true;
						EmptyBuffer();
					}
				}
			}
		}

		public void EmptyBuffer ()
		{
			if (!_downloadError) {
				if (CanEmptyBuffer && _pendingBuffer) {
					_dbf.addBufferToDatabase ();
					CheckDatabaseIntegrity ();
					_pendingBuffer = false;
					OnFinishedDownload(EventArgs.Empty);
				}
			}
		}

		private void OnAppUpdateNeeded(VersionEventArgs e)
		{

			if (AppUpdatedNeeded != null)
				AppUpdatedNeeded (this, e);
			IsDownloading = false;
		}

		private void OnAppUpdateOptional(VersionEventArgs e)
		{
			if (AppUpdatedOptional != null)
				AppUpdatedOptional (this, e);
		}
			
		private void OnFinishedDownload(EventArgs e)
		{

			if (DownloadFinished != null)
				DownloadFinished (this, e);
			IsDownloading = false;


		}

		private void OnDownloadFailed(object sender, EventArgs e)
		{
			if (DownloadFailed != null) 
			{
				_downloadError = true;
				DownloadFailed (sender, e);
			}
			IsDownloading = false;
		}

		private void OnImageDownloadFinished(ImageEventArgs e)
		{
			if (ImageDownloadFinished != null)
				ImageDownloadFinished (this, e);
			IsDownloading = false;
		}

		private void OnNewCardsAvailable(EventArgs e)
		{
			if (NewCardsAvailable != null)
				NewCardsAvailable (this, e);
		}

		public void CheckDatabaseIntegrity()
		{
			HelperMethods.ReportInfo ("\n\n--Categories---------------------------------------------------------\n\n");

			var cards = _dm.GetActionCards ().ToList ();
			var cats = _dm.GetCategories ();
			var notes = _dm.GetNotes ();
			foreach (var cat in cats) {
				HelperMethods.ReportInfo (string.Format ("\nTitle: {0}\n ID: {1}\n Parent: {2}\n\n\n\n", 
					cat.Title, cat.ID, cat.ParentCatID));

			}

			HelperMethods.ReportInfo ("\n\n---Cards-------------------------------------------------------------\n\n");
			foreach (var card in cards) {
				HelperMethods.ReportInfo (string.Format ("\nTitle: {0}\n ID: {1}\n Subtitle: {2}\n Parent: {3}\n\n\n\n", 
					card.Title, card.ID, card.Stitle, card.ParentCatID));

			}

			List<string> cardsToBeDeleted = new List<string> ();
			foreach (var card in cards) {
				var found = cats.Any (x => x.ID == card.ParentCatID);
				if (found == false) {
					cardsToBeDeleted.Add (card.ID);
				}
			}

			foreach (var id in cardsToBeDeleted) {
				var card = _dm.GetActionCardById (id);
				HelperMethods.ReportInfo (
					string.Format ("Removing card with title: {0}, and ID: {1}. Its parentID didn't match any categories", 
						card.Title, card.ParentCatID));
				_dm.deleteActionCard (card);
			}
				
			List<int> notesToBeDeleted = new List<int> ();
			foreach (var note in notes) {
				var found = cards.Any (x => x.ID == note.ID);
				if (found == false) {
					notesToBeDeleted.Add (note.NoteID);
				}
			}

			foreach (var id in notesToBeDeleted) {
				var note = notes.Where(x => x.NoteID == id).FirstOrDefault();
				if(note != null){
					_dm.RemoveNote (note);
					HelperMethods.ReportInfo (
						string.Format ("Removing note with CardID: {0}. Its parentID didn't match any categories", 
							note.ID));
					_dm.RemoveNote (note);
				}
			}



			
					
		}

	}
}

