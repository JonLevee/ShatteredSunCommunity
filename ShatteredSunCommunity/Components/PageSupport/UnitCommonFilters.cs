using Microsoft.VisualBasic;
using ShatteredSunCommunity.Models;
using System;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public abstract class UnitCommonFilters : List<UnitCommonFilter>, IRefresh
    {
        protected readonly UnitViewFilters Parent;
        protected List<string> SelectorValues;

        public abstract string FirstItemHeader { get; }
        public abstract string RemainingItemHeader { get; }
        public virtual bool UseUnitFilterItems => false;
        public List<UnitCommonSelector> Selectors { get; }
        protected UnitCommonFilters(UnitViewFilters parent)
        {
            Parent = parent;
            SelectorValues = new List<string>
            {
                string.Empty,
            };
            Selectors = new List<UnitCommonSelector>();
            AddSelector();
            Parent.Changed += (o, e) => OnChanged();
        }

        public virtual void Refresh()
        {
            Parent.Refresh();
        }

        protected virtual void OnChanged()
        {

        }

        public IEnumerable<UnitCommonSelector> GetDisplayableSelectors()
        {
            foreach (var selector in Selectors)
            {
                yield return selector;
                if (!selector.IsActive)
                    yield break;
            }
        }


        internal UnitCommonFilter GetFilter(UnitCommonSelector selector)
        {
            return this.SingleOrDefault(f => f.Display == selector.Selected);
        }
        public virtual void UpdateSelectors(IEnumerable<UnitCommonSelector> otherSelectorsToCheck = null)
        {
            // to simplify processing, we will always remove inactive selectors then add a
            // single inactive selector to the tail so the user always has way to add a new filter
            var selectorsToCheck = null == otherSelectorsToCheck ? Selectors : otherSelectorsToCheck.Concat(Selectors);
            for (int i = 0; i < Selectors.Count; i++)
            {
                var selector = Selectors[i];
                if (!selector.IsActive)
                {
                    Selectors.RemoveAt(i--);
                    continue;
                }
                if (!TrySetDisabled(selector, selectorsToCheck))
                {
                    --i;
                }
            }
            AppendInactiveSelector(selectorsToCheck, true);
        }

        protected void AppendInactiveSelector(IEnumerable<UnitCommonSelector> selectorsToCheck, bool checkDisabled)
        {
            if (Selectors.Count < SelectorValues.Count)
            {
                // if the last one is a UnitFilter and CanFilter is falst, then don't add
                var last = Selectors.LastOrDefault();
                if (last == null ||
                    !(bool)last.FilterItem?.CanFilter)
                    AddSelector();
                if (checkDisabled)
                {
                    Debug.Assert(TrySetDisabled(Selectors.Last(), selectorsToCheck));
                }
            }
        }

        private void AddSelector()
        {
            Selectors.Add(new UnitCommonSelector(this, SelectorValues, UseUnitFilterItems));
        }
        private bool TrySetDisabled(UnitCommonSelector selector, IEnumerable<UnitCommonSelector> selectorsToCheck)
        {

            selector.IsCheckBoxDisabled = false;
            foreach (var option in selector.Options)
            {
                if (option.IsDefault)
                    option.IsDisabled = false;
                else
                {
                    option.IsDisabled = selectorsToCheck.Any(s => s != selector && s.Selected == option.Value);
                }
                if (option.IsSelected && option.IsDisabled)
                {
                    // a selector above us selected the same option as our selected option
                    // remove ourselves and shift everything up
                    Selectors.Remove(selector);
                    return false;
                }
            }
            return true;
        }

        public virtual void Add(UnitCommonFilter filter)
        {
            base.Add(filter);
            SelectorValues.Add(filter.Display);
        }
    }
}
