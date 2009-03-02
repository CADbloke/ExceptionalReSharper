using System;
using System.Xml;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;

namespace CodeGears.ReSharper.Exceptional
{
    public class XmlDocCommentHelper
    {
        public static DocumentRange FindRangeOfExceptionTag(XmlNode node, string exceptionType, IDocCommentBlockNode docNode)
        {
            if (docNode == null) return DocumentRange.InvalidRange;

            for (var currentNode = docNode.FirstChild; currentNode != null; currentNode = currentNode.NextSibling)
            {
                var text = currentNode.GetText();
                if (text.Contains("<exception") == false) continue;

                var index = exceptionType.LastIndexOf('.');
                var exceptiontypeName = exceptionType.Substring(index + 1);

                if (text.Contains(exceptiontypeName) && text.Contains(node.InnerText))
                {
                    return currentNode.GetDocumentRange();
                }
            }

            return DocumentRange.InvalidRange;
        }

        public static void AddExceptionDocumentation(ICSharpTypeMemberDeclarationNode memberDeclaration, string exceptionName, IProject project)
        {
            var comment = SharedImplUtil.GetDocCommentBlockNode(memberDeclaration);
            var text = comment != null ? comment.GetText() + Environment.NewLine : String.Empty;

            text += String.Format("/// <exception cref=\"{0}\"></exception>", exceptionName) + Environment.NewLine +
                    " public void foo() {}";

            var commentOwner = CSharpElementFactory.GetInstance(project).CreateTypeMemberDeclaration(text) as IDocCommentBlockOwnerNode;

            SharedImplUtil.SetDocCommentBlockNode(memberDeclaration, commentOwner.GetDocCommentBlockNode());
        }
    }
}