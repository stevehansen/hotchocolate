using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Filters.Properties;
using HotChocolate.Utilities;

namespace HotChocolate.Types.Filters.Conventions
{
    public static class FilterConventionImplicitFiltersExtensions
    {

        public static IFilterConventionDescriptor UseImplicitFilters(
            this IFilterConventionDescriptor descriptor)
        {
            return descriptor
                .Type(FilterKind.String)
                    .TryCreateImplicitFilter(TryCreateStringFilter)
                    .And()
                .Type(FilterKind.Boolean)
                    .TryCreateImplicitFilter(TryCreateBooleanFilter)
                    .And()
                .Type(FilterKind.Comparable)
                    .TryCreateImplicitFilter(TryCreateComparableFilter)
                    .And()
                .Type(FilterKind.Array)
                    .TryCreateImplicitFilter(TryCreateArrayFilter)
                    .And()
                .Type(FilterKind.Object)
                    .TryCreateImplicitFilter(TryCreateObjectFilter)
                    .And();
        }


        private static bool TryCreateStringFilter(
            IDescriptorContext context,
            Type type,
            PropertyInfo property,
            out FilterFieldDefintion definition)
        {

            if (type == typeof(string))
            {
                var field = new StringFilterFieldDescriptor(context, property);
                definition = field.CreateDefinition();
                return true;
            }

            definition = null;
            return false;
        }

        private static bool TryCreateBooleanFilter(
            IDescriptorContext context,
            Type type,
            PropertyInfo property,
            out FilterFieldDefintion definition)
        {

            if (type == typeof(bool))
            {
                var field = new BooleanFilterFieldDescriptor(
                    context, property);
                definition = field.CreateDefinition();
                return true;
            }

            definition = null;
            return false;
        }

        private static bool TryCreateComparableFilter(
            IDescriptorContext context,
            Type type,
            PropertyInfo property,
            out FilterFieldDefintion definition)
        {
            if (type != typeof(bool) &&
                type != typeof(string) &&
                IsComparable(property.PropertyType))
            {
                var field = new ComparableFilterFieldDescriptor(
                    context, property);
                definition = field.CreateDefinition();
                return true;
            }

            definition = null;
            return false;
        }

        private static bool TryCreateObjectFilter(
            IDescriptorContext context,
            Type type,
            PropertyInfo property,
            out FilterFieldDefintion definition)
        {
            if (type.IsClass)
            {
                var field = new ObjectFilterFieldDescriptor(
                    context, property, property.PropertyType);
                definition = field.CreateDefinition();
                return true;
            }

            definition = null;
            return false;
        }

        private static bool TryCreateArrayFilter(
            IDescriptorContext context,
            Type type,
            PropertyInfo property,
            out FilterFieldDefintion definition)
        {
            if (DotNetTypeInfoFactory.IsListType(type))
            {
                if (!TypeInspector.Default.TryCreate(type, out Utilities.TypeInfo typeInfo))
                {
                    throw new ArgumentException(
                        FilterResources.FilterArrayFieldDescriptor_InvalidType,
                        nameof(property));
                }

                Type elementType = typeInfo.ClrType;
                ArrayFilterFieldDescriptor field;

                if (elementType == typeof(string)
                    || elementType == typeof(bool)
                    || typeof(IComparable).IsAssignableFrom(elementType))
                {
                    elementType = typeof(ISingleFilter<>).MakeGenericType(elementType);
                }

                field = new ArrayFilterFieldDescriptor(context, property, elementType);

                definition = field.CreateDefinition();
                return true;
            }

            definition = null;
            return false;
        }



        private static bool IsComparable(Type type)
        {
            if (typeof(IComparable).IsAssignableFrom(type))
            {
                return true;
            }

            if (type.IsValueType
                && type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return typeof(IComparable).IsAssignableFrom(
                    System.Nullable.GetUnderlyingType(type));
            }

            return false;
        }
    }
}