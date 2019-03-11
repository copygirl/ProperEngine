using System;
using System.Globalization;
using System.Text;

namespace ProperEngine.Utility
{
	public static class StringExtension
	{
		/// <summary>
		/// Returns the specified string as a C# literal encased in double quotes,
		/// with control, formatting characters and similar escaped.
		/// </summary>
		public static string ToLiteral(this string str)
		{
			if (str == null) return "null";
			var sb = new StringBuilder(str.Length + 2);
			sb.Append('"');
			
			foreach (var chr in str)
				switch (chr) {
					// case '\'': sb.Append(@"\'"); break;
					case '\"': sb.Append(@"\"""); break;
					case '\\': sb.Append(@"\\"); break;
					case '\0': sb.Append(@"\0"); break;
					case '\a': sb.Append(@"\a"); break;
					case '\b': sb.Append(@"\b"); break;
					case '\f': sb.Append(@"\f"); break;
					case '\n': sb.Append(@"\n"); break;
					case '\r': sb.Append(@"\r"); break;
					case '\t': sb.Append(@"\t"); break;
					case '\v': sb.Append(@"\v"); break;
					default:
						var category = Char.GetUnicodeCategory(chr);
						// Escape control and formatting characters.
						if ((category == UnicodeCategory.Control) ||
						    (category == UnicodeCategory.Format))
							sb.AppendFormat(@"\u{0:X4}", chr);
						// Everything else can just be included as is.
						else sb.Append(chr);
						break;
				}
			
			sb.Append('"');
			return sb.ToString();
		}
	}
}
