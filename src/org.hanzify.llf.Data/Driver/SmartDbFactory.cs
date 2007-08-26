
#region usings

using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;

using org.hanzify.llf.util;
using org.hanzify.llf.util.Setting;

#endregion

namespace org.hanzify.llf.Data.Driver
{
    internal class SmartDbFactory : DbFactory
    {
        private object[] CiParam = new object[] { };
        private ConstructorInfo CiCommand;
        private ConstructorInfo CiConnection;
        private ConstructorInfo CiDataAdapter;
        private ConstructorInfo CiParameter;
        private MethodInfo MiCb_DeriveParameters = null;

        public bool DeriveParametersIsValid
        {
            get { return MiCb_DeriveParameters != null; }
        }

        public SmartDbFactory() { }

        public void Init(string AssemblyName)
        {
            InitWithAssemblyName(AssemblyName);
        }

        private void InitWithAssemblyName(string AssemblyName)
        {
            Type[] EmptyParam = new Type[] { };
            Assembly asm = Assembly.Load(AssemblyName);
            Type[] ts = asm.GetTypes();
            Type CommandType = null;
            foreach (Type t in ts)
            {
                if (CiCommand == null && IsInterfaceOf(t, typeof(IDbCommand)))
                {
                    CommandType = t;
                    CiCommand = t.GetConstructor(EmptyParam);
                }
                if (CiConnection == null && IsInterfaceOf(t, typeof(IDbConnection)))
                {
                    CiConnection = t.GetConstructor(EmptyParam);
                }
                if (CiDataAdapter == null && IsInterfaceOf(t, typeof(IDbDataAdapter)))
                {
                    CiDataAdapter = t.GetConstructor(EmptyParam);
                }
                if (CiParameter == null && IsInterfaceOf(t, typeof(IDbDataParameter)))
                {
                    CiParameter = t.GetConstructor(EmptyParam);
                }
            }
            AssertConstructorNotNull(CiCommand, CiConnection, CiDataAdapter, CiParameter);
            TryGetDeriveParametersMethod(ts, CommandType);
        }

        private void TryGetDeriveParametersMethod(Type[] ts, Type CommandType)
        {
            try
            {
                foreach (Type t in ts)
                {
                    if (t.Name.EndsWith("CommandBuilder"))
                    {
                        object[] os = new object[] { };
                        MiCb_DeriveParameters = t.GetMethod("DeriveParameters", ClassHelper.StaticFlag,
                            null, CallingConventions.Any, new Type[] { CommandType }, null);
                        break;
                    }
                }
            }
            catch { }
        }

        private bool IsInterfaceOf(Type t, Type it)
        {
            if (t.GetInterface(it.Name) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void AssertConstructorNotNull(params ConstructorInfo[] ts)
        {
            foreach (ConstructorInfo t in ts)
            {
                if (t == null)
                {
                    throw new ArgumentNullException("Type is null, please check info of App.config.");
                }
            }
        }

        public override IDbCommand CreateCommand()
        {
            return (IDbCommand)CiCommand.Invoke(CiParam);
        }

        public override IDbConnection CreateConnection()
        {
            return (IDbConnection)CiConnection.Invoke(CiParam);
        }

        public override IDbDataAdapter CreateDataAdapter()
        {
            return (IDbDataAdapter)CiDataAdapter.Invoke(CiParam);
        }

        public override IDbDataParameter CreateParameter()
        {
            return (IDbDataParameter)CiParameter.Invoke(CiParam);
        }

        public void DeriveParameters(IDbCommand Command)
        {
            if (MiCb_DeriveParameters != null)
            {
                MiCb_DeriveParameters.Invoke(null, new object[] { Command });
            }
            else
            {
                throw new DbEntryException("DeriveParameters not found.");
            }
        }
    }
}