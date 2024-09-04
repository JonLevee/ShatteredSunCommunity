using NLog.Filters;
using ShatteredSunCommunity.Models;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitFieldComparer : IComparer<UnitFieldValue>
    {
        public static readonly UnitFieldComparer Default = new UnitFieldComparer();
        public int Compare(UnitFieldValue? x, UnitFieldValue? y)
        {
            switch (x.UnitFieldType)
            {
                case UnitFieldTypeEnum.String:
                    return x.String.CompareTo(y.String);
                case UnitFieldTypeEnum.Double:
                    return (int)(x.Double - y.Double);
                case UnitFieldTypeEnum.Long:
                    return (int)(x.Long - y.Double);
                case UnitFieldTypeEnum.StringArray:
                    throw new NotImplementedException($"{x.UnitFieldType}");
                case UnitFieldTypeEnum.Image:
                    return x.String.CompareTo(y.String);
                case UnitFieldTypeEnum.Bool:
                    return x.String.CompareTo(y.String);
                default:
                    throw new NotImplementedException($"{x.UnitFieldType}");
            }
        }
    }
}