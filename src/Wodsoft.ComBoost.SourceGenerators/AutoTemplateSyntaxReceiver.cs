using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wodsoft.ComBoost
{
    public class AutoTemplateSyntaxReceiver : ISyntaxReceiver
    {
        public List<AttributeSyntax> Attributes { get; } = new List<AttributeSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode.Language == "C#" && syntaxNode is AttributeSyntax attributeSyntax)
            {
                if (SyntaxHelper.IsSameFullName(attributeSyntax.Name, "Wodsoft.ComBoost.AutoTemplateAttribute", true))
                {
                    Attributes.Add(attributeSyntax);
                }
            }
        }
    }
}
