using System.Linq.Expressions;

namespace GreenDonut.Data.Internal;

internal static class DataLoaderStateHelper
{
    internal static IDataLoader CreateBranch<TKey, TValue>(
        string branchKey,
        IDataLoader<TKey, TValue> dataLoader,
        QueryState state)
        where TKey : notnull
    {
        var branch = new QueryDataLoader<TKey, TValue>(
            (DataLoaderBase<TKey, TValue>)dataLoader,
            branchKey);
        branch.SetState(state.Key, state.Value);
        return branch;
    }

    internal static IDataLoader CreateBranch<TKey, TValue, TElement>(
        string branchKey,
        IDataLoader<TKey, TValue> dataLoader,
        QueryContext<TElement> state)
        where TKey : notnull
    {
        var branch = new QueryDataLoader<TKey, TValue>(
            (DataLoaderBase<TKey, TValue>)dataLoader,
            branchKey);

        if (state.Selector is not null)
        {
            branch.SetState(DataLoaderStateKeys.Selector, new DefaultSelectorBuilder(state.Selector));
        }

        if (state.Predicate is not null)
        {
            branch.SetState(DataLoaderStateKeys.Predicate, new DefaultPredicateBuilder(state.Predicate));
        }

        if (state.Sorting is not null)
        {
            branch.SetState(DataLoaderStateKeys.Sorting, state.Sorting);
        }

        return branch;
    }

    internal static IDataLoader CreateBranch<TKey, TValue, TElement>(
        string branchKey,
        IDataLoader<TKey, Page<TElement>> dataLoader,
        PagingState<TValue> state)
        where TKey : notnull
    {
        var branch = new QueryDataLoader<TKey, Page<TValue>>(
            (DataLoaderBase<TKey, Page<TValue>>)dataLoader,
            branchKey);

        branch.SetState(DataLoaderStateKeys.PagingArgs, state.PagingArgs);

        if (state.Context is not null)
        {
            if (state.Context.Selector is not null)
            {
                branch.SetState(DataLoaderStateKeys.Selector, new DefaultSelectorBuilder(state.Context.Selector));
            }

            if (state.Context.Predicate is not null)
            {
                branch.SetState(DataLoaderStateKeys.Predicate, new DefaultPredicateBuilder(state.Context.Predicate));
            }

            if (state.Context.Sorting is not null)
            {
                branch.SetState(DataLoaderStateKeys.Sorting, state.Context.Sorting);
            }
        }

        return branch;
    }

    internal static string ComputeHash<TValue>(this QueryContext<TValue> state)
    {
        var hasher = ExpressionHasherPool.Shared.Get();

        if (state.Selector is not null)
        {
            hasher.Add(state.Selector);
        }

        if (state.Predicate is not null)
        {
            hasher.Add(state.Predicate);
        }

        if (state.Sorting is not null)
        {
            hasher.Add(state.Sorting);
        }

        var hash = hasher.Compute();
        ExpressionHasherPool.Shared.Return(hasher);
        return hash;
    }

    internal static ExpressionHasher Add<TValue>(this ExpressionHasher hasher, QueryContext<TValue> state)
    {
        if (state.Selector is not null)
        {
            hasher.Add(state.Selector);
        }

        if (state.Predicate is not null)
        {
            hasher.Add(state.Predicate);
        }

        if (state.Sorting is not null)
        {
            hasher.Add(state.Sorting);
        }

        return hasher;
    }

    public static string ComputeHash(this Expression expression)
    {
        var hasher = ExpressionHasherPool.Shared.Get();
        var branchKey = hasher.Add(expression).Compute();
        ExpressionHasherPool.Shared.Return(hasher);
        return branchKey;
    }

    public static string ComputeHash<T>(this SortDefinition<T> sortDefinition)
    {
        var hasher = ExpressionHasherPool.Shared.Get();
        var branchKey = hasher.Add(sortDefinition).Compute();
        ExpressionHasherPool.Shared.Return(hasher);
        return branchKey;
    }
}
