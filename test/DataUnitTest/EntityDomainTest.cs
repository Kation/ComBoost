using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Wodsoft.ComBoost;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Security;
using System.ComponentModel;
using System.Linq.Expressions;

namespace DataUnitTest
{
    public class EntityDomainTest
    {
        //[Fact]
        //public async Task CreateTest()
        //{
        //    var serviceProvider = DataCommon.GetServiceProvider();
        //    var domainProvider = serviceProvider.GetService<IDomainProvider>();
        //    var userDomain = domainProvider.GetService<EntityDomainService<User>>();

        //    //var task = await userDomain.ExecuteAsync<EntityDomainService<User>, IDatabaseContext, IAuthenticationProvider, IEntityEditModel<User>>(null, userDomain.Create);
        //    //await userDomain.ExecuteAsync<EntityDomainService<User>, IDatabaseContext, IAuthenticationProvider, IEntityEditModel<User>>(null, t => t.Create);
        //}
    }

    //public class Test
    //{
    //    public void Do()
    //    {
    //        Get(this, t => t.TestMethod1);
    //    }
    //    public bool TestMethod1()
    //    {
    //        return true;
    //    }
    //    public bool TestMethod2(string arg1)
    //    {
    //        return false;
    //    }
    //    //public TResult Get<TResult>(Func<TResult> method)
    //    //{
    //    //    return default(TResult);
    //    //}
    //    public TResult Get<T, TResult>(T source, Expression<Func<T, TResult>> method)
    //    {
    //        return default(TResult);
    //    }
    //}
}
