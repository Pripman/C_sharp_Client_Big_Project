using System;

namespace Actioncards.SAL
{
	public class ImageEventArgs:EventArgs	
	{
		public string Filename{ get; set;}


		public ImageEventArgs (string filename)
		{
			this.Filename = filename;
		}
	}
}

