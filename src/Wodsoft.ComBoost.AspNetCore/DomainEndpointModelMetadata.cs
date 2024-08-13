#if !NETCOREAPP2_1
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost.AspNetCore
{
    public class DomainEndpointModelMetadata : ModelMetadata
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public DomainEndpointModelMetadata(ModelMetadataIdentity identity) : base(identity)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            IsBindingAllowed = true;
        }

        public override IReadOnlyDictionary<object, object> AdditionalValues { get; }

        public override ModelPropertyCollection Properties { get; }

        public override string? BinderModelName { get; }

        public override Type? BinderType { get; }

        public override BindingSource? BindingSource { get; }

        public override bool ConvertEmptyStringToNull { get; }

        public override string? DataTypeName { get; }

        public override string? Description { get; }

        public override string? DisplayFormatString { get; }

        public override string? DisplayName { get; }

        public override string? EditFormatString { get; }

        public override ModelMetadata? ElementMetadata { get; }

        public override IEnumerable<KeyValuePair<EnumGroupAndName, string>>? EnumGroupedDisplayNamesAndValues { get; }

        public override IReadOnlyDictionary<string, string>? EnumNamesAndValues { get; }

        public override bool HasNonDefaultEditFormat { get; }

        public override bool HtmlEncode { get; }

        public override bool HideSurroundingHtml { get; }

        public override bool IsBindingAllowed { get; }

        public override bool IsBindingRequired { get; }

        public override bool IsEnum { get; }

        public override bool IsFlagsEnum { get; }

        public override bool IsReadOnly { get; }

        public override bool IsRequired { get; }

        public override ModelBindingMessageProvider ModelBindingMessageProvider { get; }

        public override int Order { get; }

        public override string? Placeholder { get; }

        public override string? NullDisplayText { get; }

        public override IPropertyFilterProvider? PropertyFilterProvider { get; }

        public override bool ShowForDisplay { get; }

        public override bool ShowForEdit { get; }

        public override string? SimpleDisplayProperty { get; }

        public override string? TemplateHint { get; }

        public override bool ValidateChildren { get; }

        public override IReadOnlyList<object> ValidatorMetadata { get; }

        public override Func<object, object?>? PropertyGetter { get; }

        public override Action<object, object?>? PropertySetter { get; }
    }
}
#endif