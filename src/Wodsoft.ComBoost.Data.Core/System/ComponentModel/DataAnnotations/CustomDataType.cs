using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 自定义数据类型。
    /// </summary>
    public enum CustomDataType
    {
        /// <summary>
        /// 默认。
        /// </summary>
        Default = 0,
        /// <summary>
        /// 日期与时间。
        /// </summary>
        DateTime = 1,
        /// <summary>
        /// 仅日期。
        /// </summary>
        Date = 2,
        /// <summary>
        /// 仅时间（Timespan）。
        /// </summary>
        Time = 3,
        /// <summary>
        /// 性别。
        /// </summary>
        Gender = 4,
        /// <summary>
        /// 电话号码。
        /// </summary>
        PhoneNumber = 5,
        /// <summary>
        /// 货币。
        /// </summary>
        Currency = 6,
        /// <summary>
        /// 单行文本。
        /// </summary>
        Text = 7,
        /// <summary>
        /// Html内容。
        /// </summary>
        Html = 8,
        /// <summary>
        /// 多行文本。
        /// </summary>
        MultilineText = 9,
        /// <summary>
        /// 电子邮箱地址。
        /// </summary>
        EmailAddress = 10,
        /// <summary>
        /// 密码。
        /// </summary>
        Password = 11,
        /// <summary>
        /// Url地址。
        /// </summary>
        Url = 12,
        /// <summary>
        /// 图片地址。
        /// </summary>
        ImageUrl = 13,
        /// <summary>
        /// 是否。
        /// </summary>
        Boolean = 14,
        /// <summary>
        /// 整数。
        /// </summary>
        Integer = 15,
        /// <summary>
        /// 数字。
        /// </summary>
        Number = 16,
        /// <summary>
        /// 图片。
        /// </summary>
        Image = 17,
        /// <summary>
        /// 文件。
        /// </summary>
        File = 18,
        /// <summary>
        /// 其它自定义类型。
        /// </summary>
        Other = 19,
    }
}
