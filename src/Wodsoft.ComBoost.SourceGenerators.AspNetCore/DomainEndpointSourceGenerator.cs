using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Wodsoft.ComBoost
{
    [Generator]
    public class DomainEndpointSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            List<string> files = new List<string>();
            if (context.SyntaxReceiver is DomainEndpointSyntaxReceiver receiver)
            {
                foreach (var attrSyntax in receiver.Attributes)
                {
                    var model = context.Compilation.GetSemanticModel(attrSyntax.SyntaxTree);
                    if (!SyntaxHelper.IsSameFullName(attrSyntax.Name, "Wodsoft.ComBoost.AspNetCore.DomainEndpointAttribute", model))
                        continue;
                    var classSyntax = attrSyntax.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                    if (classSyntax == null)
                        continue;
                    var classType = model.GetDeclaredSymbol(classSyntax);
                    if (!IsDomainEndpointType(classType.BaseType, out var templateType))
                        continue;
                    if (!classSyntax.Modifiers.Any(t => t.IsKind(SyntaxKind.PartialKeyword)))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("CBANC001", "Class must be partial.", "A domain endpoint class must be partial to generate codes.", "Wodsoft.ComBoost.AspNetCore", DiagnosticSeverity.Error, true)
                            , classSyntax.GetLocation()));
                        continue;
                    }
                    if (templateType.GetMembers().GroupBy(t => t.Name).Any(t => t.Count() != 1))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("CBANC002", "Domain template contains overload methods.", "A domain template binded to endpoint cannot contains overload methods. ASP.NET Core could not recognize which method should bind.", "Wodsoft.ComBoost.AspNetCore", DiagnosticSeverity.Error, true)
                            , classSyntax.GetLocation()));
                        continue;
                    }
                    string ns = classType.ContainingNamespace.ToString();
                    string name = classSyntax.Identifier.ValueText;
                    var builder = new StringBuilder();
                    builder.AppendLine("// ComBoost auto generated domain endpoint codes.");
                    builder.AppendLine("using Microsoft.AspNetCore.Http;");
                    builder.AppendLine("using System;");
                    builder.AppendLine("using System.Threading.Tasks;");
                    builder.AppendLine("namespace " + ns);
                    builder.AppendLine("{");
                    builder.AppendLine($"    public partial class {name}");
                    builder.AppendLine("    {");
                    builder.AppendLine($"        protected override async Task HandleRequest(HttpContext httpContext, {GetTypeFullText((INamedTypeSymbol)templateType)} domainTemplate, string method)");
                    builder.AppendLine("        {");
                    builder.AppendLine("            switch (method.ToLower())");
                    builder.AppendLine("            {");
                    bool fail = false;
                    foreach (var member in templateType.GetMembers().OfType<IMethodSymbol>())
                    {
                        builder.AppendLine($"                case \"{member.Name.ToLower()}\":");
                        builder.AppendLine("                {");
                        string httpMethod;
                        if (member.Name.StartsWith("Get"))
                            httpMethod = "GET";
                        else if (member.Name.StartsWith("Post") || member.Name.StartsWith("Insert") || member.Name.StartsWith("Create"))
                            httpMethod = "POST";
                        else if (member.Name.StartsWith("Update") || member.Name.StartsWith("Edit"))
                            httpMethod = "PUT";
                        else if (member.Name.StartsWith("Delete") || member.Name.StartsWith("Remove"))
                            httpMethod = "DELETE";
                        else
                            httpMethod = "POST";
                        if (httpMethod != "GET" && member.Parameters.Length != 1)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("CBANC003", "Domain template contains not support methods.", "Domain template contains methods which is non HTTP GET and have more than one parameters.", "Wodsoft.ComBoost.AspNetCore", DiagnosticSeverity.Error, true)
                                , classSyntax.GetLocation()));
                            fail = true;
                            break;
                        }
                        foreach (var parameter in member.Parameters)
                        {
                            builder.AppendLine($"                    if (httpContext.Request.Method != \"{httpMethod}\")");
                            builder.AppendLine("                    {");
                            builder.AppendLine("                        httpContext.Response.StatusCode = 405;");
                            builder.AppendLine("                        break;");
                            builder.AppendLine("                    }");
                            var parameterType = (INamedTypeSymbol)parameter.Type;
                            if (httpMethod == "GET")
                            {
                                builder.Append($"                    var {parameter.Name} = ");
                                if (parameterType.IsGenericType && parameterType.ContainingNamespace.ToString() == "System.Collections.Generic" && parameterType.Name == "List")
                                {
                                    builder.AppendLine($"GetQueryListValue<{GetTypeFullText((INamedTypeSymbol)parameterType.TypeArguments[0])}>(httpContext, \"{parameter.Name}\");");
                                }
                                else if (parameterType.TypeKind == TypeKind.Array)
                                {
                                    builder.AppendLine($"GetQueryArrayValue<{GetTypeFullText(parameterType)}>(httpContext, \"{parameter.Name}\");");
                                }
                                else
                                {
                                    if (parameterType.ContainingNamespace.ToString() == "System" && parameterType.Name == "Nullable")
                                        builder.AppendLine($"GetQueryNullableValue<{GetTypeFullText((INamedTypeSymbol)parameterType.TypeArguments[0])}>(httpContext, \"{parameter.Name}\");");
                                    else
                                        builder.AppendLine($"GetQueryValue<{GetTypeFullText(parameterType)}>(httpContext, \"{parameter.Name}\");");
                                }
                            }
                            else
                            {
                                builder.AppendLine($"                    var {parameter.Name}Value = await GetBodyValueAsync<{GetTypeFullText(parameterType)}>(httpContext);");
                                builder.AppendLine($"                    if (!{parameter.Name}Value.Item1)");
                                builder.AppendLine("                        break;");
                                builder.AppendLine($"                    var {parameter.Name} = {parameter.Name}Value.Item2;");
                            }
                        }
                        var returnType = (INamedTypeSymbol)member.ReturnType;
                        builder.AppendLine("                    bool success = false;");
                        if (returnType.IsGenericType)
                        {
                            builder.AppendLine($"                    {GetTypeFullText((INamedTypeSymbol)returnType.TypeArguments[0])} result = default;");
                            builder.AppendLine("                    try");
                            builder.AppendLine("                    {");
                            builder.AppendLine($"                        result = await domainTemplate.{member.Name}({string.Join(", ", member.Parameters.Select(t => t.Name))});");
                            builder.AppendLine("                        success = true;");
                            builder.AppendLine("                    }");
                            builder.AppendLine("                    catch (Exception ex)");
                            builder.AppendLine("                    {");
                            builder.AppendLine($"                        await OnDomainException(httpContext, \"{member.Name}\", ex);");
                            builder.AppendLine("                    }");
                            builder.AppendLine("                    if (success)");
                            builder.AppendLine($"                        await OnDomainExecuted(httpContext, \"{member.Name}\", result);");
                        }
                        else
                        {
                            builder.AppendLine("                    try");
                            builder.AppendLine("                    {");
                            builder.AppendLine($"                        await domainTemplate.{member.Name}({string.Join(", ", member.Parameters.Select(t => t.Name))});");
                            builder.AppendLine("                    }");
                            builder.AppendLine("                    catch (Exception ex)");
                            builder.AppendLine("                    {");
                            builder.AppendLine($"                        await OnDomainException(httpContext, \"{member.Name}\", ex);");
                            builder.AppendLine("                    }");
                            builder.AppendLine("                    if (success)");
                            builder.AppendLine($"                        await OnDomainExecuted(httpContext, \"{member.Name}\");");
                        }
                        builder.AppendLine("                    break;");
                        builder.AppendLine("                }");
                    }
                    if (fail)
                        continue;
                    builder.AppendLine("                default:");
                    builder.AppendLine("                    return;");
                    builder.AppendLine("            }");
                    builder.AppendLine("        }");
                    builder.AppendLine("    }");
                    builder.Append("}");
                    var filename = $"{ns}.{name}.g.cs";
                    if (files.Contains(filename))
                        continue;
                    files.Add(filename);
                    context.AddSource(filename, builder.ToString());
                }
            }
        }

        private bool IsDomainEndpointType(INamedTypeSymbol type, out ITypeSymbol templateType)
        {
            if (!type.IsGenericType)
            {
                templateType = null;
                return false;
            }
            templateType = type.TypeArguments[0];
            while (true)
            {
                if (type == null)
                    return false;
                if (type.ContainingNamespace.ToString() == "Wodsoft.ComBoost.AspNetCore" && type.Name == "DomainEndpoint")
                    return true;
                type = type.BaseType;
            }
        }

        private string GetTypeFullText(INamedTypeSymbol type)
        {
            string name = $"global::{type.ContainingNamespace}.{type.Name}";
            if (type.IsGenericType)
            {
                name += $"<{string.Join(", ", type.TypeArguments.Select(t => GetTypeFullText((INamedTypeSymbol)t)))}>";
            }
            return name;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new DomainEndpointSyntaxReceiver());
        }
    }
}
