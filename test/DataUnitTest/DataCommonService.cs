using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Data.Entity;

namespace DataUnitTest
{
    public class DataCommonService : DomainService
    {
        public static async Task DataInit([FromService]IDatabaseContext databaseContext)
        {
            var categoryContext = databaseContext.GetContext<Category>();
            var category = categoryContext.Create();
            category.Name = "TestCategory";
            categoryContext.Add(category);
            var userContext = databaseContext.GetContext<User>();
            var user1 = userContext.Create();
            user1.Username = "TestUser1";
            user1.Category = category;
            userContext.Add(user1);
            var user2 = userContext.Create();
            user2.Username = "TestUser2";
            user2.Category = category;
            userContext.Add(user2);
            await databaseContext.SaveAsync();
        }
    }
}
