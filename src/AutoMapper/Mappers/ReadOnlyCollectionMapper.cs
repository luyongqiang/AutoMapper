namespace AutoMapper.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Configuration;

    public class ReadOnlyCollectionMapper : IObjectMapper
    {
        public object Map(ResolutionContext context)
        {
            Type genericType = typeof (EnumerableMapper<>);

            var elementType = TypeHelper.GetElementType(context.DestinationType);

            var enumerableMapper = genericType.MakeGenericType(elementType);

            var objectMapper = (IObjectMapper) Activator.CreateInstance(enumerableMapper);

            var nullDestinationValueSoTheReadOnlyCollectionMapperWorks =
                    new ResolutionContext(context.SourceValue, null, context.Types, context);

            return objectMapper.Map(nullDestinationValueSoTheReadOnlyCollectionMapperWorks);
        }

        public bool IsMatch(TypePair context)
        {
            if (!(context.SourceType.IsEnumerableType() && context.DestinationType.IsGenericType()))
                return false;

            var genericType = context.DestinationType.GetGenericTypeDefinition();

            return genericType == typeof (ReadOnlyCollection<>);
        }

        #region Nested type: EnumerableMapper

        private class EnumerableMapper<TElement> : EnumerableMapperBase<IList<TElement>>
        {
            private readonly IList<TElement> inner = new List<TElement>();

            public override bool IsMatch(TypePair context)
            {
                throw new NotImplementedException();
            }

            protected override void SetElementValue(IList<TElement> elements, object mappedValue, int index)
            {
                inner.Add((TElement) mappedValue);
            }

            protected override IList<TElement> GetEnumerableFor(object destination)
            {
                return inner;
            }

            protected override IList<TElement> CreateDestinationObjectBase(Type destElementType, int sourceLength)
            {
                throw new NotImplementedException();
            }

            protected override object CreateDestinationObject(ResolutionContext context, Type destinationElementType, int count)
            {
                return new ReadOnlyCollection<TElement>(inner);
            }
        }

        #endregion
    }
}