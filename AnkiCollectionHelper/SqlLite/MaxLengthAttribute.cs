using System;

namespace AnkiCollectionHelper.SqlLite
{
    [AttributeUsage (AttributeTargets.Property)]
    public class MaxLengthAttribute : Attribute
    {
        public int Value { get; private set; }

        public MaxLengthAttribute (int length)
        {
            Value = length;
        }
    }
}