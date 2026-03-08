using System.ComponentModel;
using System.Windows.Data;

namespace TelAvivMuni_Exercise.Presentation.Extensions;

/// <summary>
/// Extension methods for <see cref="ICollectionView"/>.
/// </summary>
internal static class CollectionViewExtensions
{
	/// <summary>
	/// Determines whether an item passes the full filter pipeline of the collection view,
	/// including any overridden filter logic in concrete implementations such as
	/// <see cref="ListCollectionView"/> or <see cref="BindingListCollectionView"/>.
	/// Falls back to invoking <see cref="ICollectionView.Filter"/> directly for any
	/// custom <see cref="ICollectionView"/> implementation that does not derive from
	/// <see cref="CollectionView"/>.
	/// </summary>
	/// <param name="collectionView">The collection view to evaluate against.</param>
	/// <param name="item">The item to test.</param>
	/// <returns>
	/// <see langword="true"/> if the item passes the filter or no filter is active;
	/// otherwise, <see langword="false"/>.
	/// </returns>
	public static bool PassesFilter(this ICollectionView collectionView, object item)
	{
		// Prefer the concrete CollectionView.PassesFilter, which correctly handles
		// overridden filter logic in ListCollectionView and BindingListCollectionView.
		if (collectionView is CollectionView cv)
			return cv.PassesFilter(item);

		// Fallback for custom ICollectionView implementations not derived from CollectionView.
		return collectionView.Filter?.Invoke(item) ?? true;
	}
}