using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Middleware.Application.Tests.Helpers
{
    /// <summary>
    /// Utilitário para converter coleções em memória em <see cref="IQueryable{T}"/> assíncronos
    /// compatíveis com <c>Microsoft.EntityFrameworkCore.ToListAsync</c> e operadores LINQ assíncronos.
    /// </summary>
    public static class AsyncQueryable
    {
        /// <summary>
        /// Envolve a coleção em um <see cref="IQueryable{T}"/> que também implementa
        /// <see cref="IAsyncEnumerable{T}"/> e <see cref="IOrderedQueryable{T}"/>.
        /// </summary>
        public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new AsyncQueryable<T>(source);
        }
    }

    /// <summary>
    /// Implementação de <see cref="IOrderedQueryable{T}"/> assíncrono para testes.
    /// </summary>
    internal class AsyncQueryable<T> : IOrderedQueryable<T>, IAsyncEnumerable<T>
    {
        public AsyncQueryable(IEnumerable<T> source)
        {
            var queryableSource = source.AsQueryable();
            Provider = new AsyncQueryProvider<T>(queryableSource);
            Expression = Expression.Constant(this);
            ElementType = typeof(T);
        }

        public AsyncQueryable(IQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
            ElementType = typeof(T);
        }

        public Type ElementType { get; }

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerator<T>(Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator());
        }
    }

    /// <summary>
    /// Provedor de query que reescreve constantes <see cref="AsyncQueryable{T}"/> para a fonte
    /// real, permitindo executar expressões LINQ sobre coleções em memória.
    /// </summary>
    internal class AsyncQueryProvider<T> : IQueryProvider
    {
        private readonly IQueryable<T> _source;

        public AsyncQueryProvider(IQueryable<T> source)
        {
            _source = source;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotSupportedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) != typeof(T))
            {
                throw new NotSupportedException($"AsyncQueryable only supports queries of type {typeof(T).Name}.");
            }

            return new AsyncQueryable<TElement>(new AsyncQueryProvider<TElement>((IQueryable<TElement>)_source), expression);
        }

        public object Execute(Expression expression)
        {
            return _source.Provider.Execute(Rewrite(expression));
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _source.Provider.Execute<TResult>(Rewrite(expression));
        }

        private Expression Rewrite(Expression expression)
        {
            return new AsyncQueryableRewriter<T>(_source).Visit(expression);
        }
    }

    /// <summary>
    /// Visitor que substitui constantes <see cref="AsyncQueryable{T}"/> pela fonte em memória.
    /// </summary>
    internal class AsyncQueryableRewriter<T> : ExpressionVisitor
    {
        private readonly IQueryable<T> _source;

        public AsyncQueryableRewriter(IQueryable<T> source)
        {
            _source = source;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is IQueryable queryable && queryable.Provider is AsyncQueryProvider<T>)
            {
                return Expression.Constant(_source, typeof(IQueryable<T>));
            }

            return base.VisitConstant(node);
        }
    }

    /// <summary>
    /// Enumerador assíncrono que envolve um <see cref="IEnumerator{T}"/> síncrono.
    /// </summary>
    internal class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public AsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public T Current => _inner.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask();
        }
    }
}
