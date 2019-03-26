﻿using System;

namespace LinqToDB.Linq.Parser.Clauses
{
	public abstract class BaseSetClause : BaseClause, IQuerySource2
	{
		public BaseSetClause([JetBrains.Annotations.NotNull] Type itemType, string itemName, [JetBrains.Annotations.NotNull] Sequence sequence1, [JetBrains.Annotations.NotNull] Sequence sequence2)
		{
			ItemType = itemType ?? throw new ArgumentNullException(nameof(itemType));
			ItemName = itemName;
			Sequence1 = sequence1 ?? throw new ArgumentNullException(nameof(sequence1));
			Sequence2 = sequence2 ?? throw new ArgumentNullException(nameof(sequence2));
			QuerySourceId = QuerySourceHelper.GetNexSourceId();
		}

		public int QuerySourceId { get; }
		public Type ItemType { get; }
		public string ItemName { get; }

		public Sequence Sequence1 { get; }
		public Sequence Sequence2 { get; }

		public override BaseClause Visit(Func<BaseClause, BaseClause> func)
		{
			var sequence1 = Sequence1.Visit(func);
			var sequence2 = Sequence2.Visit(func);

			var current = this;
			if (sequence1 != Sequence1 || sequence2 != Sequence2)
				current = new UnionClause(ItemType, ItemName, Sequence1, Sequence2);

			return func(current);
		}

		public abstract bool AllFieldsRequired { get; }

		public override bool VisitParentFirst(Func<BaseClause, bool> func)
		{
			return func(this) && Sequence1.VisitParentFirst(func) && Sequence2.VisitParentFirst(func);
		}

	}
}