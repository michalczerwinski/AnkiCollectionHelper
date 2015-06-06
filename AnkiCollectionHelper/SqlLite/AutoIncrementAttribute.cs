using System;

namespace AnkiCollectionHelper.SqlLite
{
    [AttributeUsage (AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute
    {
    }
}