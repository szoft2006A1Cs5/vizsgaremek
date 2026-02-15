using backend.Models;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection.Metadata;

namespace backend.VisibilityFiltering
{
    public class ExpressionParameterReplacer : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, Expression> _replacements;

        public ExpressionParameterReplacer(Dictionary<ParameterExpression, Expression> replacements)
        {
            _replacements = replacements;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _replacements.TryGetValue(node, out var retNode) ? retNode : base.VisitParameter(node);
        }

        public static Expression<Func<T, object>>? ReplaceAuthUserParam<T>(
            Expression<Func<T, User?, object>> expression,
            User? authUser
        )
        {
            var replacedBody = new ExpressionParameterReplacer(new Dictionary<ParameterExpression, Expression> {
                { expression.Parameters[1], Expression.Constant(authUser, typeof(User)) }
            }).Visit(expression.Body);

            return Expression.Lambda<Func<T, object>>(replacedBody, [expression.Parameters[0]]);
        }
    }
}
