
using System;
using System.Collections;
using System.Text;
using Lephone.Data.Common;
using Lephone.Util;

namespace Lephone.Data.Caching
{
    public class StaticHashCacheProvider : CacheProvider
    {
        protected static Hashtable pool = new Hashtable(DataSetting.CacheSize);

        protected internal StaticHashCacheProvider() { }

        public override object this[string Key]
        {
            get
            {
                TimeValue tv = (TimeValue)pool[Key];
                if (tv == null)
                {
                    return null;
                }
                else if (NowProvider.Instance.Now > tv.ExpiredOn)
                {
                    Remove(Key);
                    return null;
                }
                return tv.Value;
            }
            set
            {
                if (value == null)
                {
                    Remove(Key);
                }
                else
                {
                    TimeValue tv = TimeValue.CreateTimeValue(value);
                    lock (pool.SyncRoot)
                    {
                        if (pool.Count > DataSetting.CacheSize)
                        {
                            pool.Clear();
                        }

                        pool[Key] = tv;
                    }
                }
            }
        }

        public override void Remove(string Key)
        {
            lock (pool.SyncRoot)
            {
                pool.Remove(Key);
            }
        }

        public override void Clear()
        {
            lock (pool.SyncRoot)
            {
                pool.Clear();
            }
        }
    }
}