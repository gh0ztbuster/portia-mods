using System;
using System.Collections.Generic;

namespace ItemPickup
{
    [Serializable]
    public class ItemPickupConfig
    {
        public bool PreferBagFirst = false;

        public class ItemIgnoreConfig
        {
            public string MatchMethod = "exact";
            public string Language = "English";
            public List<string> Items = new List<string>();
        }
        public ItemIgnoreConfig ItemIgnore;
    }
}
