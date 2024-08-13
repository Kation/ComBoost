using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class DomainActionSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> ClassDeclarations { get; } = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode.Language == "C#" && syntaxNode is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.Modifiers.Any(t => t.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
            {
                if (classDeclarationSyntax.BaseList != null && classDeclarationSyntax.BaseList.Types.Any(t => SyntaxHelper.IsSameFullName(t.Type, "Wodsoft.ComBoost.Mvc.IDomainAction", false)))
                {
                    ClassDeclarations.Add(classDeclarationSyntax);
                }
            }
        }
    }
}
