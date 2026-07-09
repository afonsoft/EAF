#nullable disable

using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public AsyncQueryable(IAsyncQueryProvider provider, Expression expression)
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
    internal class AsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IQueryable<T> _source;

        public AsyncQueryProvider(IQueryable<T> source)
        {
            _source = source;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var elementType = expression.Type.GetGenericArguments().FirstOrDefault();
            if (elementType == null)
                throw new NotSupportedException();

            var queryType = typeof(AsyncQueryable<>).MakeGenericType(elementType);
            return (IQueryable)Activator.CreateInstance(queryType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { this, expression }, null);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AsyncQueryable<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return _source.Provider.Execute(Rewrite(expression));
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _source.Provider.Execute<TResult>(Rewrite(expression));
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var result = Execute(Rewrite(expression));

            if (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>))
            {
                var valueType = typeof(TResult).GetGenericArguments()[0];

                if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var list = (IList)Activator.CreateInstance(valueType);
                    foreach (var item in (IEnumerable)result)
                    {
                        list.Add(item);
                    }

                    return (TResult)TaskFromResult(valueType, list);
                }

                var converted = Convert.ChangeType(result, valueType);
                return (TResult)TaskFromResult(valueType, converted);
            }

            return (TResult)result;
        }

        private static object TaskFromResult(Type valueType, object value)
        {
            return typeof(Task)
                .GetMethod("FromResult")!
                .MakeGenericMethod(valueType)
                .Invoke(null, new[] { value });
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
