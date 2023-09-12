using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Wodsoft.ComBoost
{
    public static class SyntaxHelper
    {
        public static bool IsSameFullName(TypeSyntax nameSyntax, string fullname, bool isAttribute)
        {
            if (nameSyntax is QualifiedNameSyntax qualifiedName)
            {
                var name = GetFullname(qualifiedName, out var isGlobal);
                if (isAttribute && !name.EndsWith("Attribute"))
                    name += "Attribute";
                if (isGlobal)
                    return name == fullname;
                if (name.Contains("."))
                {
                    var i = name.IndexOf('.');
                    var f = name.Substring(0, i);
                    foreach (UsingDirectiveSyntax item in ((Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax)nameSyntax.SyntaxTree.GetRoot()).Usings)
                    {
                        if (!item.StaticKeyword.IsKind(SyntaxKind.None))
                            continue;
                        if (item.Alias != null)
                        {
                            if (item.Alias.Name.ToString() == f)
                            {
                                return fullname == item.Name.ToString() + name.Substring(i);
                            }
                        }
                        else
                        {
                            if (item.Name.ToString() + "." + name == fullname)
                                return true;
                        }
                    }
                    var nsList = new List<string>();
                    var parentNS = nameSyntax.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
                    while (parentNS != null)
                    {
                        var nsSplit = parentNS.Name.ToString().Split('.');
                        nsList.InsertRange(0, nsSplit);
                        parentNS = parentNS.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>(t => t != parentNS);
                    }
                    if (nsList.Count > 0)
                    {
                        string ns = string.Empty;
                        for (i = 0; i < nsList.Count; i++)
                        {
                            ns += nsList[0] + ".";
                            if (ns + name == fullname)
                                return true;
                        }
                    }
                }
            }
            else if (nameSyntax is IdentifierNameSyntax identifierName)
            {
                var nsIndex = fullname.LastIndexOf('.');
                var ns = nsIndex > 0 ? fullname.Substring(0, nsIndex) : fullname;
                var name = identifierName.Identifier.ValueText;
                foreach (UsingDirectiveSyntax item in ((Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax)nameSyntax.SyntaxTree.GetRoot()).Usings)
                {
                    if (!item.StaticKeyword.IsKind(SyntaxKind.None))
                        continue;
                    if (item.Alias != null)
                    {
                        if (item.Name.ToString() == fullname && name == item.Alias.Name.ToString())
                            return true;
                    }
                    else
                    {
                        if (item.Name.ToString() == ns)
                        {
                            if (isAttribute && !name.EndsWith("Attribute"))
                                name += "Attribute";
                            if (name == fullname.Substring(nsIndex + 1))
                                return true;
                        }
                    }
                }
                var nsList = new List<string>();
                var parentNS = nameSyntax.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
                while (parentNS != null)
                {
                    var nsSplit = parentNS.Name.ToString().Split('.');
                    nsList.InsertRange(0, nsSplit);
                    parentNS = parentNS.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>(t => t != parentNS);
                }
                var targetNSList = ns.Split('.');
                if (nsList.Count >= targetNSList.Length)
                {
                    bool eq = true;
                    for (int i = 0; i < targetNSList.Length; i++)
                    {
                        if (targetNSList[i] != nsList[i])
                        {
                            eq = false;
                            break;
                        }
                    }
                    if (eq)
                    {
                        if (isAttribute && !name.EndsWith("Attribute"))
                            name += "Attribute";
                        if (nsIndex > 0)
                            return name == fullname.Substring(nsIndex + 1);
                        else
                            return name == fullname;
                    }
                }
            }
            else if (nameSyntax is PredefinedTypeSyntax predefinedSyntax)
            {

            }
            return false;
        }

        private static string GetFullname(QualifiedNameSyntax qualifiedName, out bool isGlobal)
        {
            string left, right;
            if (qualifiedName.Left is AliasQualifiedNameSyntax aliasSyntax)
            {
                if (aliasSyntax.Alias.Identifier.Text == "global")
                {
                    left = aliasSyntax.Name.ToString();
                    isGlobal = true;
                }
                else
                {
                    foreach (UsingDirectiveSyntax item in ((Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax)qualifiedName.SyntaxTree.GetRoot()).Usings)
                    {
                        if (!item.StaticKeyword.IsKind(SyntaxKind.None))
                            continue;
                        if (item.Alias != null)
                        {
                            if (item.Alias.Name.ToString() == aliasSyntax.Alias.Identifier.Text)
                                left = item.Name.ToString() + "." + aliasSyntax.Name.ToString();
                        }
                    }
                    throw new InvalidProgramException($"Can not find alias name '{aliasSyntax.Name}'.");
                }
            }
            else
            {
                left = GetName(qualifiedName.Left, out isGlobal);
            }
            right = GetName(qualifiedName.Right, out _);
            if (isGlobal)
                return right;
            return left + "." + right;
        }

        private static string GetName(NameSyntax nameSyntax, out bool isGlobal)
        {
            isGlobal = false;
            if (nameSyntax is QualifiedNameSyntax qualified)
                return GetFullname(qualified, out isGlobal);
            else if (nameSyntax is IdentifierNameSyntax identifierName)
                return identifierName.Identifier.Text;
            else if (nameSyntax is GenericNameSyntax genericSyntax)
                return genericSyntax.Identifier.Text;
            else
                throw new NotSupportedException();
        }

        public static bool IsSameFullName(TypeSyntax nameSyntax, string fullName, SemanticModel model)
        {
            var typeInfo = model.GetTypeInfo(nameSyntax);
            if (typeInfo.Type == null)
                return false;
            return typeInfo.Type.ContainingNamespace + "." + typeInfo.Type.Name == fullName;
        }
    }
}
