using System.Linq.Expressions;
using System.Reflection;
using TelerikToStrawberryShake.Exceptions;
using TelerikToStrawberryShake.Models;

namespace TelerikToStrawberryShake.Mappings;

internal sealed class GraphQlMapperBuilder<T>
{
    private const string operation = "OperationFilterInput";

    private const string readModelPrefix = "ReadModel_";

    private const string filterSuffix = "FilterInput";

    private const string sortSuffix = "SortInput";

    private static readonly IEnumerable<Type> graphQlTypes = typeof(T).Assembly.GetTypes();

    private readonly IDictionary<string, GraphQlFilterDescriptor> _graphQlFilterDescriptors = new Dictionary<string, GraphQlFilterDescriptor>();

    private readonly IDictionary<string, GraphQlSortDescriptor> _graphQlSortDescriptors = new Dictionary<string, GraphQlSortDescriptor>();

    private Type? _filterInputTypeExtension;

    private Type? _sortInputTypeExtension;

    public GraphQlMapperBuilder<T> AddFiltering<TMember>(Expression<Func<T, TMember>> memberPicker)
    {
        var members = GetChain((MemberExpression)memberPicker.Body);

        var filter = CreateFilterDescriptor<TMember>(members, members.Pop());

        _graphQlFilterDescriptors.Add(filter.Path(), filter);

        return this;
    }

    public GraphQlMapperBuilder<T> AddFilteringSorting<TMember>(Expression<Func<T, TMember>> memberPicker)
    {
        AddFiltering(memberPicker);
        AddSorting(memberPicker);

        return this;
    }

    public GraphQlMapperBuilder<T> AddSorting<TMember>(Expression<Func<T, TMember>> memberPicker)
    {
        var members = GetChain((MemberExpression)memberPicker.Body);

        var sort = CreateSortDescriptor<TMember>(members, members.Pop());

        _graphQlSortDescriptors.Add(sort.Path(), sort);

        return this;
    }

    public GraphQlMapper Build()
    {
        var tName = typeof(T).Name[1..];

        var rootFilterType = graphQlTypes.SingleOrDefault(obj => obj.Name == $"{tName}{filterSuffix}") ?? _filterInputTypeExtension;

        var rootSortType = graphQlTypes.SingleOrDefault(obj => obj.Name == $"{tName}{sortSuffix}") ?? _sortInputTypeExtension;

        return new GraphQlMapper(rootFilterType, rootSortType, _graphQlFilterDescriptors, _graphQlSortDescriptors);
    }

    public GraphQlMapperBuilder<T> ExtendFilterInput<TFilterInput>()
    {
        _filterInputTypeExtension = typeof(TFilterInput);
        return this;
    }

    public GraphQlMapperBuilder<T> ExtendSortInput<TSortInput>()
    {
        _sortInputTypeExtension = typeof(TSortInput);
        return this;
    }

    private static GraphQlFilterDescriptor CreateFilterDescriptor<TMember>(Stack<Member> stack, Member member)
    {
        if (IsSimple(member))
        {
            return CreateSimpleFilterDescriptor(member);
        }

        var filterDescriptor = CreateFilterDescriptor<TMember>(stack, stack.Pop());

        var complexFilterType = graphQlTypes.Where(t => t.Name.EndsWith(filterSuffix))
                                            .SingleOrDefault(obj => obj.Name.Equals($"{readModelPrefix}{member.MemberInfo.Name}{filterSuffix}"));

        if (complexFilterType == null)
        {
            throw new NotSupportedException($"System type '{member.MemberInfo.Name}' not supported as complex GraphQL filter input");
        }

        return new GraphQlFilterDescriptor(
                                           member.MemberInfo.Name,
                                           complexFilterType,
                                           complexFilterType.Name,
                                           filterDescriptor);
    }

    private static GraphQlFilterDescriptor CreateSimpleFilterDescriptor(Member member)
    {
        var fieldName = member.MemberInfo.Name;
        var fieldType = member.Type;

        string? filterType = null;
        if (fieldType == typeof(string))
        {
            filterType = $"String{operation}";
        }

        if (fieldType == typeof(bool) ||
            fieldType == typeof(bool?))
        {
            filterType = $"Boolean{operation}";
        }

        if (fieldType == typeof(int) || fieldType == typeof(int?))
        {
            filterType = $"Int{operation}";
        }

        if (fieldType == typeof(Guid) || fieldType == typeof(Guid?))
        {
            filterType = $"Uuid{operation}";
        }

        if (fieldType == typeof(DateTimeOffset) || fieldType == typeof(DateTimeOffset?))
        {
            filterType = $"DateTime{operation}";
        }

        if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
        {
            filterType = $"Date{operation}";
        }

        return new GraphQlFilterDescriptor(
                                           fieldName,
                                           fieldType,
                                           filterType ?? throw new MissingGraphQlFilterDescriptorMappingException(fieldType),
                                           null);
    }

    private static GraphQlSortDescriptor CreateSimpleSortDescriptor(Member member) => new(member.MemberInfo.Name, typeof(SortEnumType), null);

    private static GraphQlSortDescriptor CreateSortDescriptor<TMember>(Stack<Member> stack, Member member)
    {
        if (IsSimple(member))
        {
            return CreateSimpleSortDescriptor(member);
        }

        var sortDescriptor = CreateSortDescriptor<TMember>(stack, stack.Pop());

        var complexSortType = graphQlTypes.Where(t => t.Name.EndsWith(sortSuffix))
                                          .SingleOrDefault(obj => obj.Name.Equals($"{readModelPrefix}{member.MemberInfo.Name}{sortSuffix}"));

        if (complexSortType == null)
        {
            throw new NotSupportedException($"System type '{member.MemberInfo.Name}' not supported as complex GraphQL sort input");
        }

        return new GraphQlSortDescriptor(
                                         member.MemberInfo.Name,
                                         complexSortType,
                                         sortDescriptor);
    }

    private static Stack<Member> GetChain(Expression? expression)
    {
        var stack = new Stack<Member>();

        while (expression != null)
        {
            var member = expression switch
                         {
                             MemberExpression { Expression: { } target, Member: { } propertyOrField } =>
                                 new Member(expression, propertyOrField, target),
                             _ => default
                         };

            if (member?.Expression == null)
            {
                break;
            }

            stack.Push(member);
            expression = member.Target;
        }

        return stack;
    }

    private static bool IsSimple(Member member)
    {
        var type = member.Type;
        return
            type == typeof(string) ||
            type == typeof(bool) ||
            type == typeof(bool?) ||
            type == typeof(int) ||
            type == typeof(int?) ||
            type == typeof(Guid) ||
            type == typeof(Guid?) ||
            type == typeof(DateTimeOffset) ||
            type == typeof(DateTimeOffset?) ||
            type == typeof(DateTime) ||
            type == typeof(DateTime?);
    }

    private sealed record Member(Expression Expression, MemberInfo MemberInfo, Expression Target)
    {
        public Type Type => ((PropertyInfo)MemberInfo).PropertyType;
    }
}