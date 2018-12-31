using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects;
using Forum.Data.Models;
using Forum.Data.Models.Users;
using Forum.Data.Services.Tests.Fake;
using Forum.Services.Data;
using Forum.Services.Data.Interfaces;
using Forum.Services.Data.Utils;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Forum.Data.Services.Tests
{
    public class DatabaseInitializerTests
    {
        private readonly IRepository<Category> categoryRepository;
        private readonly IRepository<Post> postRepository;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Comment> commentRepository;
        private readonly ILogger<IDatabaseInitializer> logger;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly IAccountService accountService;
        private readonly IDatabaseInitializer databaseInitializer;
        private readonly IConfigurationRoot config;

        public DatabaseInitializerTests()
        {
            this.config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();

            var services = new ServiceCollection();

            services.AddDbContext<ForumContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            services.AddHttpContextAccessor();
            services.AddOptions();
            services.AddLogging();
            services.AddAutoMapper(con =>
            {
                con.AddProfile(new MapInitialization());
            });

            var httpContext = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(httpContext.Object);
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor
            {
                HttpContext = httpContext.Object,
            });

            services.AddScoped<IAccountService, AccountService>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ForumContext>()
                .AddDefaultTokenProviders();

            var serviceScope = services.BuildServiceProvider();

            this.roleManager = serviceScope.GetService<RoleManager<IdentityRole>>();
            this.userManager = serviceScope.GetService<UserManager<User>>();
            this.logger = serviceScope.GetService<ILogger<IDatabaseInitializer>>();
            this.userRepository = serviceScope.GetService<IRepository<User>>();
            this.categoryRepository = serviceScope.GetService<IRepository<Category>>();
            this.postRepository = serviceScope.GetService<IRepository<Post>>();

            this.accountService = serviceScope.GetService<IAccountService>();
            this.databaseInitializer = new DatabaseInitializer();
        }

        [Fact]
        public async Task ShouldSeedSuccessfully()
        {
            var expectedCategories = new string[]
                {"Education", "Football", "Basketball", "Marketing", "Blockchain", "Programming", "Game Theory"};

            await this.databaseInitializer.Seed(roleManager, userManager, config, accountService, logger, userRepository,
                categoryRepository, postRepository);
            var categories = this.categoryRepository.Query().ToList();
            Assert.Equal(expectedCategories.Length, categories.Count);

            int i = 0;
            foreach (var category in categories)
            {
                Assert.Equal(expectedCategories[i], category.Name);
                i++;
            }

            var posts = this.postRepository.Query().ToList();

            string[] postTitles =
            {
                "Is Education Important?",
                "Cristiano in Juventus",
                "Lebron in Lakers",
                "What is Marketing?",
                "Where does it come?",
                "React Tutorial",
                "GEB - Kalai",
            };

            string[] postDescriptions =
            {
                "Why is Education So Important in Our Life? When I started thinking about why education is so important, I remembered my high school years when I used to spend almost five hours a month on math homework, wake up at 6:00 AM and get ready for my PSAL soccer game after school. I remembered my teachers, school subjects, the study and the fun! I never really hated school. But I have seen many of my peers who hated going to school; I have had some friends who did not like the idea of studying. Some needed to be up in summer school for recovery. I personally was always focused because I wanted to become a software engineer. I know it will be hard and very challenging. However I believe I can handle the challenge. The first thing that strikes me about education is knowledge gain. Education gives us a knowledge of the world around us and changes it into something better. It develops in us a perspective of looking at life. It helps us build opinions and have points of view on things in life. People debate over the subject of whether education is the only thing that gives knowledge. Some say education is the process of gaining information about the surrounding world while knowledge is something very different. They are right. But then again, information cannot be converted into knowledge without education. Education makes us capable of interpreting things, among other things. It is not just about lessons in textbooks. It is about the lessons of life. One thing I wish I can do is, to provide education for all: no child left behind and change the world for good!!",
                "When the rumours of a possible deal were first leaked, few believed it to be true. Ronaldo was simply doing his usual summer dance with Real Madrid in a bid to receive more love, support and money from his employers. Additionally, if he were ever to leave, surely it wouldn\'t be to Italy? Yet remarkably, Juventus - renowned for their ability to spot a bargain - have decided to go off script and make the most expensive Serie A signing in history. Agent Jorge Mendes first implied a deal was possible when he met with the club over their £35.5m signing of his client Joao Cancelo from Valencia in June. Moving heaven and earth financially, Juventus found a way to stump up over 105m euros (£93.01m) to Real Madrid to take the soon to be 34-year-old legend to Turin. Then, having agreed to pay the forward over 30m euros (£26.57m) a year in wages after tax (nearly 60m euros before deductions), the deal was struck.",
                "Magic Johnson says LeBron James\' arrival has taken the Los Angeles Lakers\' three-year rebuilding plan to \"a whole nother level\" and that the team plans to remain disciplined and maintain salary-cap space to pursue another max free agent next summer. The Lakers\' president of basketball operations said the team\'s rebuilding timetable remains on track to take another significant step next season. \"If we feel there\'s somebody out there or a deal to be made to make our team better, then we\'ll do it as long as it\'s a great deal for us,\" Johnson said during a conference call Friday. \"If it\'s not, we have our team and we\'ll go to battle, go to war with this team. We feel really good about this team. \"Then we\'ll have enough room for next summer to give another player a max deal. [General manager] Rob [Pelinka] and I, we already put the strategy together. LeBron of course changed some of that, but we\'re still going to stay disciplined and hope we\'ll be a team that can have a championship run for a long time.\"",
                "The management process through which goods and services move from concept to the customer. It includes the coordination of four elements called the 4 P\'s of marketing: (1) identification, selection and development of a product, (2) determination of its price, (3) selection of a distribution channel to reach the customer\'s place, and (4) development and implementation of a promotional strategy. For example, new Apple products are developed to include improved applications and systems, are set at different prices depending on how much capability the customer desires, and are sold in places where other Apple products are sold. In order to promote the device, the company featured its debut at tech events and is highly advertised on the web and on television. Marketing is based on thinking about the business in terms of customer needs and their satisfaction. Marketing differs from selling because (in the words of Harvard Business School\'s retired professor of marketing Theodore C. Levitt) \"Selling concerns itself with the tricks and techniques of getting people to exchange their cash for your product. It is not concerned with the values that the exchange is all about. And it does not, as marketing invariable does, view the entire business process as consisting of a tightly integrated effort to discover, create, arouse and satisfy customer needs.\" In other words, marketing has less to do with getting customers to pay for your product as it does developing a demand for that product and fulfilling the customer\'s needs Read more: http://www.businessdictionary.com/definition/marketing.html",
                "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of \"de Finibus Bonorum et Malorum\" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, \"Lorem ipsum dolor sit amet..\", comes from a line in section 1.10.32. The standard chunk of Lorem Ipsum used since the 1500s is reproduced below for those interested. Sections 1.10.32 and 1.10.33 from \"de Finibus Bonorum et Malorum\" by Cicero are also reproduced in their exact original form, accompanied by English versions from the 1914 translation by H. Rackham.",
                "In this tutorial, we’ll show how to build an interactive tic-tac-toe game with React. You can see what we’ll be building here: Final Result. If the code doesn’t make sense to you, or if you are unfamiliar with the code’s syntax, don’t worry! The goal of this tutorial is to help you understand React and its syntax. We recommend that you check out the tic-tac-toe game before continuing with the tutorial. One of the features that you’ll notice is that there is a numbered list to the right of the game’s board. This list gives you a history of all of the moves that have occurred in the game, and is updated as the game progresses. You can close the tic-tac-toe game once you’re familiar with it. We’ll be starting from a simpler template in this tutorial. Our next step is to set you up so that you can start building the game.",
                "Games and Economic Behavior (GEB) is a general-interest journal devoted to the advancement of game theory and it applications. Game theory applications cover a wide range of subjects in social, behavioral, mathematical and biological sciences, and game theoretic methodologies draw on a large variety..."
            };

            i = 0;
            foreach (var post in posts)
            {
                Assert.Equal(postTitles[i], post.Title);
                Assert.Equal(postDescriptions[i], post.Body);
                i++;
            }
        }
    }
}
