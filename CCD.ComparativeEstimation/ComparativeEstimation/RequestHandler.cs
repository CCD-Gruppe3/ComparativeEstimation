using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using EmailChecker;


namespace ComparativeEstimation
{
	public class RequestHandler
	{
		private SessionState state;
		private DbProvider.DbProvider provider;

		internal RequestHandler(SessionState state, DbProvider.DbProvider provider)
		{
			this.state = state;
			this.provider = provider;
		}

		public bool Login(string email)
		{
			// Check valid Email
			var isValid = new RegexUtilities().IsValidEmail(email);

			if (isValid)
			{
				state.Email = email;
				return true;
			}
			state.Email = null;
			return false;
		}

		public IEnumerable<Pair> QueryGetPairs(string id)
		{
			var project = LoadProject(id);
			var userStories = project.UserStories;

			foreach (var left in userStories)
			{
				foreach (var right in userStories)
				{
					if(left.Id < right.Id) yield return new Pair(left, right);
				}
			}
		}

		public IEnumerable<IProjectInfo> ListProjects()
		{
			return provider.LoadProjects(state.Email);
		}

		public InputResult RankingAbgeben(string id, IEnumerable<PairRanking> rankings)
		{
			var personalRanking = CalulatePersonalRanking(rankings);

			if (personalRanking == null)
				return InputResult.Inconsistent;

			provider.SaveProjectRanking(state.Email, id, personalRanking);
			return InputResult.Successful;
		}

		

		public string NewProject(string title, List<UserStory> userStories)
		{
			
			var number = Guid.NewGuid().GetHashCode();
			var newProject = new Project(number, title, state.Email, userStories.OfType<IUserStory>().ToArray());
			
			return provider.SaveProject(newProject);
		}

		public IProject LoadProject(string projetId)
		{
			return provider.LoadProject(projetId);
		}

		public IList<IUserStory> LoadPersonalRanking(string projectId)
		{
			var result = new List<IUserStory>();
			var userRanking = provider.LoadUserRanking(state.Email, projectId);
			var project = provider.LoadProject(projectId);

			foreach (var userStory in userRanking)
			{
				result.Add(project.UserStories.Single(u => u.Id == userStory));
			}

			return result;
		}

		public IList<IUserStory> LoadTotalRanking(string id)
		{
			var rankings =  provider.LoadProjectRankings(id);
			var project = provider.LoadProject(id);

			return CalculateProjectRanking(project, rankings);
		}

		private int[] CalulatePersonalRanking(IEnumerable<PairRanking> rankings)
		{
			return new int[] { 1, 2, 3, 4};
		}

		private IList<IUserStory> CalculateProjectRanking(IProject project, IEnumerable<int[]> rankings)
		{
			return project.UserStories.ToList();
		}

		public string Email => state.Email;
	}

	internal class SessionState
	{
		internal string Email;
	}
}
