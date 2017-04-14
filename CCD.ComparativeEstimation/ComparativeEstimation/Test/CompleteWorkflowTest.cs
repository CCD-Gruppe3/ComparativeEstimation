using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using DbProvider;
using Xunit;

namespace ComparativeEstimation.Test
{
	public class CompleteWorkflowTest
	{
		private readonly string projectPath = @"D:\test";
		private readonly string admin = "admin@foo.hh";
		private readonly string user0 = "user0@foo.hh";
		private readonly string user1 = "user1@foo.hh";
		private readonly string user2 = "user2@foo.hh";
		private readonly string user3 = "user3@foo.hh";

		[Fact]
		public void TestProjectSunshine()
		{
			var dbProvider = new DbProvider.DbProvider(projectPath);
			var handler = new RequestHandler(new SessionState(), dbProvider);

			IProject project = CreateProjectWithUserstories(handler);

			ListProjects(handler);

			RankProject(handler, user0, project.Id);
			RankProject(handler, user1, project.Id);
			RankProject(handler, user2, project.Id);
			RankProject(handler, user3, project.Id);

			ListPersonalProject(handler, project.Id);

			CalculateProjectRanking(handler, project.Id);

		}


		private void ListProjects(RequestHandler handler)
		{
			handler.Login(admin);

			IEnumerable<IProjectInfo> listProjects = handler.ListProjects();
			Assert.Equal(1, listProjects.Count());
			Assert.Equal("Sunshine", listProjects.First().Title);
		}

		private void ListPersonalProject(RequestHandler handler, string id)
		{
			handler.Login(user0);

			var ranking = handler.LoadPersonalRanking(id);

			Assert.Equal(0, ranking[0].Id);
			Assert.Equal(1, ranking[1].Id);
			Assert.Equal(2, ranking[2].Id);
			Assert.Equal(3, ranking[3].Id);
		}


		private IProject CreateProjectWithUserstories(RequestHandler handler)
		{
			handler.Login(admin);
			Assert.Equal(admin, handler.Email);

			var userstories = new List<UserStory>
			{
				new UserStory(0, "Sunrise"),
				new UserStory(1, "High Noon"),
				new UserStory(2, "Sunset"),
				new UserStory(3, "Moonshine")
			};

			var projectId = handler.NewProject("Sunshine", userstories);
			var project = handler.LoadProject(projectId);


			Assert.True(project.UserStories.Any(u => u.Title == "Sunrise"));
			Assert.True(project.UserStories.Any(u => u.Title == "High Noon"));
			Assert.True(project.UserStories.Any(u => u.Title == "Sunset"));
			Assert.True(project.UserStories.Any(u => u.Title == "Moonshine"));


			return project;

		}

		private void RankProject(RequestHandler handler, string user, string id)
		{
			handler.Login(user);

			var project = handler.LoadProject(id);
			Assert.Equal("Sunshine", project.Title);


			IEnumerable<Pair> queryPairs = handler.QueryGetPairs(id);
			var rankings = new List<PairRanking>();
			foreach (var queryPair in queryPairs)
			{
				var left = queryPair.Left.Id < queryPair.Right.Id ? queryPair.Left : queryPair.Right;
				var rihgt = queryPair.Left.Id < queryPair.Right.Id ? queryPair.Right : queryPair.Left;

				rankings.Add(new PairRanking(left.Id, rihgt.Id));
			}

			handler.RankingAbgeben(id, rankings);

			var ranking = handler.LoadPersonalRanking(id);
			Assert.Equal(0, ranking[0].Id);
			Assert.Equal(1, ranking[1].Id);
			Assert.Equal(2, ranking[2].Id);
			Assert.Equal(3, ranking[3].Id);
		}

		private void CalculateProjectRanking(RequestHandler handler, string id)
		{
			handler.Login(admin);

			var ranking = handler.LoadTotalRanking(id);

			Assert.Equal(0, ranking[0].Id);
			Assert.Equal(1, ranking[1].Id);
			Assert.Equal(2, ranking[2].Id);
			Assert.Equal(3, ranking[3].Id);
		}


	}



}
