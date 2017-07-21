using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost
{
    /// <summary>
    /// 领域服务事件路由。
    /// </summary>
    public sealed class DomainServiceEventRoute
    {
        /// <summary>
        /// 注册同步事件。
        /// </summary>
        /// <returns></returns>
        public static DomainServiceEventRoute RegisterEvent()
        {
            DomainServiceEventRoute route = new DomainServiceEventRoute(typeof(DomainServiceEventHandler));
            return route;
        }

        /// <summary>
        /// 注册同步事件。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <returns></returns>
        public static DomainServiceEventRoute RegisterEvent<T>()
            where T :EventArgs
        {
            DomainServiceEventRoute route = new DomainServiceEventRoute(typeof(DomainServiceEventHandler<T>));
            return route;
        }

        /// <summary>
        /// 注册异步事件。
        /// </summary>
        /// <returns></returns>
        public static DomainServiceEventRoute RegisterAsyncEvent()
        {
            DomainServiceEventRoute route = new DomainServiceEventRoute(typeof(DomainServiceAsyncEventHandler));
            return route;
        }

        /// <summary>
        /// 注册异步事件。
        /// </summary>
        /// <typeparam name="T">事件参数类型。</typeparam>
        /// <returns></returns>
        public static DomainServiceEventRoute RegisterAsyncEvent<T>()
            where T : EventArgs
        {
            DomainServiceEventRoute route = new DomainServiceEventRoute(typeof(DomainServiceAsyncEventHandler<T>));
            return route;
        }
        
        private DomainServiceEventRoute(Type handlerType)
        {
            HandlerType = handlerType;
            DomainServiceEventManager.GlobalEventManager.RegisterEventRoute(this);
        }

        /// <summary>
        /// 获取事件委托类型。
        /// </summary>
        public Type HandlerType { get; private set; }
    }
}
