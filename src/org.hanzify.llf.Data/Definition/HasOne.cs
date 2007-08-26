
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Driver;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [Serializable]
    public class HasOne<T> : LazyLoadOneBase<T>, IRenew
    {
        private OrderBy Order;

        internal HasOne(object owner) : base(owner) { }

        public HasOne(object owner, OrderBy Order)
            : base(owner)
        {
            this.Order = Order;
        }

        public HasOne(object owner, string OrderByString)
            : base(owner)
        {
            this.Order = OrderBy.Parse(OrderByString);
        }

        protected override void DoWrite(bool IsLoad)
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(typeof(T));
            if (oi.BelongsToField != null)
            {
                Type t = oi.BelongsToField.FieldType.GetGenericArguments()[0];
                if (t == owner.GetType())
                {
                    ILazyLoading ll = (ILazyLoading)oi.BelongsToField.GetValue(m_Value);
                    ll.Write(owner, false);
                }
            }
        }

        protected override void DoSetOwner()
        {
            if (Order == null)
            {
                ObjectInfo oi = DbObjectHelper.GetObjectInfo(owner.GetType());
                if (oi.HasSystemKey)
                {
                    Order = new OrderBy(oi.KeyFields[0].Name);
                }
            }
        }

        protected override void DoLoad()
        {
            if (RelationName == null) { return; }
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(owner.GetType());
            object key = oi.KeyFields[0].GetValue(owner);
            m_Value = context.GetObject<T>(CK.K[RelationName] == key, Order);

            if (m_Value != null)
            {
                ObjectInfo ti = DbObjectHelper.GetObjectInfo(typeof(T));
                if (ti.BelongsToField != null)
                {
                    ILazyLoading ll = (ILazyLoading)ti.BelongsToField.GetValue(m_Value);
                    ll.Write(owner, true);
                }
            }
        }

        void IRenew.SetAsNew()
        {
            if (m_Value != null)
            {
                MemberHandler f = DbObjectHelper.GetObjectInfo(typeof(T)).KeyFields[0];
                f.SetValue(m_Value, f.UnsavedValue);
            }
        }
    }
}