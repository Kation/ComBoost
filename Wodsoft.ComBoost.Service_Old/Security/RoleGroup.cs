using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace System.Security
{
    public class RoleGroup : EntityBase
    {
        /// <summary>
        /// 角色组名称
        /// </summary>
        public string Name { get { return (string)GetValue("Name"); } set { SetValue("Name", value); } }

        /// <summary>
        /// 拥有的角色
        /// </summary>
        public IList<string> Roles { get { return (IList<string>)GetValue("Roles"); } set { SetValue("Roles", value); } }

        /// <summary>
        /// 子角色组
        /// </summary>
        public virtual ICollection<RoleGroup> Children { get { return (ICollection<RoleGroup>)GetValue("Children"); } set { SetValue("Children", value); } }

        /// <summary>
        /// 父角色组
        /// </summary>
        public virtual RoleGroup Parent { get { return (RoleGroup)GetValue("Parent"); } set { SetValue("Parent", value); } }
    }
}
