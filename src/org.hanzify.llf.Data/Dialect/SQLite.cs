
#region usings

using System;
using org.hanzify.llf.Data.Builder;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.Dialect
{
	public class SQLite : DbDialect
	{
		public SQLite() {}

        public override string GetConnectionString(string ConnectionString)
        {
            string s = base.ProcessConnectionnString(ConnectionString);
            if (s[0] == '@')
            {
                return "Cache Size=102400;Synchronous=Off;Data Source=" + s.Substring(1);
            }
            return s;
        }

        public override bool NeedBracketForJoin
        {
            get { return false; }
        }

        public override bool SupportsRange
        {
            get { return true; }
        }

        public override DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(null, new string[]{null, null, null, "table"}, null, null, null);
        }

        protected override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            SqlStatement Sql = base.GetNormalSelectSqlStatement(ssb);
            Sql.SqlCommandText = string.Format("{0} Limit {1}, {2}", 
                Sql.SqlCommandText, ssb.Range.Offset, ssb.Range.Rows);
            return Sql;
        }

        public override bool SupportsIdentitySelectInInsert
        {
            get { return true; }
        }

        public override string PrimaryKeyString
        {
            get { return ""; }
        }

        public override string IdentityTypeString
        {
            get { return "INTEGER PRIMARY KEY AUTOINCREMENT"; }
        }

        public override char CloseQuote
        {
            get { return ']'; }
        }

        public override char OpenQuote
        {
            get { return '['; }
        }

        public override string IdentityColumnString
        {
            get { return ""; }
        }

        public override string IdentitySelectString
		{
            get { return "SELECT last_insert_rowid();\n"; }
		}
	}
}