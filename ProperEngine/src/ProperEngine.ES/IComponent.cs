using System;

namespace ProperEngine.ES
{
	public interface IComponent {  }
	
	
	public static class ComponentExtensions
	{
		public static TComponent Ensure<TComponent>(
				this IComponent component, string paramName)
			where TComponent : IComponent
		{
			if (component == null) throw new ArgumentNullException(paramName);
			if (!(component is TComponent c)) throw new ArgumentException(
				$"Component is type '{ component.GetType() }' but expected '{ typeof(TComponent) }'", paramName);
			return c;
		}
	}
}
