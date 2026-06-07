using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.RevitTools.ViewModels
{
    public class ElementSelectorViewModel
    {
        public string HeaderText { get; }

        public List<SelectableItem> Items { get; }

        public ElementSelectorViewModel(
            IEnumerable<object> items,
            Func<object, string> displayFunction,
            string headerText)
        {
            HeaderText = headerText;

            Items = items
                .Select(item => new SelectableItem
                {
                    Item = item,
                    DisplayName = displayFunction(item),
                    IsSelected = false
                })
                .ToList();
        }
        public List<T> GetSelectedItems<T>()
        {
            return Items
                .Where(item => item.IsSelected)
                .Select(item => (T)item.Item)
                .ToList();
        }

        public void SelectAll()
        {
            foreach (SelectableItem item in Items)
            {
                item.IsSelected = true;
            }
        }
    }
}