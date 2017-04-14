using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Xunit;

namespace ComparativeEstimation.Test
{
	/*
			Gewichtung:
			1 2 3 4

			Einzel-Rankings:
			3,4,2,1
			4,2,3,1
			4,3,2,1
			1,3,4,2

			Ergebnis:
			4  7
			3  8
			2  12
			1  13
	 */


	public class DbProviderTests
	{
		DbProvider.DbProvider Provider = new DbProvider.DbProvider(@"Test\TestData");

		
		[Fact]
		public void LoadProjectTest()
		{
			var p1 = Provider.LoadProject("Project_1");
			Assert.Equal(1234, p1.Number);
			Assert.Equal("jo@cp.ag", p1.Email);
			Assert.Equal("NewChart without MutliDim", p1.Title);
			Assert.Equal(4, p1.UserStories.Length);
			Assert.Equal("Blaa", p1.UserStories[0].Title);
			Assert.Equal("Blubb", p1.UserStories[1].Title);
			Assert.Equal("Laber", p1.UserStories[2].Title);
			Assert.Equal("Rhabarber", p1.UserStories[3].Title);
		}

		[Fact]
		public void LoadProjectRankingsTest()
		{
			var r0 = Provider.LoadProjectRankings("Project_1").ToArray();

			Assert.Equal(4, r0.Length);
			Assert.True(r0.Any(r => B(r, 3, 4, 2, 1)));
			Assert.True(r0.Any(r => B(r, 4, 2, 3, 1)));
			Assert.True(r0.Any(r => B(r, 4, 3, 2, 1)));
			Assert.True(r0.Any(r => B(r, 1, 3, 4, 2)));
		}

		private static bool B(int[] r, int a, int b, int c, int d) =>
			r[0] == a && r[1] == b && r[2] == c && r[3] == d;
		

		[Fact]
		public void LoadUserRankingTest()
		{
			var r0 = Provider.LoadUserRanking("tom@cp.ag","Project_1");

			Assert.Equal(new[] { 3, 4, 2, 1 }, r0);
		}


		[Fact]
		public void ListProjectsTest()
		{
			var p0 = Provider.LoadProjects("jo@cp.ag").ToArray();

			Assert.Equal(1, p0.Length);
			Assert.Equal("Project_1", p0[0].Id);
			Assert.Equal("jo@cp.ag", p0[0].Email);
		}

		[Fact]
		public void ListProjectsTest_Fail()
		{
			var p0 = Provider.LoadProjects("tom@cp.ag").ToArray();
			Assert.Equal(0, p0.Length);
		}

		[Fact]
		public void GetProjectIdTest()
		{
			var id = Provider.GetProjectId(1234);

			Assert.Equal("Project_1", id);
		}

		[Fact]
		public void SaveProjectTest()
		{
			var userStories = new[]
			{
				new UserStory(1, "Blaa"),
				new UserStory(2, "Blubb"),
				new UserStory(3, "Laber"),
				new UserStory(4, "Rhabarber"),
				new UserStory(5, "Erberkäse")
			};

			var project = new Project(1234,"flo@cp.ag", "NewChart without MutliDim", userStories);

			var id = Provider.SaveProject(project);

			Assert.True(Directory.Exists($@"Test\TestData\{id}"));

			var loadProject = Provider.LoadProject(id);
			Assert.NotNull(loadProject);

			Assert.Equal(project.Email, loadProject.Email);
			Assert.Equal(project.Title, loadProject.Title);


			Directory.Delete($@"Test\TestData\{id}", true);
		}


		public void SaveProjectRankingTest()
		{
			
			Provider.SaveProjectRanking("bigBoss@cp.ag","Project_1", new[] {4, 1, 3, 2} );

			Assert.True(File.Exists(@"Test\TestData\Project_1\Rankings\bigBoss@cp.ag.txt"));

			var ranking = Provider.LoadUserRanking("bigBoss@cp.ag", "Project_1");
			Assert.NotNull(ranking);
			Assert.Equal(ranking, new[] { 4, 1, 3, 2 });

			Provider.SaveProjectRanking("bigBoss@cp.ag", "Project_1", new[] {3, 2, 4, 1});
			ranking = Provider.LoadUserRanking("bigBoss@cp.ag", "Project_1");
			Assert.NotNull(ranking);
			Assert.Equal(ranking, new[] {3, 2, 4, 1});


			File.Delete(@"Test\TestData\Project_1\Rankings\bigBoss@cp.ag.txt");
		}
	}
}
