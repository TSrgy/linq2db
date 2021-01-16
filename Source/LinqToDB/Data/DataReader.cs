﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LinqToDB.Data
{
	public class DataReader : IDisposable
	{
		private DataReaderWrapper? _wrapper;

		public   DbDataReader?    Reader      => _wrapper?.DataReader;
		public   CommandInfo?     CommandInfo { get; }
		internal int              ReadNumber  { get; set; }
		private  DateTime         StartedOn   { get; }      = DateTime.UtcNow;
		private  Stopwatch        Stopwatch   { get; }      = Stopwatch.StartNew();
		internal Action?          OnDispose   { get; set; }

		public DataReader(CommandInfo commandInfo, DataReaderWrapper dataReader)
		{
			CommandInfo = commandInfo;
			_wrapper    = dataReader;
		}

		public void Dispose()
		{
			if (_wrapper != null)
			{
				if (CommandInfo?.DataConnection.TraceSwitchConnection.TraceInfo == true)
				{
					CommandInfo.DataConnection.OnTraceConnection(new TraceInfo(CommandInfo.DataConnection, TraceInfoStep.Completed)
					{
						TraceLevel      = TraceLevel.Info,
						Command         = _wrapper.Command,
						StartTime       = StartedOn,
						ExecutionTime   = Stopwatch.Elapsed,
						RecordsAffected = ReadNumber,
					});
				}

				_wrapper.Dispose();
				_wrapper = null;
			}

			OnDispose?.Invoke();
		}

		#region Query with object reader

		public IEnumerable<T> Query<T>(Func<IDataReader,T> objectReader)
		{
			while (Reader!.Read())
				yield return objectReader(Reader);
		}

		#endregion

		#region Query

		public IEnumerable<T> Query<T>()
		{
			if (ReadNumber != 0)
				if (!Reader!.NextResult())
					return Enumerable.Empty<T>();

			ReadNumber++;

			return CommandInfo!.ExecuteQuery<T>(Reader!, CommandInfo.CommandText + "$$$" + ReadNumber);
		}

		#endregion

		#region Query with template

		public IEnumerable<T> Query<T>(T template)
		{
			return Query<T>();
		}

		#endregion

		#region Execute scalar

		[return: MaybeNull]
		public T Execute<T>()
		{
			if (ReadNumber != 0)
				if (!Reader!.NextResult())
					return default(T);

			ReadNumber++;

			var sql = CommandInfo!.CommandText + "$$$" + ReadNumber;

			return CommandInfo.ExecuteScalar<T>(Reader!, sql);
		}

		#endregion
	}
}
