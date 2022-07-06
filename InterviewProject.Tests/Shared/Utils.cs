using System;
using AutoMapper;
using InterviewProject.Data;
using InterviewProject.MappingConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace InterviewProject.Tests.Shared
{
    public static class Utils
    {
        public static IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new InterviewMappingProfile());
            });

            return new Mapper(mapperConfig);
        }

        public static MainDbContext CreateDbContext(bool feedWithData = false)
        {
            var mainDbContext = new MainDbContext(
                new DbContextOptionsBuilder<MainDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options);

            if (feedWithData)
            {
                mainDbContext.Tags.Add(FakeData.GetTag(1, "Tag1"));
                mainDbContext.Tags.Add(FakeData.GetTag(2, "Tag2"));
                mainDbContext.SaveChanges();
            }

            return mainDbContext;
        }
    }
}
