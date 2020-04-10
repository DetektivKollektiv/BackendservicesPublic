using Amazon.DynamoDBv2.DataModel;

namespace DetektivKollektiv.DataLayer
{
    [DynamoDBTable("items")]
    public class Item
    {
        [DynamoDBHashKey]
        public string ItemId { get; set; }
        public string Text { get; set; }
        public int ReviewGood { get; set; }
        public int ReviewBad { get; set; }
    }
}
