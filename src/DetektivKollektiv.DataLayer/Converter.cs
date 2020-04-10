using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;
using DetektivKollektiv.DataLayer;

namespace DetektivKollektiv.LambdaFunctions
{
    public static class Converter
    {
        public static Item ConvertAttributesToItem(Dictionary<string, AttributeValue> dict)
        {
            Item item = new Item();
            AttributeValue itemId = new AttributeValue();
            AttributeValue text = new AttributeValue();
            AttributeValue reviewGood = new AttributeValue();
            AttributeValue reviewBad = new AttributeValue();

            dict.TryGetValue("ItemId", out itemId);
            dict.TryGetValue("Text", out text);
            dict.TryGetValue("ReviewGood", out reviewGood);
            dict.TryGetValue("ReviewBad", out reviewBad);

            item.ItemId = itemId.S;
            item.Text = text.S;
            item.ReviewBad = Int32.Parse(reviewBad.N);
            item.ReviewGood = Int32.Parse(reviewGood.N);

            return item;
        }
    }
}
