using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Forum.Domain
{
    public class PostDomainService<T> : DomainService
        where T : class, IPost, new()
    {

        public async Task<T> Create([FromService]IAuthenticationProvider authentication, [FromService]IDatabaseContext databaseContext, [FromEntity]IThread thread, [FromValue] string content)
        {
            var postContext = databaseContext.GetContext<T>();
            var post = postContext.Create();
            post.Content = content;
            post.Thread = thread;
            post.Member = authentication.GetAuthentication().GetUser<IMember>();
            postContext.Add(post);
            await databaseContext.SaveAsync();
            return post;
        }
    }
}
