using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Custom data type.
    /// </summary>
    public enum CustomDataType
    {
        /// <summary>
        /// Default.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Date and time.
        /// </summary>
        DateTime = 1,
        /// <summary>
        /// Date only.
        /// </summary>
        Date = 2,
        /// <summary>
        /// Time only(Timespan).
        /// </summary>
        Time = 3,
        /// <summary>
        /// Sex.
        /// </summary>
        Sex = 4,
        /// <summary>
        /// Phone number.
        /// </summary>
        PhoneNumber = 5,
        /// <summary>
        /// Currency.
        /// </summary>
        Currency = 6,
        /// <summary>
        /// Single-line text.
        /// </summary>
        Text = 7,
        /// <summary>
        /// Html content.
        /// </summary>
        Html = 8,
        /// <summary>
        /// Multiline text.
        /// </summary>
        MultilineText = 9,
        /// <summary>
        /// Email address.
        /// </summary>
        EmailAddress = 10,
        /// <summary>
        /// Password.
        /// </summary>
        Password = 11,
        /// <summary>
        /// Url address.
        /// </summary>
        Url = 12,
        /// <summary>
        /// Image url address.
        /// </summary>
        ImageUrl = 13,
        /// <summary>
        /// Boolean.
        /// </summary>
        Boolean = 14,
        /// <summary>
        /// Integer.
        /// </summary>
        Integer = 15,
        /// <summary>
        /// Number.
        /// </summary>
        Number = 16,
        /// <summary>
        /// Image data.
        /// </summary>
        Image = 17,
        /// <summary>
        /// File data.
        /// </summary>
        File = 18,
        /// <summary>
        /// Define in Custom property.
        /// </summary>
        Other = 19,
    }
}
