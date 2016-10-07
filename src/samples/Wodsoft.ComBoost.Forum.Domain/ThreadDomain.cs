using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Data;
using Wodsoft.ComBoost.Data.Entity;
using Wodsoft.ComBoost.Forum.Core;
using Wodsoft.ComBoost.Security;

namespace Wodsoft.ComBoost.Forum.Domain
{
    public class ThreadDomain : DomainService
    {
        public async Task<IThread> Create([FromService]IAuthenticationProvider authentication, [FromService]IDatabaseContext databaseContext, [FromEntity]IForum forum, [FromValue] string title, [FromValue] string content)
        {
            //TODO: 权限判断

            var threadContext = databaseContext.GetWrappedContext<IThread>();
            var thread = threadContext.Create();
            thread.Forum = forum;
            thread.Title = title;
            thread.Member = authentication.GetAuthentication().GetUser<IMember>();
            threadContext.Add(thread);
            var postContext = databaseContext.GetWrappedContext<IPost>();
            var post = postContext.Create();
            post.Content = content;
            post.Member = thread.Member;
            postContext.Add(post);
            await databaseContext.SaveAsync();
            return thread;
        }

        public async Task Remove([FromService]IDatabaseContext databaseContext, [FromEntity]IThread thread)
        {
            var threadContext = databaseContext.GetWrappedContext<IThread>();
            threadContext.Remove(thread);
            await databaseContext.SaveAsync();
        }
    }
}
