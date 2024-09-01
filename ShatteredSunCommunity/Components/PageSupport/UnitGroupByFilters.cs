using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitGroupByFilters : IEnumerable<UnitGroupByFilter>
    {
        private List<string> selectorValues;
        private List<DataHeader> headers;
        private List<UnitGroupByFilter> Filters { get; }
        public List<UnitGroupBySelector> Selectors { get; }
        public UnitGroupByFilters() 
        { 
            Filters = new List<UnitGroupByFilter>();
            Selectors = new List<UnitGroupBySelector>();
            headers = new List<DataHeader>();
            selectorValues = new List<string>
            {
                string.Empty,
            };
        }

        [DebuggerDisplay("{Header}[{Colspan}]")]
        public class DataHeader
        {
            public string Header { get; }
            public int Colspan { get; }

            public DataHeader(string header, int colspan)
            {
                Header = header;
                Colspan = colspan;
            }
        }
        public IEnumerable<DataHeader> GetDataHeaders(UnitGroupBySelector selector)
        {
            if (!selector.IsActive)
            {
                return [];
            }
            var filter = Filters.SingleOrDefault(f=>f.Display == selector.Selected);
            var activeSelectorInfo = Selectors
                .Where(s=>s.IsActive)
                .Select(s=>new {s,f= Filters.SingleOrDefault(f => f.Display == s.Selected) })
                .ToList();
            var colspan = activeSelectorInfo
                .SkipWhile(item=>item.s != selector)
                .Skip(1)
                .Aggregate(1,(_colspan,item) =>_colspan * item.f.Values.Count);
            var repeatCols = activeSelectorInfo
                .TakeWhile(item => item.s != selector)
                .Aggregate(1, (_colspan, item) => _colspan * item.f.Values.Count);
        
            var headers = Enumerable
                .Range(0, repeatCols)
                .SelectMany(i=>filter.Values.Select(v => new DataHeader(v, colspan)))
                .ToList();
            return headers;
        }

        public void OnChanged()
        {
            foreach(var selector in Selectors)
            {
                foreach(var option in selector.Options)
                {
                    option.IsDisabled = Selectors.Any(s => s != selector && s.IsActive && s.Selected == option.Value);
                }
            }
        }
        public void Add(string field, IEnumerable<string> values)
        {
            var filter = new UnitGroupByFilter(Filters.Count, field, values);
            Filters.Add(filter);
            selectorValues.Add(filter.Display);
            Selectors.Add(new UnitGroupBySelector(filter.Id, selectorValues));
        }
        public IEnumerator<UnitGroupByFilter> GetEnumerator()
        {
            return Filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Filters.GetEnumerator();
        }
    }
}
