namespace AkkaNet.Examples.Restaurant.PoC.Events
{
	/// <summary>
	/// Represents the needs of a customer as example data.
	/// </summary>
	public enum CustomerNeeds
    {
		/// <summary>
		/// The customer's needs are unknown.
		/// </summary>
		Unbekannt,

		/// <summary>
		/// The customer has a strong peanut allergy.
		/// </summary>
		StarkeErdnussallergie,

		/// <summary>
		/// The customer has a tomato intolerance.
		/// </summary>
		TomatenUnverträglichkeit,

		/// <summary>
		/// The customer wants to sit at the window side.
		/// </summary>
		Fensterplatz,

		/// <summary>
		/// The customer wants not to sit near the restroom.
		/// </summary>
		NichtNebenDerToilette,

		/// <summary>
		/// The customer wants to sit in a quiet area.
		/// </summary>
		RuheBereich
	}

}
