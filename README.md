# telerik-to-strawberry-shake
Extension for mapping Telerik 'Grid' to ChilliCream StrawberryShake 'GraphQL'

# Usage
````csharp
private static readonly GraphQlMapper mapper = new GraphQlMapperBuilder<IMyGeneratedType>()
                                                   .ExtendFilterInput<MyGeneratedTypeFilterInput>()
                                                   .ExtendSortInput<MyGeneratedTypeSortInput>()
                                                   .AddFilteringSorting(obj => obj.Value)
                                                   .AddFilteringSorting(obj => obj.Tree.Leaf)
                                                   .Build();
````
