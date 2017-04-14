using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	public interface IUserStory
	{
		int Id { get; set; }
		string Title { get; set; }
	}
}
