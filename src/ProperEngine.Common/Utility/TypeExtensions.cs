using System;

namespace ProperEngine.Common.Utility
{
	public static class TypeExtensions
	{
		public static TResult Ensure<TValue, TResult>(
			this TValue value, string paramName)
		{
			if (value == null) throw new ArgumentNullException(paramName);
			if (!(value is TResult result)) throw new ArgumentException(
				$"Argument { paramName } is type '{ value.GetType() }' but expected '{ typeof(TResult) }'", paramName);
			return result;
		}
		
		public static TResult Ensure<TResult>(
			this object value, string paramName)
				=> Ensure<object, TResult>(value, paramName);
	}
}
