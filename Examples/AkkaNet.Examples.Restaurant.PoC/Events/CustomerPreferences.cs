namespace AkkaNet.Examples.Restaurant.PoC.Events
{
    public enum CustomerPreferences
    {
		/// <summary>
		/// The customer's preferences are unknown.
		/// </summary>
		Unbekannt,
		/// <summary>
		/// The customer prefers wine.
		/// </summary>
		Weinkarte,
		/// <summary>
		/// The customer prefers espresso.
		/// </summary>
		Espresso,
		/// <summary>
		/// The customer prefers a schnaps after the meal.
		/// </summary>
		Schnaps,
		/// <summary>
		/// The customer prefers a vegetarian food.
		/// </summary>
		Vegetarisch,
		/// <summary>
		/// The customer prefers a vegan food.
		/// </summary>
		Vegan,
		/// <summary>
		/// The customer prefers a gluten-free food.
		/// </summary>
		GlutenFrei,
	}
}
