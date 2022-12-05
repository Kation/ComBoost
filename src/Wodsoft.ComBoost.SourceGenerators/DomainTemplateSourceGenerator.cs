using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Wodsoft.ComBoost
{
    [Generator]
    public class DomainTemplateSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            List<string> files = new List<string>();
            if (context.SyntaxReceiver is AutoTemplateSyntaxReceiver receiver)
            {
                foreach (var attrSyntax in receiver.Attributes)
                {
                    var model = context.Compilation.GetSemanticModel(attrSyntax.SyntaxTree);
                    if (!SyntaxHelper.IsSameFullName(attrSyntax.Name, "Wodsoft.ComBoost.AutoTemplateAttribute", model))
                        continue;
                    var classSyntax = attrSyntax.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                    if (classSyntax == null)
                        continue;
                    bool isPartial = classSyntax.Modifiers.Any(t => t.IsKind(SyntaxKind.PartialKeyword));
                    var node = classSyntax.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
                    string ns = null, oldNS = null;
                    while (node != null)
                    {
                        if (ns == null)
                            ns = node.Name.ToString();
                        else
                            ns = node.Name.ToString() + "." + ns;
                        node = node.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>(t => t != node);
                    }
                    var name = "I" + classSyntax.Identifier.ValueText;
                    string group = null;
                    if (attrSyntax.ArgumentList != null && attrSyntax.ArgumentList.Arguments.Count != 0)
                    {
                        foreach (var argumentSyntax in attrSyntax.ArgumentList.Arguments)
                        {
                            if (argumentSyntax.NameEquals != null && argumentSyntax.Expression is LiteralExpressionSyntax literal)
                            {
                                switch (argumentSyntax.NameEquals.Name.Identifier.ValueText)
                                {
                                    case "Namespace":
                                        oldNS = ns;
                                        ns = literal.Token.ValueText;
                                        break;
                                    case "TemplateName":
                                        name = literal.Token.ValueText;
                                        break;
                                    case "Group":
                                        group = literal.Token.ValueText;
                                        break;
                                }
                            }
                        }
                    }
                    var builder = new StringBuilder();
                    builder.AppendLine("// ComBoost auto generated domain template interface.");
                    foreach (UsingDirectiveSyntax item in ((Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax)attrSyntax.SyntaxTree.GetRoot()).Usings)
                    {
                        if (!item.StaticKeyword.IsKind(SyntaxKind.None))
                            continue;
                        if (item.Alias != null)
                            builder.AppendLine($"using {item.Alias.Name} = {item.Name};");
                        else
                        {
                            builder.AppendLine($"using {item.Name};");
                        }
                    }
                    if (oldNS != null)
                        builder.AppendLine($"using {oldNS};");
                    builder.AppendLine();
                    builder.AppendLine("namespace " + ns);
                    builder.AppendLine("{");
                    builder.AppendLine($"    public partial interface {name} : global::Wodsoft.ComBoost.IDomainTemplate");
                    builder.AppendLine("    {");
                    var methods = classSyntax.Members.OfType<MethodDeclarationSyntax>().Select(method =>
                    {
                        AttributeSyntax attr;
                        var query = method.AttributeLists.SelectMany(t => t.Attributes).Where(t => SyntaxHelper.IsSameFullName(t.Name, "Wodsoft.ComBoost.AutoTemplateMethodAttribute", model));
                        if (group == null)
                            attr = query.FirstOrDefault(t => t.ArgumentList == null || t.ArgumentList.Arguments.Count == 0 || t.ArgumentList.Arguments.All(x => x.NameEquals.Name.Identifier.ValueText != "Group"));
                        else
                            attr = query.FirstOrDefault(t => t.ArgumentList != null && t.ArgumentList.Arguments.Any(x => x.NameEquals != null &&
                                                        x.NameEquals.Name.Identifier.ValueText == "Group" &&
                                                        x.Expression is LiteralExpressionSyntax literal && literal.Token.ValueText == group));
                        bool? isIncluded = null, isExcluded = null;
                        if (attr != null)
                        {
                            var expression = attr.ArgumentList?.Arguments.FirstOrDefault(t => t.NameEquals != null && t.NameEquals.Name.Identifier.ValueText == "IsIncluded")?.Expression;
                            if (expression != null)
                                isIncluded = expression.ToString() == "true";
                            expression = attr.ArgumentList?.Arguments.FirstOrDefault(t => t.NameEquals != null && t.NameEquals.Name.Identifier.ValueText == "IsExcluded")?.Expression;
                            if (expression != null)
                                isExcluded = expression.ToString() == "true";
                        }
                        return new
                        {
                            Method = method,
                            IsIncluded = isIncluded,
                            IsExcluded = isExcluded
                        };
                    });
                    if (methods.Any())
                    {
                        if (methods.Any(t => t.IsIncluded == true))
                            methods = methods.Where(t => t.IsIncluded == true);
                        else if (methods.Any(t => t.IsExcluded == true))
                            methods = methods.Where(t => t.IsExcluded != true);
                        foreach (var methodSyntax in methods.Select(t => t.Method))
                        {
                            if ((methodSyntax.Modifiers.Count == 1 && methodSyntax.Modifiers[0].IsKind(SyntaxKind.PublicKeyword)) ||
                                (methodSyntax.Modifiers.Count == 2 && methodSyntax.Modifiers.Any(t => t.IsKind(SyntaxKind.PublicKeyword)) && methodSyntax.Modifiers.Any(t => t.IsKind(SyntaxKind.AsyncKeyword))))
                            {
                                if (SyntaxHelper.IsSameFullName(methodSyntax.ReturnType, "System.Threading.Tasks.Task", model))
                                {
                                    builder.Append($"        {methodSyntax.ReturnType} {methodSyntax.Identifier.Text}(");
                                    bool prepend = false;
                                    foreach (var parameter in methodSyntax.ParameterList.Parameters)
                                    {
                                        if (parameter.AttributeLists.Count == 0 ||
                                            parameter.AttributeLists.SelectMany(t => t.Attributes)
                                            .Any(t => SyntaxHelper.IsSameFullName(t.Name, "Wodsoft.ComBoost.FromValueAttribute", model)))
                                        {
                                            if (prepend)
                                                builder.Append(", ");
                                            builder.Append($"{parameter.Type} {parameter.Identifier.Text}");
                                            prepend = true;
                                        }
                                    }
                                    builder.AppendLine(");");
                                }
                            }
                        }
                    }
                    builder.AppendLine("    }");
                    if (isPartial && oldNS == null)
                    {
                        builder.AppendLine();
                        builder.AppendLine($"    [global::Wodsoft.ComBoost.DomainTemplateImplementer(typeof({name}))]");
                        builder.AppendLine($"    {classSyntax.Modifiers} class {classSyntax.Identifier.ValueText} {{ }}");
                    }
                    builder.Append("}");
                    if (isPartial && oldNS != null)
                    {
                        builder.AppendLine();
                        builder.AppendLine();
                        builder.AppendLine("namespace " + oldNS);
                        builder.AppendLine("{");
                        builder.AppendLine($"    [global::Wodsoft.ComBoost.DomainTemplateImplementer(typeof(global::{ns}.{name}))]");
                        builder.AppendLine($"    {classSyntax.Modifiers} class {classSyntax.Identifier.ValueText} {{ }}");
                        builder.Append("}");
                    }
                    var filename = $"{ns}.{name}.g.cs";
                    if (files.Contains(filename))
                        continue;
                    files.Add(filename);
                    context.AddSource(filename, builder.ToString());
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new AutoTemplateSyntaxReceiver());
        }
    }
}
