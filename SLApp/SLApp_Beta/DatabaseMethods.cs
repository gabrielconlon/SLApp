using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;

namespace SLApp_Beta
{
    class DatabaseMethods
    {


        public bool CheckDatabaseConnection()
        {
            Mouse.SetCursor(Cursors.Wait);
            using (PubsDataContext db = new PubsDataContext())
            {
                if (db.DatabaseExists())
                {
                    Mouse.SetCursor(Cursors.Arrow);
                    return true;
                }
                else
                {
                    MessageBox.Show("Database connection is down.", "Database Connection Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    Mouse.SetCursor(Cursors.Arrow);
                    return false;
                }
            }
        }
    }

	class QueryFormatter
	{
		
		public QueryFormatter(string type, int count)
		{
			Type = type;
			Count = count;
		}

		public string Type;
		public int Count;

	}

	class ServiceOpportunity
	{
		public ServiceOpportunity()
		{

		}

		public ServiceOpportunity(string name, string service, string title, string description)
		{
			Agency = name;
			Service = service;
			Title = title;
			Description = description;
		}

		public string Agency { get; set; }
		public string Service { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
	}

	static partial class MoreEnumerable
	{
		/// <summary>
		/// Returns the maximal element of the given sequence, based on
		/// the given projection.
		/// </summary>
		/// <remarks>
		/// If more than one element has the maximal projected value, the first
		/// one encountered will be returned. This overload uses the default comparer
		/// for the projected type. This operator uses immediate execution, but
		/// only buffers a single result (the current maximal element).
		/// </remarks>
		/// <typeparam name="TSource">Type of the source sequence</typeparam>
		/// <typeparam name="TKey">Type of the projected element</typeparam>
		/// <param name="source">Source sequence</param>
		/// <param name="selector">Selector to use to pick the results to compare</param>
		/// <returns>The maximal element, according to the projection.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
		/// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>

		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> selector)
		{
			return source.MaxBy(selector, Comparer<TKey>.Default);
		}

		/// <summary>
		/// Returns the maximal element of the given sequence, based on
		/// the given projection and the specified comparer for projected values. 
		/// </summary>
		/// <remarks>
		/// If more than one element has the maximal projected value, the first
		/// one encountered will be returned. This overload uses the default comparer
		/// for the projected type. This operator uses immediate execution, but
		/// only buffers a single result (the current maximal element).
		/// </remarks>
		/// <typeparam name="TSource">Type of the source sequence</typeparam>
		/// <typeparam name="TKey">Type of the projected element</typeparam>
		/// <param name="source">Source sequence</param>
		/// <param name="selector">Selector to use to pick the results to compare</param>
		/// <param name="comparer">Comparer to use to compare projected values</param>
		/// <returns>The maximal element, according to the projection.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
		/// or <paramref name="comparer"/> is null</exception>
		/// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>

		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (selector == null) throw new ArgumentNullException("selector");
			if (comparer == null) throw new ArgumentNullException("comparer");
			using (var sourceIterator = source.GetEnumerator())
			{
				if (!sourceIterator.MoveNext())
				{
					throw new InvalidOperationException("Sequence contains no elements");
				}
				var max = sourceIterator.Current;
				var maxKey = selector(max);
				while (sourceIterator.MoveNext())
				{
					var candidate = sourceIterator.Current;
					var candidateProjected = selector(candidate);
					if (comparer.Compare(candidateProjected, maxKey) > 0)
					{
						max = candidate;
						maxKey = candidateProjected;
					}
				}
				return max;
			}
		}
	}
}
