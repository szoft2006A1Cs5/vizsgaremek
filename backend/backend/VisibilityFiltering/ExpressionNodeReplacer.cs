using backend.Models;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace backend.VisibilityFiltering
{
    public class ExpressionNodeReplacer : ExpressionVisitor
    {
        private readonly Expression _oldNode, _newNode;

        public ExpressionNodeReplacer(Expression oldNode, Expression newNode)
        {
            _oldNode = oldNode;
            _newNode = newNode;
        }

        public override Expression? Visit(Expression? node)
        {
            return node == _oldNode ? _newNode : base.Visit(node);
        }

        public static Expression<Func<T, object>>? ReplaceAuthUserParam<T>(Expression<Func<T, User?, object>> expression, User? authUser)
        {
            var replacedBody = new ExpressionNodeReplacer(
                expression.Parameters[1],
                Expression.Constant(authUser, typeof(User))
            ).Visit(expression.Body);

            if (replacedBody == null) return null;

            return Expression.Lambda<Func<T, object>>(replacedBody, [expression.Parameters[0]]);
        }
    }
}
