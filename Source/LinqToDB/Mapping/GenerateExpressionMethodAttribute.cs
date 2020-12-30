﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace LinqToDB.Mapping
{
	/// <summary>
	/// <para>
	/// When applied to method or propoerty, tells linq2db to create an expression version of the member.
	/// This is only supported in C# 9 or later, using Source Generators.
	/// </para>
	/// 
	/// <para>
	/// The new method by default will be marked <c>private</c>, with the generated name <c>__&lt;name&gt;Expression</c>.
	/// However, if <see cref="MethodName"/> is provided, the new methohd will use that name and be marked <c>public</c>.
	/// </para>
	/// 
	/// <para>
	/// Requirements:
	/// </para>
	/// 
	/// <list type="bullet">
	/// <item>
	/// <term><c>class</c></term>
	/// <description>The containing class must be marked <c>partial</c>, so that the generator can add new methods to it.</description>
	/// </item> 
	/// <item>
	/// <term><c>method</c> or <c>property</c></term>
	/// <description>
	/// <list type="bullet">
	/// <item>Must not have <c>void</c> return type.</item>
	/// <item>Must have a single statement returning a mapped object.</item>
	/// </list>
	/// </description>
	/// </item>
	/// </list>
	/// 
	/// </summary>
	/// 
	/// <remarks>
	/// See <see href="https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/"/> for additional information.
	/// </remarks>
	[PublicAPI]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class GenerateExpressionMethodAttribute : Attribute
	{
		/// <summary>
		/// Name of method in the same class that returns substitution expression.
		/// </summary>
		public string? MethodName { get; set; }

		/// <summary>
		/// Mapping schema configuration name, for which this attribute should be taken into account.
		/// <see cref="ProviderName"/> for standard names.
		/// Attributes with <c>null</c> or empty string <see cref="Configuration"/> value applied to all configurations (if no attribute found for current configuration).
		/// </summary>
		public string? Configuration { get; set; }
	}
}