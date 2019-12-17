using System;
using System.Linq.Expressions;
using HotChocolate.Language;

namespace HotChocolate.Types.Sorting
{
    public interface ISortInputTypeDescriptor<T>
        : ISortInputTypeDescriptor
    {

        /// <summary>
        /// Defines the name of the <see cref="SortInputType{T}"/>.
        /// </summary>
        /// <param name="value">The sort type name.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c> or
        /// <see cref="string.Empty"/>.
        /// </exception>
        new ISortInputTypeDescriptor<T> Name(NameString value);

        /// <summary>
        /// Adds explanatory text of the <see cref="SortInputType{T}"/>
        /// that can be accessd via introspection.
        /// </summary>
        /// <param name="value">The sort type description.</param>
        /// 
        new ISortInputTypeDescriptor<T> Description(string value);

        /// <summary>
        /// Defines the sort binding behavior.
        ///
        /// The default binding behavior is set to
        /// <see cref="BindingBehavior.Implicit"/>.
        /// </summary>
        /// <param name="behavior">
        /// The binding behavior.
        ///
        /// Implicit:
        /// The sort type descriptor will try to infer the sortable fields
        /// from the specified <typeparamref name="T"/>.
        ///
        /// Explicit:
        /// All sortable fields have to be specified explicitly by specifying
        /// which field is sortable.
        /// </param>
        new ISortInputTypeDescriptor<T> BindFields(
            BindingBehavior behavior);

        /// <summary>
        /// Defines that all sortable fields have to be specified explicitly by specifying
        /// which field is sortable.
        /// </summary>
        new ISortInputTypeDescriptor<T> BindFieldsExplicitly();

        /// <summary>
        /// Defines that the sort type descriptor will try to infer the sortable fields
        /// from the specified <typeparamref name="T"/>.
        /// </summary>
        new ISortInputTypeDescriptor<T> BindFieldsImplicitly();

        /// <summary>
        /// Defines that the selected property is sortable.
        /// </summary>
        /// <param name="property">
        /// The property that is sortable.
        /// </param>
        ISortOperationDescriptor Sortable(
            Expression<Func<T, IComparable>> property);

        /// <summary>
        /// Ignore the specified property.
        /// </summary>
        /// <param name="property">The property that hall be ignored.</param>
        ISortInputTypeDescriptor<T> Ignore(
            Expression<Func<T, object>> property);


        new ISortInputTypeDescriptor<T> Directive<TDirective>(
            TDirective directiveInstance)
            where TDirective : class;

        new ISortInputTypeDescriptor<T> Directive<TDirective>()
            where TDirective : class, new();

        new ISortInputTypeDescriptor<T> Directive(
            NameString name,
            params ArgumentNode[] arguments);

        /// <summary>
        /// Defines that the selected property is sortable.
        /// </summary>
        /// <param name="property">
        /// The property that is sortable.
        /// </param>
        ISortObjectOperationDescriptor<TObject> SortableObject<TObject>(
            Expression<Func<T, TObject>> property) where TObject : class;
    }
}