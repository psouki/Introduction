using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToSql.DynamicLambda
{
    public class QueryFilter
    {
        public QueryFilter(string propertyName, object value)
        {
            PropertyName = propertyName;
            Value = value;
        }
        public QueryFilter(string propertyName, object value, Operator operatorValue)
        {
            PropertyName = propertyName;
            Value = value;
            Operator = operatorValue;
        }

        public string PropertyName { get; set; }
        public object Value { get; set; }
        public Operator Operator { get; set; } = Operator.Equals;
    }
}
