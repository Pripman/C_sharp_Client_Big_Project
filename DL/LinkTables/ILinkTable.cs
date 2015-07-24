using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actioncards.BL;

namespace Actioncards.DL
{
	public interface ILinkTable: IBusinessEntity
	{
		int ParentId{ get; set;}
		int ChildId{ get; set;}
	}
}

