using System.Linq.Expressions;

namespace PlaylistManager.Core.Common.Utils;

public static class ExpressionConverter
{
	public static Expression<Func<TEntity, bool>> TranslateQuery<TDomainModel, TEntity>(Expression<Func<TDomainModel, bool>> query)
	{
		var visitor = new GenericExpressionVisitor<TDomainModel, TEntity>(Expression.Parameter(typeof(TEntity), query.Parameters[0].Name));
		var convertedExpression = Expression.Lambda<Func<TEntity, bool>>(visitor.Visit(query.Body), visitor.NewParameterExpression);
		return convertedExpression;
	}
}

public class GenericExpressionVisitor<TIn, TOut> : ExpressionVisitor
{
	public ParameterExpression NewParameterExpression { get; private set; }

	public GenericExpressionVisitor(ParameterExpression newParameterExpression)
	{
		NewParameterExpression = newParameterExpression;
	}

	protected override Expression VisitParameter(ParameterExpression node)
	{
		return NewParameterExpression;
	}

	protected override Expression VisitMember(MemberExpression node)
	{
		if (node.Member.DeclaringType == typeof(TIn))
		{
			var member = typeof(TOut).GetMember(node.Member.Name).FirstOrDefault();
			return Expression.MakeMemberAccess(this.Visit(node.Expression), member);
		}
			
		if (node.Member.DeclaringType == typeof(TIn).BaseType)
		{
			var member = typeof(TOut).GetMember(node.Member.Name).FirstOrDefault();
			return Expression.MakeMemberAccess(this.Visit(node.Expression), member);
		}

			
		return base.VisitMember(node);
	}
}