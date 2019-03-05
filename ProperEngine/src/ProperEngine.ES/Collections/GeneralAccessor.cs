using System;
using System.Collections.Generic;
using ProperEngine.ES.Raw;
using ProperEngine.ES.Raw.Collections;

namespace ProperEngine.ES.Collections
{
	public class GeneralAccessor<TEntityKey>
			: IAccessor<TEntityKey>
		where TEntityKey : struct
	{
		private readonly Dictionary<Type, IComponentMap> _mapLookup
			= new Dictionary<Type, IComponentMap>();
		
		private readonly Dictionary<Type, object> _rawMapLookup
			= new Dictionary<Type, object>();
		
		
		public void Add<TComponent>(IComponentMap<TEntityKey, TComponent> map)
		{
			if (map == null) throw new ArgumentNullException(nameof(map));
			_mapLookup.Add(typeof(TComponent), map);
		}
		
		public void Add<TComponent>(IRawComponentMap<TEntityKey, TComponent> map)
			where TComponent : struct
		{
			if (map == null) throw new ArgumentNullException(nameof(map));
			_rawMapLookup.Add(typeof(TComponent), map);
			_mapLookup.Add(typeof(Nullable<>).MakeGenericType(typeof(TComponent)),
			               new RawComponentMapProxy<TEntityKey, TComponent>(map));
		}
		
		
		// IAccessor implementation
		
		public IComponentMap<TEntityKey, TComponent> Component<TComponent>()
		{
			var type = typeof(TComponent);
			if (!_mapLookup.TryGetValue(type, out var map)) {
				if (type.IsValueType) {
					Type componentType = null;
					componentType = Nullable.GetUnderlyingType(type);
					if (componentType == null) throw new ArgumentException(
						$"Can't get IComponentMap<K,C> of '{ type }' - must be Nullable", nameof(TComponent));
					map = CreateAndAddComponentMap(componentType, type);
				} else {
					map = CreateAndAddComponentMap(type, null);
				}
			}
			return (IComponentMap<TEntityKey, TComponent>)map;
		}
		
		public IRawComponentMap<TEntityKey, TComponent> RawComponent<TComponent>()
			where TComponent : struct
		{
			if (_rawMapLookup.TryGetValue(typeof(TComponent), out var map))
				return (IRawComponentMap<TEntityKey, TComponent>)map;
			
			var rawMap = new RawComponentMap<TEntityKey, TComponent>();
			Add(rawMap);
			return rawMap;
		}
		
		public IComponentMap Component(Type componentType)
		{
			if (componentType == null) throw new ArgumentNullException(nameof(componentType));
			if (componentType.IsValueType) {
				if (Nullable.GetUnderlyingType(componentType) != null) throw new ArgumentException(
					$"Can't get IComponentMap of '{ componentType }' - it is Nullable", nameof(componentType));
				var nullableType = typeof(Nullable<>).MakeGenericType(componentType);
				return _mapLookup.TryGetValue(nullableType, out var map)
					? map : CreateAndAddComponentMap(componentType, nullableType);
			} else {
				return _mapLookup.TryGetValue(componentType, out var map)
					? map : CreateAndAddComponentMap(componentType, null);
			}
		}
		
		private IComponentMap CreateAndAddComponentMap(Type componentType, Type nullableType)
		{
			if (componentType.IsValueType) {
				
				if (nullableType == null) {
					nullableType  = componentType;
					componentType = Nullable.GetUnderlyingType(nullableType);
					if (componentType == null) throw new ArgumentException(
						"Component type is not Nullable", nameof(componentType));
				}
				
				var rawMap = Activator.CreateInstance(
					typeof(RawComponentMap<,>).MakeGenericType(
						typeof(TEntityKey), componentType));
				var map = (IComponentMap)Activator.CreateInstance(
					typeof(RawComponentMapProxy<,>).MakeGenericType(
						typeof(TEntityKey), componentType), rawMap);
				
				_rawMapLookup.Add(componentType, rawMap);
				_mapLookup.Add(nullableType, map);
				return map;
				
			} else {
				
				var map = (IComponentMap)Activator
					.CreateInstance(typeof(ComponentDictionary<,>)
					.MakeGenericType(typeof(TEntityKey), componentType));
				_mapLookup.Add(componentType, map);
				return map;
				
			}
		}
		
		public IEntityRef<TEntityKey> Entity(TEntityKey key)
		{
			throw new NotImplementedException();
		}
		
		public IEntityRef Entity(object key)
		{
			throw new NotImplementedException();
		}
	}
}
