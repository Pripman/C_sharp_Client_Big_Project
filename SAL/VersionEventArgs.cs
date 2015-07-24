using System;

namespace Actioncards.SAL
{
	public class VersionEventArgs:EventArgs	
	{
		public double OnlineVersion{ get; set;}


		public VersionEventArgs (double onlineVersion)
		{
			this.OnlineVersion = onlineVersion;
		}
	}
}

