using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Wodsoft.ComBoost
{
    [Generator]
    public class DomainActionSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            List<string> files = new List<string>();
            if (context.SyntaxReceiver is DomainActionSyntaxReceiver receiver)
            {
                foreach (var classSyntax in receiver.ClassDeclarations)
                {
                    var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
                    var classType = model.GetDeclaredSymbol(classSyntax);
                    string ns = classType.ContainingNamespace.ToString();
                    string name = classSyntax.Identifier.ValueText;
                    var templateTypes = new List<INamedTypeSymbol>();
                    var methodNames = new List<string>();
                    var builder = new StringBuilder();
                    builder.AppendLine("// ComBoost auto generated domain action codes.");
                    builder.AppendLine("using Microsoft.AspNetCore.Mvc;");
                    builder.AppendLine("using System;");
                    builder.AppendLine("using System.Threading.Tasks;");
                    builder.AppendLine("namespace " + ns);
                    builder.AppendLine("{");
                    builder.AppendLine($"    public partial class {name}");
                    builder.AppendLine("    {");
                    bool fail = false;
                    foreach (var interfaceSymbol in classType.Interfaces)
                    {
                        if (interfaceSymbol.ContainingNamespace.ToString() == "Wodsoft.ComBoost.Mvc" && interfaceSymbol.Name == "IDomainAction")
                        {
                            var templateType = (INamedTypeSymbol)interfaceSymbol.TypeArguments[0];
                            if (templateTypes.Contains(templateType))
                                continue;
                            if (templateType.GetMembers().GroupBy(t => t.Name).Any(t => t.Count() != 1))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("CBANC002", "Domain template contains overload methods.", "A domain template binded to endpoint cannot contains overload methods. ASP.NET Core could not recognize which method should bind.", "Wodsoft.ComBoost.AspNetCore", DiagnosticSeverity.Error, true)
                                    , classSyntax.GetLocation()));
                                continue;
                            }
                            foreach (var member in templateType.GetMembers().OfType<IMethodSymbol>())
                            {
                                if (methodNames.Contains(member.Name.ToLower()))
                                {
                                    context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("CBANC004", "More than one domain template contains same method name.", "Controller have more than one domain template contains same method name. ASP.NET Core could not recognize which method should bind.", "Wodsoft.ComBoost.AspNetCore", DiagnosticSeverity.Error, true)
                                        , classSyntax.GetLocation()));
                                    continue;
                                }
                                bool isGetMethod = false;
                                var attr = member.GetAttributes().FirstOrDefault(t => t.AttributeClass.BaseType.ContainingNamespace.ToString() == "Microsoft.AspNetCore.Mvc" && t.AttributeClass.BaseType.Name == "HttpMethodAttribute");
                                if (attr == null)
                                {
                                    if (member.Name.StartsWith("Get"))
                                    {
                                        isGetMethod = true;
                                        builder.AppendLine("        [HttpGet]");
                                    }
                                    else if (member.Name.StartsWith("Post") || member.Name.StartsWith("Insert") || member.Name.StartsWith("Create"))
                                    {
                                        builder.AppendLine("        [HttpPost]");
                                    }
                                    else if (member.Name.StartsWith("Update") || member.Name.StartsWith("Edit"))
                                        builder.AppendLine("        [HttpPut]");
                                    else if (member.Name.StartsWith("Delete") || member.Name.StartsWith("Remove"))
                                        builder.AppendLine("        [HttpDelete]");
                                    else
                                        builder.AppendLine("        [HttpPost]");
                                }
                                else
                                {
                                    if (attr.AttributeClass.Name == "HttpGetAttribute")
                                        isGetMethod = true;
                                    builder.AppendLine($"        [{GetTypeFullText(attr.AttributeClass)}]");
                                }
                                if (!isGetMethod && member.Parameters.Length != 1)
                                {
                                    context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("CBANC003", "Domain template contains not support methods.", "Domain template contains methods which is non HTTP GET and have more than one parameters.", "Wodsoft.ComBoost.AspNetCore", DiagnosticSeverity.Error, true)
                                        , classSyntax.GetLocation()));
                                    fail = true;
                                    break;
                                }
                                builder.Append($"        public {GetTypeFullText((INamedTypeSymbol)member.ReturnType)} {member.Name}([FromServices] {GetTypeFullText(templateType)} service");
                                foreach (var parameter in member.Parameters)
                                {
                                    var parameterType = (INamedTypeSymbol)parameter.Type;
                                    if (isGetMethod)
                                    {
                                        builder.Append(", [FromQuery] ");
                                    }
                                    else
                                    {
                                        builder.Append(", [FromBody] ");
                                    }
                                    builder.Append($"{GetTypeFullText(parameterType)} {parameter.Name}");
                                }
                                builder.AppendLine(")");
                                builder.AppendLine("        {");
                                builder.AppendLine($"            return service.{member.Name}({string.Join(",", member.Parameters.Select(t => t.Name))});");
                                builder.AppendLine("        }");
                            }
                            if (fail)
                                break;
                        }
                    }
                    if (fail)
                        continue;
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
            context.RegisterForSyntaxNotifications(() => new DomainActionSyntaxReceiver());
        }
    }
}
