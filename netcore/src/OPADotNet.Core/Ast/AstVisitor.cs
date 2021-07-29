using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast
{
    public class AstVisitor<T>
    {
        private static readonly List<T> emptyList = new List<T>();

        public virtual T Visit(AstNode node)
        {
            if (node == null)
                return default;

            return node.Accept(this);
        }

        public virtual IList<T> Visit(IEnumerable<AstNode> nodes)
        {
            if (nodes == null)
                return emptyList;

            List<T> output = new List<T>();
            foreach (var node in nodes)
            {
                output.Add(Visit(node));
            }
            return output;
        }

        public virtual T VisitQueries(AstQueries partialQueries)
        {
            Visit(partialQueries.Queries);
            return default;
        }

        public virtual T VisitBody(AstBody partialBody)
        {
            Visit(partialBody.Expressions);
            return default;
        }

        public virtual T VisitExpression(AstExpression partialExpression)
        {
            Visit(partialExpression.Terms);
            return default(T);
        }

        public virtual T VisitTermNumber(AstTermNumber partialTermNumber)
        {
            return default(T);
        }

        public virtual T VisitTermRef(AstTermRef partialTermRef)
        {
            Visit(partialTermRef.Value);
            return default(T);
        }

        public virtual T VisitTermString(AstTermString partialTermString)
        {
            return default(T);
        }

        public virtual T VisitTermVar(AstTermVar partialTermVar)
        {
            return default(T);
        }

        public virtual T VisitTermObject(AstTermObject astTermObject)
        {
            Visit(astTermObject.Value);
            return default(T);
        }

        public virtual T VisitObjectProperty(AstObjectProperty astObjectProperty)
        {
            Visit(astObjectProperty.Values);
            return default(T);
        }

        public virtual T VisitPolicy(AstPolicy astPolicy)
        {
            Visit(astPolicy.Package);
            Visit(astPolicy.Rules);
            return default;
        }

        public virtual T VisitPolicyPackage(AstPolicyPackage astPolicyPackage)
        {
            Visit(astPolicyPackage.Path);
            return default;
        }

        public virtual T VisitPolicyRule(AstPolicyRule policyRule)
        {
            Visit(policyRule.Body);
            Visit(policyRule.Head);
            return default;
        }

        public virtual T VisitRuleHead(AstRuleHead ruleHead)
        {
            Visit(ruleHead.Value);
            return default;
        }

        public virtual T VisitTermBoolean(AstTermBoolean termBoolean)
        {
            return default;
        }
    }
}
