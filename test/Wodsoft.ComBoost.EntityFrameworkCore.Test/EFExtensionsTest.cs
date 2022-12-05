//using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Data.Linq;
using Wodsoft.ComBoost.Test;
using Wodsoft.ComBoost.Test.Entities;
using Xunit;

namespace Wodsoft.ComBoost.EntityFrameworkCore.Test
{
    public class EFExtensionsTest
    {
        private IDatabaseContext SeedData([CallerMemberName] string callerName = null)
        {
            var dataContext = new DataContext();
            dataContext.Tests.Add(new ComBoost.Test.Entities.TestEntity
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTimeOffset.Now,
                ModificationDate = DateTimeOffset.Now,
                ValueInt = 1,
                ValueLong = 1,
                ValueFloat = 1,
                ValueDouble = 2,
                ValueDecimal = 1
            });
            dataContext.Tests.Add(new ComBoost.Test.Entities.TestEntity
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTimeOffset.Now,
                ModificationDate = DateTimeOffset.Now,
                ValueInt = 1,
                ValueLong = 1,
                ValueFloat = 1,
                ValueDouble = 1,
                ValueDecimal = 1
            });
            dataContext.Tests.Add(new ComBoost.Test.Entities.TestEntity
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTimeOffset.Now,
                ModificationDate = DateTimeOffset.Now,
                ValueInt = 2,
                ValueLong = 2,
                ValueFloat = 2,
                ValueDouble = 2,
                ValueDecimal = 2
            });
            dataContext.Tests.Add(new ComBoost.Test.Entities.TestEntity
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTimeOffset.Now,
                ModificationDate = DateTimeOffset.Now,
                ValueInt = 3,
                ValueLong = 3,
                ValueFloat = 3,
                ValueDouble = 3,
                ValueDecimal = 3
            });
            dataContext.Tests.Add(new ComBoost.Test.Entities.TestEntity
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTimeOffset.Now,
                ModificationDate = DateTimeOffset.Now,
                ValueInt = 4,
                ValueLong = 4,
                ValueFloat = 4,
                ValueDouble = 4,
                ValueDecimal = 4
            });
            dataContext.Tests.Add(new ComBoost.Test.Entities.TestEntity
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTimeOffset.Now,
                ModificationDate = DateTimeOffset.Now,
                ValueInt = 5,
                ValueLong = 5,
                ValueFloat = 5,
                ValueDouble = 5,
                ValueDecimal = 5
            });
            dataContext.Tests.Add(new ComBoost.Test.Entities.TestEntity
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTimeOffset.Now,
                ModificationDate = DateTimeOffset.Now,
                ValueInt = 5,
                ValueLong = 5,
                ValueFloat = 5,
                ValueDouble = 4,
                ValueDecimal = 5,
                Include = new IncludeEntity
                {
                    Id = Guid.NewGuid(),
                    Text = "include",
                    ThenInclude = new ThenIncludeEntity
                    {
                        Id = Guid.NewGuid(),
                        Text = "theninclude"
                    }
                },
                Items = new List<SubItemEntity>
                {
                    new SubItemEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test1"
                    },
                    new SubItemEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test2"
                    }
                }
            });
            dataContext.SaveChanges();

            return new DatabaseContext<DataContext>(dataContext);
        }

        [Fact]
        public async Task MaxTest()
        {
            var databaseContext = SeedData();
            var entityContext = databaseContext.GetContext<TestEntity>();
            Assert.Equal(5, await entityContext.Query().MaxAsync(t => t.ValueInt));
            Assert.Equal(5, await entityContext.Query().MaxAsync(t => t.ValueLong));
            Assert.Equal(5, await entityContext.Query().MaxAsync(t => t.ValueDecimal));
            Assert.Equal(5, await entityContext.Query().MaxAsync(t => t.ValueFloat));
            Assert.Equal(5, await entityContext.Query().MaxAsync(t => t.ValueInt));
            Assert.Equal(5, await entityContext.Query().Select(t => t.ValueInt).MaxAsync());
            Assert.Equal(5, await entityContext.Query().Select(t => t.ValueLong).MaxAsync());
            Assert.Equal(5, await entityContext.Query().Select(t => t.ValueDecimal).MaxAsync());
            Assert.Equal(5, await entityContext.Query().Select(t => t.ValueFloat).MaxAsync());
            Assert.Equal(5, await entityContext.Query().Select(t => t.ValueInt).MaxAsync());
        }

        [Fact]
        public async Task MinTest()
        {
            var databaseContext = SeedData();
            var entityContext = databaseContext.GetContext<TestEntity>();
            Assert.Equal(1, await entityContext.Query().MinAsync(t => t.ValueInt));
            Assert.Equal(1, await entityContext.Query().MinAsync(t => t.ValueLong));
            Assert.Equal(1, await entityContext.Query().MinAsync(t => t.ValueDecimal));
            Assert.Equal(1, await entityContext.Query().MinAsync(t => t.ValueFloat));
            Assert.Equal(1, await entityContext.Query().MinAsync(t => t.ValueInt));
            Assert.Equal(1, await entityContext.Query().Select(t => t.ValueInt).MinAsync());
            Assert.Equal(1, await entityContext.Query().Select(t => t.ValueLong).MinAsync());
            Assert.Equal(1, await entityContext.Query().Select(t => t.ValueDecimal).MinAsync());
            Assert.Equal(1, await entityContext.Query().Select(t => t.ValueFloat).MinAsync());
            Assert.Equal(1, await entityContext.Query().Select(t => t.ValueInt).MinAsync());
        }

        [Fact]
        public async Task AverageTest()
        {
            var databaseContext = SeedData();
            var entityContext = databaseContext.GetContext<TestEntity>();
            Assert.Equal(3, await entityContext.Query().AverageAsync(t => t.ValueInt));
            Assert.Equal(3, await entityContext.Query().AverageAsync(t => t.ValueLong));
            Assert.Equal(3, await entityContext.Query().AverageAsync(t => t.ValueDecimal));
            Assert.Equal(3, await entityContext.Query().AverageAsync(t => t.ValueFloat));
            Assert.Equal(3, await entityContext.Query().AverageAsync(t => t.ValueInt));
            Assert.Equal(3, await entityContext.Query().Select(t => t.ValueInt).AverageAsync());
            Assert.Equal(3, await entityContext.Query().Select(t => t.ValueLong).AverageAsync());
            Assert.Equal(3, await entityContext.Query().Select(t => t.ValueDecimal).AverageAsync());
            Assert.Equal(3, await entityContext.Query().Select(t => t.ValueFloat).AverageAsync());
            Assert.Equal(3, await entityContext.Query().Select(t => t.ValueInt).AverageAsync());
        }

        [Fact]
        public async Task SumTest()
        {
            var databaseContext = SeedData();
            var entityContext = databaseContext.GetContext<TestEntity>();
            Assert.Equal(21, await entityContext.Query().SumAsync(t => t.ValueInt));
            Assert.Equal(21, await entityContext.Query().SumAsync(t => t.ValueLong));
            Assert.Equal(21, await entityContext.Query().SumAsync(t => t.ValueDecimal));
            Assert.Equal(21, await entityContext.Query().SumAsync(t => t.ValueFloat));
            Assert.Equal(21, await entityContext.Query().SumAsync(t => t.ValueInt));
            Assert.Equal(21, await entityContext.Query().Select(t => t.ValueInt).SumAsync());
            Assert.Equal(21, await entityContext.Query().Select(t => t.ValueLong).SumAsync());
            Assert.Equal(21, await entityContext.Query().Select(t => t.ValueDecimal).SumAsync());
            Assert.Equal(21, await entityContext.Query().Select(t => t.ValueFloat).SumAsync());
            Assert.Equal(21, await entityContext.Query().Select(t => t.ValueInt).SumAsync());
        }

        [Fact]
        public async Task ToArrayTest()
        {
            var databaseContext = SeedData();
            var entityContext = databaseContext.GetContext<TestEntity>();
            Assert.Equal(7, (await entityContext.Query().ToArrayAsync()).Length);
        }

        [Fact]
        public async Task ToListTest()
        {
            var databaseContext = SeedData();
            var entityContext = databaseContext.GetContext<TestEntity>();
            Assert.Equal(7, (await entityContext.Query().ToListAsync()).Count);
        }

        [Fact]
        public async Task ToDictionaryTest()
        {
            var databaseContext = SeedData();
            var entityContext = databaseContext.GetContext<TestEntity>();
            Assert.Equal(3, (await entityContext.Query().Where(t => t.ValueInt > 1 && t.ValueInt < 5).ToDictionaryAsync(t => t.ValueInt)).Count);
            Assert.Equal(3, (await entityContext.Query().Where(t => t.ValueInt > 1 && t.ValueInt < 5).ToDictionaryAsync(t => t.ValueInt, t => t.ValueInt)).Count);
        }

        [Fact]
        public async Task AllTest()
        {
            var databaseContext = SeedData();
            var entityContext = databaseContext.GetContext<TestEntity>();
            Assert.False(await entityContext.Query().AllAsync(t => t.ValueInt == 1));
        }

        [Fact]
        public async Task AnyTest()
        {
            var databaseContext = SeedData();
            var entityContext = databaseContext.GetContext<TestEntity>();
            Assert.True(await entityContext.Query().AnyAsync(t => t.ValueInt == 1));
            Assert.True(await entityContext.Query().AnyAsync());
        }

        [Fact]
        public async Task CountTest()
        {
            var databaseContext = SeedData();
            var entityContext = (EntityContext<TestEntity>)databaseContext.GetContext<TestEntity>();
            Assert.Equal(7, await entityContext.Query().CountAsync());
            Assert.Equal(2, await entityContext.Query().CountAsync(t => t.ValueInt == 1));
            Assert.Equal(7, await entityContext.Query().LongCountAsync());
            Assert.Equal(2, await entityContext.Query().LongCountAsync(t => t.ValueInt == 5));
        }

        [Fact]
        public async Task SingleTest()
        {
            var databaseContext = SeedData();
            var entityContext = (EntityContext<TestEntity>)databaseContext.GetContext<TestEntity>();
            Assert.NotNull(await entityContext.Query().Where(t => t.ValueInt == 2).SingleAsync());
            Assert.NotNull(await entityContext.Query().SingleAsync(t => t.ValueInt == 2));
            Assert.Null(await entityContext.Query().Where(t => t.ValueInt == 6).SingleOrDefaultAsync());
            Assert.NotNull(await entityContext.Query().SingleOrDefaultAsync(t => t.ValueInt == 2));
        }

        [Fact]
        public async Task FirstTest()
        {
            var databaseContext = SeedData();
            var entityContext = (EntityContext<TestEntity>)databaseContext.GetContext<TestEntity>();
            Assert.Equal(1, (await entityContext.Query().OrderBy(t => t.ValueInt).FirstAsync()).ValueInt);
            Assert.Equal(1, (await entityContext.Query().FirstAsync(t => t.ValueInt == 1)).ValueInt);
            Assert.NotNull(await entityContext.Query().FirstOrDefaultAsync());
            Assert.NotNull(await entityContext.Query().FirstOrDefaultAsync(t => t.ValueInt == 1));
        }

        [Fact]
        public async Task LastTest()
        {
            var databaseContext = SeedData();
            var entityContext = (EntityContext<TestEntity>)databaseContext.GetContext<TestEntity>();
            Assert.Equal(5, (await entityContext.Query().OrderBy(t => t.ValueInt).LastAsync()).ValueInt);
            Assert.Equal(1, (await entityContext.Query().LastAsync(t => t.ValueInt == 1)).ValueInt);
            Assert.NotNull(await entityContext.Query().LastOrDefaultAsync());
            Assert.NotNull(await entityContext.Query().LastOrDefaultAsync(t => t.ValueInt == 1));
        }

        [Fact]
        public async Task IncludeTest()
        {
            var databaseContext = SeedData();
            var entityContext = (EntityContext<TestEntity>)databaseContext.GetContext<TestEntity>();
            var items = await entityContext.Query().Include(t => t.Include).ThenInclude(t => t.ThenInclude).OrderBy(t => t.ValueInt).ThenBy(t => t.ValueDouble).ToArrayAsync();
            Assert.NotNull(items[5].Include);
            Assert.NotNull(items[5].Include.ThenInclude);
        }

        [Fact]
        public async Task AsTrackingTest()
        {
            var databaseContext = SeedData();
            var entityContext = databaseContext.GetContext<TestEntity>();
            await entityContext.Query().AsTracking().ToArrayAsync();
        }

        [Fact]
        public async Task AsNoTrackingTest()
        {
            var databaseContext = SeedData();
            var entityContext = databaseContext.GetContext<TestEntity>();
            await entityContext.Query().AsNoTracking().ToArrayAsync();
        }
    }
}
