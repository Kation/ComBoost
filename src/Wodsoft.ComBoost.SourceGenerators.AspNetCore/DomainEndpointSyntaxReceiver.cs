using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Wodsoft.ComBoost
{
    public class DomainEndpointSyntaxReceiver : ISyntaxReceiver
    {
        public List<AttributeSyntax> Attributes { get; } = new List<AttributeSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {            
            if (syntaxNode.Language == "C#" && syntaxNode is AttributeSyntax attributeSyntax)
            {
                if (SyntaxHelper.IsSameFullName(attributeSyntax.Name, "Wodsoft.ComBoost.AspNetCore.DomainEndpointAttribute", true))
                {
                    Attributes.Add(attributeSyntax);
                }
                else
                {
                    var name = attributeSyntax.Name.ToString();
                    var className = name.Split('.').Last();
                    if (className == "DomainEndpointAttribute" || className == "DomainEndpoint")
                        Attributes.Add(attributeSyntax);
                }
            }
        }
    }
}
