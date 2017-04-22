using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Query.Internal
{
    public class SpannerQueryMethodProvider: QueryMethodProvider
    {
        private static readonly MethodInfo _baseShapedQuery =
            typeof(QueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_ShapedQuery");

        private static readonly MethodInfo _baseDefaultIfEmptyShapedQuery =
            typeof(QueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_DefaultIfEmptyShapedQuery");

        private static readonly MethodInfo _baseQuery =
            typeof(QueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_Query");

        private static readonly MethodInfo _baseInclude =
            typeof(QueryMethodProvider).GetTypeInfo().GetDeclaredMethod("_Include");


        public override MethodInfo ShapedQueryMethod => _shapedQueryMethodInfo;

        private static readonly MethodInfo _shapedQueryMethodInfo
            = typeof(SpannerQueryMethodProvider).GetTypeInfo()
                .GetDeclaredMethod(nameof(_ShapedQuery));

        private static IEnumerable<T> _ShapedQuery<T>(
            QueryContext queryContext,
            ShaperCommandContext shaperCommandContext,
            IShaper<T> shaper)
        {
            return new SpannerQueryingEnumerable<T>(queryContext as SpannerQueryContext,
                (IEnumerable<T>)_baseShapedQuery.MakeGenericMethod(typeof(T))
                    .Invoke(null, new object[] { queryContext, shaperCommandContext, shaper }));
        }


        public override MethodInfo DefaultIfEmptyShapedQueryMethod => _defaultIfEmptyShapedQueryMethodInfo;

        private static readonly MethodInfo _defaultIfEmptyShapedQueryMethodInfo
            = typeof(SpannerQueryMethodProvider).GetTypeInfo()
                .GetDeclaredMethod(nameof(_DefaultIfEmptyShapedQuery));

        [UsedImplicitly]
        private static IEnumerable<T> _DefaultIfEmptyShapedQuery<T>(
            QueryContext queryContext,
            ShaperCommandContext shaperCommandContext,
            IShaper<T> shaper)
        {
            return new SpannerQueryingEnumerable<T>(queryContext as SpannerQueryContext,
                (IEnumerable<T>)_baseDefaultIfEmptyShapedQuery.MakeGenericMethod(typeof(T))
                    .Invoke(null, new object[] { queryContext, shaperCommandContext, shaper }));
        }


        public override MethodInfo QueryMethod => _queryMethodInfo;

        private static readonly MethodInfo _queryMethodInfo
            = typeof(SpannerQueryMethodProvider).GetTypeInfo()
                .GetDeclaredMethod(nameof(_Query));

        private static IEnumerable<ValueBuffer> _Query(
            QueryContext queryContext,
            ShaperCommandContext shaperCommandContext,
            int? queryIndex)
        {
            return new SpannerQueryingEnumerable<ValueBuffer>(queryContext as SpannerQueryContext,
                (IEnumerable<ValueBuffer>)
                _baseQuery.Invoke(null, new object[] { queryContext, shaperCommandContext, queryIndex }));
        }


        public override MethodInfo IncludeMethod => _includeMethodInfo;

        private static readonly MethodInfo _includeMethodInfo
            = typeof(SpannerQueryMethodProvider).GetTypeInfo()
                .GetDeclaredMethod(nameof(_Include));

        private static IEnumerable<T> _Include<T>(
            RelationalQueryContext queryContext,
            IEnumerable<T> innerResults,
            Func<T, object> entityAccessor,
            IReadOnlyList<INavigation> navigationPath,
            IReadOnlyList<Func<QueryContext, IRelatedEntitiesLoader>> relatedEntitiesLoaderFactories,
            bool querySourceRequiresTracking)
        {
            // ReSharper disable once PossibleNullReferenceException
            (queryContext as SpannerQueryContext).HasInclude = true;
            return (IEnumerable<T>)_baseInclude.MakeGenericMethod(typeof(T))
                .Invoke(null, new object[] { queryContext, innerResults, entityAccessor, navigationPath, relatedEntitiesLoaderFactories, querySourceRequiresTracking });
        }
    }
}
