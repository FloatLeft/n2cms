﻿using System.Collections.Generic;
using N2.Collections;
using N2.Engine.Aspects;
using N2.Definitions;
using N2.Integrity;
using System.Security.Principal;

namespace N2.Web
{
	/// <summary>
	/// Controls aspects related to zones, zone definitions, and items to display in a zone.
	/// </summary>
	[Controls(typeof(ContentItem))]
	public class ZoneAspectController : IAspectController
	{
		#region IAspectController Members

		public PathData Path { get; set; }

		public N2.Engine.IEngine Engine { get; set; }

		#endregion

		/// <summary>Retrieves content items added to a zone of the parnet item.</summary>
		/// <param name="parentItem">The item whose items to get.</param>
		/// <param name="zoneName">The zone in which the items should be contained.</param>
		/// <returns>A list of items in the zone.</returns>
		public virtual ItemList GetItemsInZone(ContentItem parentItem, string zoneName)
		{
			return parentItem.GetChildren(zoneName);
		}

		public virtual IEnumerable<ItemDefinition> GetAllowedDefinitions(ContentItem item, string zoneName, IPrincipal user)
		{
			ItemDefinition containerDefinition = Engine.Definitions.GetDefinition(item.GetType());

			foreach (ItemDefinition childDefinition in containerDefinition.AllowedChildren)
			{
				if (childDefinition.IsAllowedInZone(zoneName) && childDefinition.Enabled && childDefinition.IsAuthorized(user))
				{
					yield return childDefinition;
				}
			}
		}
	}
}