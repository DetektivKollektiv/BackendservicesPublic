using System;

namespace DetektivKollektiv.DataLayer
{
    public class Item
    {
        public Guid ItemId { get; set; }
        public string Text { get; set; }
        public int ReviewGood { get; set; }
        public int ReviewBad { get; set; }
    }
}
