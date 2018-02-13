///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;
using com.espertech.esper.client.scopetest;
using com.espertech.esper.client.time;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.compat.logging;
using com.espertech.esper.supportregression.bean;
using com.espertech.esper.supportregression.execution;

using static com.espertech.esper.supportregression.util.SupportMessageAssertUtil;
// using static org.junit.Assert.*;

using NUnit.Framework;

namespace com.espertech.esper.regression.view
{
    public class ExecViewExpressionWindow : RegressionExecution {
        public override void Run(EPServiceProvider epService) {
            epService.EPAdministrator.Configuration.AddEventType<SupportBean>();
    
            RunAssertionNewestEventOldestEvent(epService);
            RunAssertionLengthWindow(epService);
            RunAssertionTimeWindow(epService);
            RunAssertionVariable(epService);
            RunAssertionDynamicTimeWindow(epService);
            RunAssertionUDFBuiltin(epService);
            RunAssertionInvalid(epService);
            RunAssertionNamedWindowDelete(epService);
            RunAssertionPrev(epService);
            RunAssertionAggregation(epService);
        }
    
        private void RunAssertionNewestEventOldestEvent(EPServiceProvider epService) {
    
            var fields = new string[]{"theString"};
            EPStatement stmt = epService.EPAdministrator.CreateEPL("select irstream * from SupportBean#Expr(newest_event.intPrimitive = oldest_event.intPrimitive)");
            var listener = new SupportUpdateListener();
            stmt.AddListener(listener);
    
            epService.EPRuntime.SendEvent(new SupportBean("E1", 1));
            EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new Object[]{"E1"});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E2", 1));
            EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new Object[]{"E2"});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}, new object[] {"E2"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E3", 2));
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields,
                    new Object[][]{new object[] {"E3"}}, new Object[][] {new object[] {"E1"}, new object[] {"E2"}});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E3"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E4", 3));
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields,
                    new Object[][]{new object[] {"E4"}}, new Object[][] {new object[] {"E3"}});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E4"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E5", 3));
            EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new Object[]{"E5"});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E4"}, new object[] {"E5"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E6", 3));
            EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new Object[]{"E6"});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E4"}, new object[] {"E5"}, new object[] {"E6"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E7", 2));
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields,
                    new Object[][]{new object[] {"E7"}}, new Object[][] {new object[] {"E4"}, new object[] {"E5"}, new object[] {"E6"}});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E7"}});
    
            stmt.Dispose();
        }
    
        private void RunAssertionLengthWindow(EPServiceProvider epService) {
            var fields = new string[]{"theString"};
            EPStatement stmt = epService.EPAdministrator.CreateEPL("select * from SupportBean#Expr(current_count <= 2)");
            var listener = new SupportUpdateListener();
            stmt.AddListener(listener);
    
            epService.EPRuntime.SendEvent(new SupportBean("E1", 1));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E2", 2));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}, new object[] {"E2"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E3", 3));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E2"}, new object[] {"E3"}});
    
            stmt.Dispose();
        }
    
        private void RunAssertionTimeWindow(EPServiceProvider epService) {
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(0));
    
            var fields = new string[]{"theString"};
            EPStatement stmt = epService.EPAdministrator.CreateEPL("select irstream * from SupportBean#Expr(oldest_timestamp > newest_timestamp - 2000)");
            var listener = new SupportUpdateListener();
            stmt.AddListener(listener);
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(1000));
            epService.EPRuntime.SendEvent(new SupportBean("E1", 1));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}});
            EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new Object[]{"E1"});
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(1500));
            epService.EPRuntime.SendEvent(new SupportBean("E2", 2));
            EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new Object[]{"E2"});
    
            epService.EPRuntime.SendEvent(new SupportBean("E3", 3));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}, new object[] {"E2"}, new object[] {"E3"}});
            EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new Object[]{"E3"});
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(2500));
            epService.EPRuntime.SendEvent(new SupportBean("E4", 4));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}, new object[] {"E2"}, new object[] {"E3"}, new object[] {"E4"}});
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(3000));
            epService.EPRuntime.SendEvent(new SupportBean("E5", 5));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E2"}, new object[] {"E3"}, new object[] {"E4"}, new object[] {"E5"}});
            EPAssertionUtil.AssertPropsPerRow(listener.LastNewData, fields, new Object[][]{new object[] {"E5"}});
            EPAssertionUtil.AssertPropsPerRow(listener.LastOldData, fields, new Object[][]{new object[] {"E1"}});
            listener.Reset();
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(3499));
            epService.EPRuntime.SendEvent(new SupportBean("E6", 6));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E2"}, new object[] {"E3"}, new object[] {"E4"}, new object[] {"E5"}, new object[] {"E6"}});
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(3500));
            epService.EPRuntime.SendEvent(new SupportBean("E7", 7));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E4"}, new object[] {"E5"}, new object[] {"E6"}, new object[] {"E7"}});
            EPAssertionUtil.AssertPropsPerRow(listener.LastNewData, fields, new Object[][]{new object[] {"E7"}});
            EPAssertionUtil.AssertPropsPerRow(listener.LastOldData, fields, new Object[][]{new object[] {"E2"}, new object[] {"E3"}});
            listener.Reset();
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(10000));
            epService.EPRuntime.SendEvent(new SupportBean("E8", 8));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E8"}});
            EPAssertionUtil.AssertPropsPerRow(listener.LastNewData, fields, new Object[][]{new object[] {"E8"}});
            EPAssertionUtil.AssertPropsPerRow(listener.LastOldData, fields, new Object[][]{new object[] {"E4"}, new object[] {"E5"}, new object[] {"E6"}, new object[] {"E7"}});
            listener.Reset();
    
            stmt.Dispose();
        }
    
        private void RunAssertionVariable(EPServiceProvider epService) {
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(0));
            epService.EPAdministrator.CreateEPL("create variable bool KEEP = true");
    
            var fields = new string[]{"theString"};
            EPStatement stmt = epService.EPAdministrator.CreateEPL("select irstream * from SupportBean#Expr(KEEP)");
            var listener = new SupportUpdateListener();
            stmt.AddListener(listener);
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(1000));
            epService.EPRuntime.SendEvent(new SupportBean("E1", 1));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}});
    
            epService.EPRuntime.SetVariableValue("KEEP", false);
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}});
    
            listener.Reset();
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(1001));
            EPAssertionUtil.AssertProps(listener.AssertOneGetOldAndReset(), fields, new Object[]{"E1"});
            Assert.IsFalse(stmt.HasFirst());
    
            epService.EPRuntime.SendEvent(new SupportBean("E2", 2));
            EPAssertionUtil.AssertProps(listener.LastNewData[0], fields, new Object[]{"E2"});
            EPAssertionUtil.AssertProps(listener.LastOldData[0], fields, new Object[]{"E2"});
            listener.Reset();
            Assert.IsFalse(stmt.HasFirst());
    
            epService.EPRuntime.SetVariableValue("KEEP", true);
    
            epService.EPRuntime.SendEvent(new SupportBean("E3", 3));
            EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new Object[]{"E3"});
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E3"}});
    
            stmt.Stop();
        }
    
        private void RunAssertionDynamicTimeWindow(EPServiceProvider epService) {
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(0));
            epService.EPAdministrator.CreateEPL("create variable long SIZE = 1000");
    
            var fields = new string[]{"theString"};
            EPStatement stmt = epService.EPAdministrator.CreateEPL("select irstream * from SupportBean#Expr(newest_timestamp - oldest_timestamp < SIZE)");
            var listener = new SupportUpdateListener();
            stmt.AddListener(listener);
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(1000));
            epService.EPRuntime.SendEvent(new SupportBean("E1", 0));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}});
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(2000));
            epService.EPRuntime.SendEvent(new SupportBean("E2", 0));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E2"}});
    
            epService.EPRuntime.SetVariableValue("SIZE", 10000);
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(5000));
            epService.EPRuntime.SendEvent(new SupportBean("E3", 0));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E2"}, new object[] {"E3"}});
    
            epService.EPRuntime.SetVariableValue("SIZE", 2000);
    
            epService.EPRuntime.SendEvent(new CurrentTimeEvent(6000));
            epService.EPRuntime.SendEvent(new SupportBean("E4", 0));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E3"}, new object[] {"E4"}});
    
            stmt.Dispose();
        }
    
        private void RunAssertionUDFBuiltin(EPServiceProvider epService) {
            epService.EPAdministrator.Configuration.AddPlugInSingleRowFunction("udf", typeof(LocalUDF).Name, "evaluateExpiryUDF");
            epService.EPAdministrator.CreateEPL("select * from SupportBean#Expr(Udf(theString, view_reference, expired_count))");
    
            LocalUDF.Result = true;
            epService.EPRuntime.SendEvent(new SupportBean("E1", 0));
            Assert.AreEqual("E1", LocalUDF.Key);
            Assert.AreEqual(0, (int) LocalUDF.ExpiryCount);
            Assert.IsNotNull(LocalUDF.Viewref);
    
            epService.EPRuntime.SendEvent(new SupportBean("E2", 0));
    
            LocalUDF.Result = false;
            epService.EPRuntime.SendEvent(new SupportBean("E3", 0));
            Assert.AreEqual("E3", LocalUDF.Key);
            Assert.AreEqual(2, (int) LocalUDF.ExpiryCount);
            Assert.IsNotNull(LocalUDF.Viewref);
    
            epService.EPAdministrator.DestroyAllStatements();
        }
    
        private void RunAssertionInvalid(EPServiceProvider epService) {
            TryInvalid(epService, "select * from SupportBean#Expr(1)",
                    "Error starting statement: Error attaching view to event stream: Invalid return value for expiry expression, expected a bool return value but received int? [select * from SupportBean#Expr(1)]");
    
            TryInvalid(epService, "select * from SupportBean#Expr((select * from SupportBean#lastevent))",
                    "Error starting statement: Error attaching view to event stream: Invalid expiry expression: Sub-select, previous or prior functions are not supported in this context [select * from SupportBean#Expr((select * from SupportBean#lastevent))]");
        }
    
        private void RunAssertionNamedWindowDelete(EPServiceProvider epService) {
            epService.EPAdministrator.Configuration.AddEventType("SupportBean_A", typeof(SupportBean_A));
    
            var fields = new string[]{"theString"};
            EPStatement stmt = epService.EPAdministrator.CreateEPL("create window NW#Expr(true) as SupportBean");
            var listener = new SupportUpdateListener();
            stmt.AddListener(listener);
    
            epService.EPAdministrator.CreateEPL("insert into NW select * from SupportBean");
    
            epService.EPRuntime.SendEvent(new SupportBean("E1", 1));
            epService.EPRuntime.SendEvent(new SupportBean("E2", 2));
            epService.EPRuntime.SendEvent(new SupportBean("E3", 3));
            listener.Reset();
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}, new object[] {"E2"}, new object[] {"E3"}});
    
            epService.EPAdministrator.CreateEPL("on SupportBean_A delete from NW where theString = id");
            epService.EPRuntime.SendEvent(new SupportBean_A("E2"));
            EPAssertionUtil.AssertPropsPerRow(stmt.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}, new object[] {"E3"}});
            EPAssertionUtil.AssertProps(listener.AssertOneGetOldAndReset(), fields, new Object[]{"E2"});
    
            epService.EPAdministrator.DestroyAllStatements();
        }
    
        private void RunAssertionPrev(EPServiceProvider epService) {
            var fields = new string[]{"val0"};
            EPStatement stmt = epService.EPAdministrator.CreateEPL("select Prev(1, theString) as val0 from SupportBean#Expr(true)");
            var listener = new SupportUpdateListener();
            stmt.AddListener(listener);
    
            epService.EPRuntime.SendEvent(new SupportBean("E1", 1));
            EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new Object[]{null});
    
            epService.EPRuntime.SendEvent(new SupportBean("E2", 2));
            EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new Object[]{"E1"});
    
            stmt.Dispose();
        }
    
        private void RunAssertionAggregation(EPServiceProvider epService) {
            // Test ungrouped
            var fields = new string[]{"theString"};
            EPStatement stmtUngrouped = epService.EPAdministrator.CreateEPL("select irstream theString from SupportBean#Expr(sum(intPrimitive) < 10)");
            var listener = new SupportUpdateListener();
            stmtUngrouped.AddListener(listener);
    
            epService.EPRuntime.SendEvent(new SupportBean("E1", 1));
            EPAssertionUtil.AssertPropsPerRow(stmtUngrouped.GetEnumerator(), fields, new Object[][]{new object[] {"E1"}});
            EPAssertionUtil.AssertProps(listener.AssertOneGetNewAndReset(), fields, new Object[]{"E1"});
    
            epService.EPRuntime.SendEvent(new SupportBean("E2", 9));
            EPAssertionUtil.AssertPropsPerRow(stmtUngrouped.GetEnumerator(), fields, new Object[][]{new object[] {"E2"}});
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E2"}}, new Object[][] {new object[] {"E1"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E3", 11));
            EPAssertionUtil.AssertPropsPerRow(stmtUngrouped.GetEnumerator(), fields, null);
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E3"}}, new Object[][] {new object[] {"E2"}, new object[] {"E3"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E4", 12));
            EPAssertionUtil.AssertPropsPerRow(stmtUngrouped.GetEnumerator(), fields, null);
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E4"}}, new Object[][] {new object[] {"E4"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E5", 1));
            EPAssertionUtil.AssertPropsPerRow(stmtUngrouped.GetEnumerator(), fields, new Object[][]{new object[] {"E5"}});
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E5"}}, null);
    
            epService.EPRuntime.SendEvent(new SupportBean("E6", 2));
            EPAssertionUtil.AssertPropsPerRow(stmtUngrouped.GetEnumerator(), fields, new Object[][]{new object[] {"E5"}, new object[] {"E6"}});
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E6"}}, null);
    
            epService.EPRuntime.SendEvent(new SupportBean("E7", 3));
            EPAssertionUtil.AssertPropsPerRow(stmtUngrouped.GetEnumerator(), fields, new Object[][]{new object[] {"E5"}, new object[] {"E6"}, new object[] {"E7"}});
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E7"}}, null);
    
            epService.EPRuntime.SendEvent(new SupportBean("E8", 6));
            EPAssertionUtil.AssertPropsPerRow(stmtUngrouped.GetEnumerator(), fields, new Object[][]{new object[] {"E7"}, new object[] {"E8"}});
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E8"}}, new Object[][] {new object[] {"E5"}, new object[] {"E6"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E9", 9));
            EPAssertionUtil.AssertPropsPerRow(stmtUngrouped.GetEnumerator(), fields, new Object[][]{new object[] {"E9"}});
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E9"}}, new Object[][] {new object[] {"E7"}, new object[] {"E8"}});
    
            stmtUngrouped.Dispose();
    
            // Test grouped
            EPStatement stmtGrouped = epService.EPAdministrator.CreateEPL("select irstream theString from SupportBean#Groupwin(intPrimitive)#Expr(sum(longPrimitive) < 10)");
            stmtGrouped.AddListener(listener);
    
            SendEvent(epService, "E1", 1, 5);
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E1"}}, null);
    
            SendEvent(epService, "E2", 2, 2);
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E2"}}, null);
    
            SendEvent(epService, "E3", 1, 3);
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E3"}}, null);
    
            SendEvent(epService, "E4", 2, 4);
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E4"}}, null);
    
            SendEvent(epService, "E5", 2, 6);
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E5"}}, new Object[][] {new object[] {"E2"}, new object[] {"E4"}});
    
            SendEvent(epService, "E6", 1, 2);
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E6"}}, new Object[][] {new object[] {"E1"}});
    
            stmtGrouped.Dispose();
    
            // Test on-delete
            epService.EPAdministrator.Configuration.AddEventType("SupportBean_A", typeof(SupportBean_A));
            EPStatement stmt = epService.EPAdministrator.CreateEPL("create window NW#Expr(sum(intPrimitive) < 10) as SupportBean");
            stmt.AddListener(listener);
            epService.EPAdministrator.CreateEPL("insert into NW select * from SupportBean");
    
            epService.EPRuntime.SendEvent(new SupportBean("E1", 1));
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E1"}}, null);
    
            epService.EPRuntime.SendEvent(new SupportBean("E2", 8));
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E2"}}, null);
    
            epService.EPAdministrator.CreateEPL("on SupportBean_A delete from NW where theString = id");
            epService.EPRuntime.SendEvent(new SupportBean_A("E2"));
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, null, new Object[][]{new object[] {"E2"}});
    
            epService.EPRuntime.SendEvent(new SupportBean("E3", 7));
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E3"}}, null);
    
            epService.EPRuntime.SendEvent(new SupportBean("E4", 2));
            EPAssertionUtil.AssertPropsPerRow(listener.GetAndResetDataListsFlattened(), fields, new Object[][]{new object[] {"E4"}}, new Object[][] {new object[] {"E1"}});
    
            stmt.Dispose();
        }
    
        private void SendEvent(EPServiceProvider epService, string theString, int intPrimitive, long longPrimitive) {
            var bean = new SupportBean(theString, intPrimitive);
            bean.LongPrimitive = longPrimitive;
            epService.EPRuntime.SendEvent(bean);
        }
    
        public class LocalUDF {
    
            private static string key;
            private static int? expiryCount;
            private static Object viewref;
            private static bool result;
    
            public static bool EvaluateExpiryUDF(string key, Object viewref, int? expiryCount) {
                LocalUDF.key = key;
                LocalUDF.viewref = viewref;
                LocalUDF.expiryCount = expiryCount;
                return result;
            }
    
            public static string GetKey() {
                return key;
            }
    
            public static int? GetExpiryCount() {
                return expiryCount;
            }
    
            public static Object GetViewref() {
                return viewref;
            }
    
            public static void SetResult(bool result) {
                LocalUDF.result = result;
            }
        }
    }
} // end of namespace