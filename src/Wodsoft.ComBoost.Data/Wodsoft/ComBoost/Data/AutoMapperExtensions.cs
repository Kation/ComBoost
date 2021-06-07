using AutoMapper;
using AutoMapper.Internal;
using AutoMapper.QueryableExtensions;
using AutoMapper.QueryableExtensions.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MemberPaths = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<System.Reflection.MemberInfo>>;
using ParameterBag = System.Collections.Generic.IDictionary<string, object>;

namespace Wodsoft.ComBoost.Data
{
    public static class AutoMapperExtensions
    {
        ///// <summary>
        ///// Maps a queryable expression of a source type to a queryable expression of a destination type
        ///// </summary>
        ///// <typeparam name="TSource">Source type</typeparam>
        ///// <typeparam name="TDestination">Destination type</typeparam>
        ///// <param name="sourceQuery">Source queryable</param>
        ///// <param name="destQuery">Destination queryable</param>
        ///// <param name="config"></param>
        ///// <returns>Mapped destination queryable</returns>
        //public static IAsyncQueryable<TDestination> Map<TSource, TDestination>(this IAsyncQueryable<TSource> sourceQuery, IQueryable<TDestination> destQuery, IConfigurationProvider config)
        //    => QueryMapperVisitor.Map<TSource,TDestination>(sourceQuery, destQuery, config);

        /// <summary>
        /// Extension method to project from a queryable using the provided mapping engine
        /// </summary>
        /// <remarks>Projections are only calculated once and cached</remarks>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="configuration">Mapper configuration</param>
        /// <param name="parameters">Optional parameter object for parameterized mapping expressions</param>
        /// <param name="membersToExpand">Explicit members to expand</param>
        /// <returns>Expression to project into</returns>
        public static IAsyncQueryable<TDestination> ProjectTo<TDestination>(this IAsyncQueryable source, IConfigurationProvider configuration, object parameters, params Expression<Func<TDestination, object>>[] membersToExpand)
            => new ProjectionExpressionAsync(source, configuration.ExpressionBuilder).To(parameters, membersToExpand);

        /// <summary>
        /// Extension method to project from a queryable using the provided mapping engine
        /// </summary>
        /// <remarks>Projections are only calculated once and cached</remarks>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="configuration">Mapper configuration</param>
        /// <param name="membersToExpand">Explicit members to expand</param>
        /// <returns>Expression to project into</returns>
        public static IAsyncQueryable<TDestination> ProjectTo<TDestination>(
            this IAsyncQueryable source,
            IConfigurationProvider configuration,
            params Expression<Func<TDestination, object>>[] membersToExpand
            )
            => source.ProjectTo(configuration, null, membersToExpand);

        /// <summary>
        /// Projects the source type to the destination type given the mapping configuration
        /// </summary>
        /// <typeparam name="TDestination">Destination type to map to</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="configuration">Mapper configuration</param>
        /// <param name="parameters">Optional parameter object for parameterized mapping expressions</param>
        /// <param name="membersToExpand">Explicit members to expand</param>
        /// <returns>Queryable result, use queryable extension methods to project and execute result</returns>
        public static IAsyncQueryable<TDestination> ProjectTo<TDestination>(this IAsyncQueryable source, IConfigurationProvider configuration, IDictionary<string, object> parameters, params string[] membersToExpand)
            => new ProjectionExpressionAsync(source, configuration.ExpressionBuilder).To<TDestination>(parameters, membersToExpand);
    }

    public class ProjectionExpressionAsync
    {

        private static readonly MethodInfo QueryableSelectMethod = FindQueryableSelectMethod();

        private readonly IAsyncQueryable _source;
        private readonly IExpressionBuilder _builder;

        public ProjectionExpressionAsync(IAsyncQueryable source, IExpressionBuilder builder)
        {
            _source = source;
            _builder = builder;
        }

        public IAsyncQueryable<TResult> To<TResult>(ParameterBag parameters, string[] membersToExpand) =>
            ToCore<TResult>(parameters, membersToExpand.Select(memberName => ReflectionHelper.GetMapMemberPath(typeof(TResult), memberName)));

        public IAsyncQueryable<TResult> To<TResult>(object parameters, Expression<Func<TResult, object>>[] membersToExpand) =>
            ToCore<TResult>(parameters, membersToExpand.Select(MapMemberVisitor.GetMemberPath));

        private IAsyncQueryable<TResult> ToCore<TResult>(object parameters, MemberPaths memberPathsToExpand)
        {
            var members = memberPathsToExpand.SelectMany(m => m).Distinct().ToArray();
            return (IAsyncQueryable<TResult>)_builder.GetMapExpression(_source.ElementType, typeof(TResult), parameters, members).Aggregate(_source, Select<TResult>);
        }

        private static IAsyncQueryable<TResult> Select<TResult>(IAsyncQueryable source, LambdaExpression lambda) => source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    null,
                    QueryableSelectMethod.MakeGenericMethod(source.ElementType, lambda.ReturnType),
                    new[] { source.Expression, Expression.Quote(lambda) }
                    )
                );

        private static MethodInfo FindQueryableSelectMethod()
        {
            Expression<Func<IAsyncQueryable<object>>> select = () => default(IAsyncQueryable<object>).Select(default(Expression<Func<object, object>>));
            var method = ((MethodCallExpression)select.Body).Method.GetGenericMethodDefinition();
            return method;
        }
    }
}
