using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amadues
{
    public class InventoryTab
    {
        public String title = "UNDEFINED";
        public SC.InventoryFilterType filterType = SC.InventoryFilterType.ALL;
        public List<Item> items;

        public InventoryTab()
        {
            items = new List<Item>();
        }

        public InventoryTab(String aTitle, SC.InventoryFilterType ft, List<Item> list)
        {
            title = aTitle;
            filterType = ft;
            if (list != null) UpdateList(list);
        }

        public void UpdateList(List<Item> list)
        {
            items = new List<Item>();
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    switch (filterType)
                    {
                        case SC.InventoryFilterType.ALL:
                            items.Add(list[i]);
                            break;
                        case SC.InventoryFilterType.ARMOUR:
                            if (list[i].itemType == (int)SC.ItemTypes.ARMOUR
                                || list[i].itemType == (int)SC.ItemTypes.SHIELD
                                || list[i].itemType == (int)SC.ItemTypes.CLOAK)
                                items.Add(list[i]);
                            break;
                        case SC.InventoryFilterType.FOOD:
                            if (list[i].itemType == (int)SC.ItemTypes.FOOD) items.Add(list[i]);
                            break;
                        case SC.InventoryFilterType.OTHER:
                            if (list[i].itemType == (int)SC.ItemTypes.QUEST
                                || list[i].itemType == (int)SC.ItemTypes.ROCK)
                                items.Add(list[i]);
                            break;
                        case SC.InventoryFilterType.POTION:
                            if (list[i].itemType == (int)SC.ItemTypes.POTION) items.Add(list[i]);
                            break;
                        case SC.InventoryFilterType.SCROLL:
                            if (list[i].itemType == (int)SC.ItemTypes.SCROLL) items.Add(list[i]);
                            break;
                        case SC.InventoryFilterType.WEAPON:
                            if (list[i].itemType == (int)SC.ItemTypes.WEAPON) items.Add(list[i]);
                            break;
                    }
                }
            }
        }
    }
}
