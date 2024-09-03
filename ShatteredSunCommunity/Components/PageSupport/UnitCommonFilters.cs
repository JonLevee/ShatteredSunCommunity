using Microsoft.VisualBasic;
using ShatteredSunCommunity.Models;
using System;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public abstract class UnitCommonFilters : List<UnitCommonFilter>, ISelectorOwner
    {
        protected readonly UnitViewFilters Parent;
        public abstract string FirstItemHeader { get; }
        public abstract string RemainingItemHeader { get; }
        public List<UnitCommonSelector> Selectors { get; }
        protected UnitCommonFilters(UnitViewFilters parent)
        {
            this.Parent = parent;
            Selectors = new List<UnitCommonSelector>();
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
        public void UpdateSelectors(IEnumerable<UnitCommonSelector> otherSelectorsToCheck = null)
        {
            var selectorsToCheck = null == otherSelectorsToCheck ? Selectors : otherSelectorsToCheck.Concat(Selectors);
            for (int i = 0; i < Selectors.Count; i++)
            {
                var selector = Selectors[i];
                selector.IsCheckBoxDisabled = false;
                foreach (var option in selector.Options)
                {
                    option.IsSelected = option.Value == selector.Selected;
                    if (option.Value == string.Empty)
                        option.IsDisabled = false;
                    else
                    {
                        option.IsDisabled = selectorsToCheck.Any(s => s != selector && s.Selected == option.Value);
                    }
                     if (option.IsSelected && option.IsDisabled)
                    {
                        // a selector above us selected the same option as our selected option
                        // remove ourselves and shift everything up
                        for(int j = i + 1; j < Selectors.Count; j++)
                        {
                            Selectors[i].SetSelected(Selectors[j].Selected, Selectors[j].IsCheckBoxChecked);
                        }
                        Selectors[Selectors.Count-1].SetSelected(string.Empty, false);
                        UpdateSelectors(otherSelectorsToCheck);
                        return;
                    }
                }
            }
        }
    }
}
