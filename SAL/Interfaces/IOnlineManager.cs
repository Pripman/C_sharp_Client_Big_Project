using System;
using Actioncards.BL;
using System.Collections.Generic;

namespace Actioncards.SAL
{
	public interface IOnlineManager
	{
		//void DownloadActionCard(string id);
		void UpdateData();
		//void DownloadCategories ();
		//void DownloadCardList();
		//void DownloadImage(string filename);
		void ResetDownloadEventlisteners ();
		void reportToLog (string msg, string url);
		bool PostFeedback (Feedback feedback);
		//void CheckForNewCards ();
		event EventHandler<VersionEventArgs> AppUpdatedNeeded;
		event EventHandler<VersionEventArgs> AppUpdatedOptional;
		event EventHandler DownloadFinished;
		//event EventHandler NewCardsAvailable;
		event EventHandler DownloadFailed;
		//event EventHandler<ImageEventArgs> ImageDownloadFinished;
		bool CanEmptyBuffer{ get; set;}
		void EmptyBuffer();
		bool IsDownloading{ get; set;}

	}
}

