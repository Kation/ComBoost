//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Wodsoft.ComBoost.Security;

//namespace Wodsoft.ComBoost.Mock
//{
//    public class MockSecurityProvider<TUser> : GeneralSecurityProvider, ICollection<TUser>
//        where TUser : IPermission
//    {
//        private Dictionary<string, TUser> _Identities;

//        public MockSecurityProvider()
//        {
//            _Identities = new Dictionary<string, TUser>();
//        }

//        public int Count { get { return _Identities.Count; } }

//        public bool IsReadOnly { get { return false; } }

//        public void Add(TUser item)
//        {
//            if (_Identities.ContainsKey(item.Identity))
//                throw new InvalidOperationException("已存在相同的Id。");
//            _Identities.Add(item.Identity, item);
//        }

//        public void Clear()
//        {
//            _Identities.Clear();
//        }

//        public bool Contains(TUser item)
//        {
//            return _Identities.ContainsKey(item.Identity);
//        }

//        public void CopyTo(TUser[] array, int arrayIndex)
//        {
//            _Identities.Values.CopyTo(array, arrayIndex);
//        }

//        public IEnumerator<TUser> GetEnumerator()
//        {
//            return _Identities.Values.GetEnumerator();
//        }

//        public bool Remove(TUser item)
//        {
//            return _Identities.Remove(item.Identity);
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return _Identities.Values.GetEnumerator();
//        }

//        protected override Task<IPermission> GetPermissionByIdentity(string identity)
//        {
//            TUser user;
//            _Identities.TryGetValue(identity, out user);
//            return Task.FromResult<IPermission>(user);
//        }

//        protected override Task<IPermission> GetPermissionByUsername(string username)
//        {
//            throw new NotSupportedException();
//        }
//    }
//}
