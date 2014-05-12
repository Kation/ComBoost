using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace System.Security
{
    public class MemberInfo : UserBase, IPassword
    {
        public string Username { get { return (string)GetValue("Username"); } set { SetValue("Username", value); } }

        public string Email { get { return (string)GetValue("Email"); } set { SetValue("Email", value); } }

        public RoleGroup Group { get { return (RoleGroup)GetValue("Group"); } set { SetValue("Group", value); } }
    }
}
