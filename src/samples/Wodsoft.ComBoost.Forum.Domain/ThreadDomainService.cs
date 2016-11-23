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
    public class ThreadDomainService<T> : DomainService
        where T : class, IThread, new()
    {
        public async Task<T> Create([FromService]IAuthenticationProvider authentication, [FromService]IDatabaseContext databaseContext, [FromEntity]IForum forum, [FromValue] string title, [FromValue] string content)
        {
            //TODO: 权限判断

            //获取主题实体上下文
            var threadContext = databaseContext.GetContext<T>();
            //创建主题实体
            var thread = threadContext.Create();
            //给主题赋值，所属板块、标题、创建者
            thread.Forum = forum;
            thread.Title = title;
            thread.Member = authentication.GetAuthentication().GetUser<IMember>();
            //添加到表
            threadContext.Add(thread);
            //获取帖子实体上下文
            var postContext = databaseContext.GetWrappedContext<IPost>();
            //创建帖子
            var post = postContext.Create();
            //给帖子赋值，所属主题、内容、创建者
            post.Thread = thread;
            post.Content = content;
            post.Member = thread.Member;
            //添加到表
            postContext.Add(post);
            //保存数据库更改
            await databaseContext.SaveAsync();
            //返回主题实体
            return thread;
        }

        public async Task Remove([FromService]IDatabaseContext databaseContext, [FromEntity]T thread)
        {
            var threadContext = databaseContext.GetContext<T>();
            threadContext.Remove(thread);
            await databaseContext.SaveAsync();
        }
    }
}
