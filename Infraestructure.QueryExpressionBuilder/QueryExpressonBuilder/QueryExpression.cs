using Infraestructure.QueryExpressionBuilder.Messages;
using LinqKit;
using System.Linq.Expressions;

namespace Infraestructure.QueryExpressionBuilder.QueryExpresson
{
    public class QueryExpression<T> where T : class
    {
        private Expression<Func<T, bool>>? query;

        /// <summary>
        /// Expressão final construída. Lança exceção se ainda não foi inicializada.
        /// </summary>
        public Expression<Func<T, bool>> Query =>
            query ?? throw new InvalidOperationException(QueryBuilderMessages.QueryBuilderNotInitialized);

        public QueryExpression()
        {
            // Inicializa com uma expressão verdadeira para permitir composição imediata
            query = PredicateBuilder.New<T>(true);
        }

        /// <summary>
        /// Adiciona um predicado à expressão existente usando operador AND.
        /// </summary>
        public QueryExpression<T> Where(Expression<Func<T, bool>> expression)
        {
            if (expression is null)
                throw new ArgumentNullException(nameof(expression), QueryBuilderMessages.ExpressionIsNull);

            query = query!.And(expression);
            return this;
        }

        /// <summary>
        /// Adiciona outra QueryExpression à expressão atual usando operador AND.
        /// </summary>
        public QueryExpression<T> And(QueryExpression<T> expression)
        {
            if (expression is null)
                throw new ArgumentNullException(nameof(expression), QueryBuilderMessages.ExpressionIsNull);

            query = query!.And(expression.Build());
            return this;
        }

        /// <summary>
        /// Adiciona diretamente uma nova expressão com operador AND.
        /// </summary>
        public QueryExpression<T> And(Expression<Func<T, bool>> expression)
        {
            if (expression is null)
                throw new ArgumentNullException(nameof(expression), QueryBuilderMessages.ExpressionIsNull);

            query = query!.And(expression);
            return this;
        }

        /// <summary>
        /// Retorna a expressão construída para uso externo (ex: no repositório).
        /// </summary>
        public Expression<Func<T, bool>> Build() => Query;
    }
}
