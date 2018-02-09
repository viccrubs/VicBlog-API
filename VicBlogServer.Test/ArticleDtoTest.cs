using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using VicBlogServer.Controllers;
using VicBlogServer.Entitites;
using VicBlogServer.Models;
using VicBlogServer.Models.Dto;
using Xunit;

namespace VicBlogServer.Test
{
    public class ArticleDtoTest
    {
        private BlogContext context;

        public ArticleDtoTest()
        {
            InitializeDb();
            InitializeData();
        }

        private void InitializeData()
        {
            List<Article> articles = new List<Article>
            {
                new Article()
                {
                    ID = "1",
                    Content = "Content",
                    LastEditedTime = new DateTime(2018,2,9,10,20,30),
                    CreateTime = new DateTime(2018,2,8,10,20,30),
                    Like = 10,
                    Tags = new List<string>{"tag1", "tag2"},
                    Title = "Title1",
                    Username = "user1"
                },
                new Article()
                {
                    ID = "2",
                    Content = "Content",
                    LastEditedTime = new DateTime(2018,2,9,10,20,59),
                    CreateTime = new DateTime(2008,2,9,10,20,30),
                    Like = 16,
                    Tags = new List<string>{"tag1"},
                    Title = "Title2 contains",
                    Username = "user1"
                }
            };
            context.Articles.AddRange(articles);
            context.SaveChanges();
        }

        private void InitializeDb()
        {
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseInMemoryDatabase(databaseName: (new Random()).NextDouble().ToString())
                .Options;

            context = new BlogContext(options);
            context.Database.EnsureCreated();
        }

        [Fact]
        public void Test()
        {
            ILoggerFactory factory = new LoggerFactory(new[]{ new ConsoleLoggerProvider((_, __) => true, true) });
            BlogContext context = new BlogContext(new DbContextOptionsBuilder<BlogContext>()
                .UseLoggerFactory(factory)
                .UseSqlServer("Data Source=(localdb)\\ProjectsV13;Initial Catalog=TestDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")
                .Options);
            context.Database.EnsureCreated();

            ArticleFilterDto dto = new ArticleFilterDto()
            {
                TitleText = "123",
                CreatedTimeRange = new long[] { 10, 20}
            };
            List<Article> retrievedArticles = dto.Execute(context.Articles);
            Assert.Single(retrievedArticles);

        }

        public static void Main(string[] args)
        {
            new ArticleDtoTest().Test();
            Console.ReadLine();
        }

        [Fact]
        public void TestTag1()
        {
            ArticleFilterDto dto = new ArticleFilterDto()
            {
                Tags = new List<string> { "tag1" }
            };
            List<Article> retrievedArticles = dto.Execute(context.Articles);
            Assert.Equal(2, retrievedArticles.Count);
            
        }

        [Fact]
        public void TestTag2()
        {
            ArticleFilterDto dto = new ArticleFilterDto()
            {
                Tags = new List<string> { "tag2" }
            };
            List<Article> retrievedArticles = dto.Execute(context.Articles);
            Assert.Single(retrievedArticles);
        }


        [Fact]
        public void TestTag1AndTag2()
        {
            ArticleFilterDto dto = new ArticleFilterDto()
            {
                Tags = new List<string> { "tag2", "tag1" }
            };
            List<Article> retrievedArticles = dto.Execute(context.Articles);
            Assert.Equal(2, retrievedArticles.Count);
        }
    }
}
