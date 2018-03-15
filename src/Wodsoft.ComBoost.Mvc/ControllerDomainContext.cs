using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Wodsoft.ComBoost.Mvc
{
    /// <summary>
    /// 控制器领域上下文。
    /// </summary>
    public class ControllerDomainContext : MvcDomainContext
    {
        /// <summary>
        /// 实例化控制器领域上下文。
        /// </summary>
        /// <param name="controller">控制器。</param>
        public ControllerDomainContext(Controller controller) : base(controller.ControllerContext)
        {
            Controller = controller;
        }

        /// <summary>
        /// 获取控制器。
        /// </summary>
        public Controller Controller { get; private set; }
    }
}
