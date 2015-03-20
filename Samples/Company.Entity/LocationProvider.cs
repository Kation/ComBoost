using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Entity
{
    public class LocationProvider : ValueFilter
    {
        public override NameValueCollection GetValues(string dependencyProperty, string dependencyValue)
        {
            NameValueCollection collection = new NameValueCollection();
            switch (dependencyProperty)
            {
                case null:
                    collection.Add("北京", "北京");
                    collection.Add("广东", "广东");
                    collection.Add("广西", "广西");
                    break;
                case "Province":
                    switch(dependencyValue)
                    {
                        case "北京":
                            collection.Add("北京市", "北京市");
                            break;
                        case "广东":
                            collection.Add("广州市", "广州市");
                            collection.Add("佛山市", "佛山市");
                            collection.Add("深圳市", "深圳市");
                            break;
                        case "广西":
                            collection.Add("南宁市", "南宁市");
                            collection.Add("柳州市", "柳州市");
                            collection.Add("桂林市", "桂林市");
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
            return collection;
        }
    }
}
