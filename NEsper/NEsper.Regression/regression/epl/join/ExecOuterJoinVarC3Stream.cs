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
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.compat.logging;
using com.espertech.esper.supportregression.bean;
using com.espertech.esper.supportregression.execution;
using com.espertech.esper.supportregression.util;


using NUnit.Framework;

namespace com.espertech.esper.regression.epl.join
{
    public class ExecOuterJoinVarC3Stream : RegressionExecution {
        private static readonly string EVENT_S0 = typeof(SupportBean_S0).FullName;
        private static readonly string EVENT_S1 = typeof(SupportBean_S1).FullName;
        private static readonly string EVENT_S2 = typeof(SupportBean_S2).FullName;
    
        public override void Run(EPServiceProvider epService) {
            RunAssertionOuterInnerJoin_root_s0(epService);
            RunAssertionOuterInnerJoin_root_s1(epService);
            RunAssertionOuterInnerJoin_root_s2(epService);
        }
    
        private void RunAssertionOuterInnerJoin_root_s0(EPServiceProvider epService) {
            /// <summary>
            /// Query:
            /// s0
            /// s1 ->      <- s2
            /// </summary>
            string epl = "select * from " +
                    EVENT_S0 + "#length(1000) as s0 " +
                    " right outer join " + EVENT_S1 + "#length(1000) as s1 on s0.p00 = s1.p10 " +
                    " right outer join " + EVENT_S2 + "#length(1000) as s2 on s0.p00 = s2.p20 ";
    
            EPStatement stmt = epService.EPAdministrator.CreateEPL(epl);
            var listener = new SupportUpdateListener();
            stmt.Events += listener.Update;
    
            TryAssertion(epService, listener);
        }
    
        private void RunAssertionOuterInnerJoin_root_s1(EPServiceProvider epService) {
            /// <summary>
            /// Query:
            /// s0
            /// s1 ->      <- s2
            /// </summary>
            string epl = "select * from " +
                    EVENT_S1 + "#length(1000) as s1 " +
                    " left outer join " + EVENT_S0 + "#length(1000) as s0 on s0.p00 = s1.p10 " +
                    " right outer join " + EVENT_S2 + "#length(1000) as s2 on s0.p00 = s2.p20 ";
    
            EPStatement stmt = epService.EPAdministrator.CreateEPL(epl);
            var listener = new SupportUpdateListener();
            stmt.Events += listener.Update;
    
            TryAssertion(epService, listener);
        }
    
        private void RunAssertionOuterInnerJoin_root_s2(EPServiceProvider epService) {
            /// <summary>
            /// Query:
            /// s0
            /// s1 ->      <- s2
            /// </summary>
            string epl = "select * from " +
                    EVENT_S2 + "#length(1000) as s2 " +
                    " left outer join " + EVENT_S0 + "#length(1000) as s0 on s0.p00 = s2.p20 " +
                    " right outer join " + EVENT_S1 + "#length(1000) as s1 on s0.p00 = s1.p10 ";
    
            EPStatement stmt = epService.EPAdministrator.CreateEPL(epl);
            var listener = new SupportUpdateListener();
            stmt.Events += listener.Update;
    
            TryAssertion(epService, listener);
        }
    
        private void TryAssertion(EPServiceProvider epService, SupportUpdateListener listener) {
            // Test s0 ... s1 with 0 rows, s2 with 0 rows
            //
            object[] s0Events = SupportBean_S0.MakeS0("A", new string[]{"A-s0-1"});
            SendEvent(epService, s0Events);
            Assert.IsFalse(listener.IsInvoked);
    
            // Test s0 ... s1 with 1 rows, s2 with 0 rows
            //
            object[] s1Events = SupportBean_S1.MakeS1("B", new string[]{"B-s1-1"});
            SendEventsAndReset(epService, listener, s1Events);
    
            s0Events = SupportBean_S0.MakeS0("B", new string[]{"B-s0-1"});
            SendEvent(epService, s0Events);
            Assert.IsFalse(listener.IsInvoked);
    
            // Test s0 ... s1 with 0 rows, s2 with 1 rows
            //
            object[] s2Events = SupportBean_S2.MakeS2("C", new string[]{"C-s2-1"});
            SendEventsAndReset(epService, listener, s2Events);
    
            s0Events = SupportBean_S0.MakeS0("C", new string[]{"C-s0-1"});
            SendEvent(epService, s0Events);
            Assert.IsFalse(listener.IsInvoked);
    
            // Test s0 ... s1 with 1 rows, s2 with 1 rows
            //
            s1Events = SupportBean_S1.MakeS1("D", new string[]{"D-s1-1"});
            SendEventsAndReset(epService, listener, s1Events);
    
            s2Events = SupportBean_S2.MakeS2("D", new string[]{"D-s2-1"});
            SendEventsAndReset(epService, listener, s2Events);
    
            s0Events = SupportBean_S0.MakeS0("D", new string[]{"D-s0-1"});
            SendEvent(epService, s0Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]}}, GetAndResetNewEvents(listener));
    
            // Test s0 ... s1 with 1 rows, s2 with 2 rows
            //
            s1Events = SupportBean_S1.MakeS1("E", new string[]{"E-s1-1"});
            SendEventsAndReset(epService, listener, s1Events);
    
            s2Events = SupportBean_S2.MakeS2("E", new string[]{"E-s2-1", "E-s2-2"});
            SendEventsAndReset(epService, listener, s2Events);
    
            s0Events = SupportBean_S0.MakeS0("E", new string[]{"E-s0-1"});
            SendEvent(epService, s0Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]},
                    new object[] {s0Events[0], s1Events[0], s2Events[1]}}, GetAndResetNewEvents(listener));
    
            // Test s0 ... s1 with 2 rows, s2 with 1 rows
            //
            s1Events = SupportBean_S1.MakeS1("F", new string[]{"F-s1-1", "F-s1-2"});
            SendEventsAndReset(epService, listener, s1Events);
    
            s2Events = SupportBean_S2.MakeS2("F", new string[]{"F-s2-1"});
            SendEventsAndReset(epService, listener, s2Events);
    
            s0Events = SupportBean_S0.MakeS0("F", new string[]{"F-s0-1"});
            SendEvent(epService, s0Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]},
                    new object[] {s0Events[0], s1Events[1], s2Events[0]}}, GetAndResetNewEvents(listener));
    
            // Test s0 ... s1 with 2 rows, s2 with 2 rows
            //
            s1Events = SupportBean_S1.MakeS1("G", new string[]{"G-s1-1", "G-s1-2"});
            SendEventsAndReset(epService, listener, s1Events);
    
            s2Events = SupportBean_S2.MakeS2("G", new string[]{"G-s2-1", "G-s2-2"});
            SendEventsAndReset(epService, listener, s2Events);
    
            s0Events = SupportBean_S0.MakeS0("G", new string[]{"G-s0-1"});
            SendEvent(epService, s0Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]},
                    new object[] {s0Events[0], s1Events[1], s2Events[0]},
                    new object[] {s0Events[0], s1Events[0], s2Events[1]},
                    new object[] {s0Events[0], s1Events[1], s2Events[1]}}, GetAndResetNewEvents(listener));
    
            // Test s1 ... s0 with 0 rows, s2 with 0 rows
            //
            s1Events = SupportBean_S1.MakeS1("H", new string[]{"H-s1-1"});
            SendEvent(epService, s1Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {null, s1Events[0], null}}, GetAndResetNewEvents(listener));
    
            // Test s1 ... s0 with 1 rows, s2 with 0 rows
            //
            s0Events = SupportBean_S0.MakeS0("I", new string[]{"I-s0-1"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s1Events = SupportBean_S1.MakeS1("I", new string[]{"I-s1-1"});
            SendEvent(epService, s1Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {null, s1Events[0], null}}, GetAndResetNewEvents(listener));
            // s0 is not expected in this case since s0 requires results in s2 which didn't exist
    
            // Test s1 ... s0 with 1 rows, s2 with 1 rows
            //
            s0Events = SupportBean_S0.MakeS0("J", new string[]{"J-s0-1"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s2Events = SupportBean_S2.MakeS2("J", new string[]{"J-s2-1"});
            SendEventsAndReset(epService, listener, s2Events);
    
            s1Events = SupportBean_S1.MakeS1("J", new string[]{"J-s1-1"});
            SendEvent(epService, s1Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]}}, GetAndResetNewEvents(listener));
    
            // Test s1 ... s0 with 1 rows, s2 with 2 rows
            //
            s0Events = SupportBean_S0.MakeS0("K", new string[]{"K-s0-1"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s2Events = SupportBean_S2.MakeS2("K", new string[]{"K-s2-1", "K-s2-1"});
            SendEventsAndReset(epService, listener, s2Events);
    
            s1Events = SupportBean_S1.MakeS1("K", new string[]{"K-s1-1"});
            SendEvent(epService, s1Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]},
                    new object[] {s0Events[0], s1Events[0], s2Events[1]}}, GetAndResetNewEvents(listener));
    
    
            // Test s1 ... s0 with 2 rows, s2 with 0 rows
            //
            s0Events = SupportBean_S0.MakeS0("L", new string[]{"L-s0-1", "L-s0-2"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s1Events = SupportBean_S1.MakeS1("L", new string[]{"L-s1-1"});
            SendEvent(epService, s1Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {null, s1Events[0], null}}, GetAndResetNewEvents(listener));
            // s0 is not expected in this case since s0 requires results in s2 which didn't exist
    
            // Test s1 ... s0 with 2 rows, s2 with 1 rows
            //
            s0Events = SupportBean_S0.MakeS0("M", new string[]{"M-s0-1", "M-s0-2"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s2Events = SupportBean_S2.MakeS2("M", new string[]{"M-s2-1"});
            SendEventsAndReset(epService, listener, s2Events);
    
            s1Events = SupportBean_S1.MakeS1("M", new string[]{"M-s1-1"});
            SendEvent(epService, s1Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]},
                    new object[] {s0Events[1], s1Events[0], s2Events[0]}}, GetAndResetNewEvents(listener));
    
            // Test s1 ... s0 with 2 rows, s2 with 2 rows
            //
            s0Events = SupportBean_S0.MakeS0("N", new string[]{"N-s0-1", "N-s0-2"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s2Events = SupportBean_S2.MakeS2("N", new string[]{"N-s2-1", "N-s2-2"});
            SendEventsAndReset(epService, listener, s2Events);
    
            s1Events = SupportBean_S1.MakeS1("N", new string[]{"N-s1-1"});
            SendEvent(epService, s1Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]},
                    new object[] {s0Events[0], s1Events[0], s2Events[1]},
                    new object[] {s0Events[1], s1Events[0], s2Events[0]},
                    new object[] {s0Events[1], s1Events[0], s2Events[1]}}, GetAndResetNewEvents(listener));
    
            // Test s2 ... s0 with 0 rows, s1 with 0 rows
            //
            s2Events = SupportBean_S2.MakeS2("P", new string[]{"P-s2-1"});
            SendEvent(epService, s2Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {null, null, s2Events[0]}}, GetAndResetNewEvents(listener));
    
            // Test s2 ... s0 with 1 rows, s1 with 0 rows
            //
            s0Events = SupportBean_S0.MakeS0("Q", new string[]{"Q-s0-1"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s2Events = SupportBean_S2.MakeS2("Q", new string[]{"Q-s2-1"});
            SendEvent(epService, s2Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {null, null, s2Events[0]}}, GetAndResetNewEvents(listener));
    
            // Test s2 ... s0 with 1 rows, s1 with 1 rows
            //
            s0Events = SupportBean_S0.MakeS0("R", new string[]{"R-s0-1"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s1Events = SupportBean_S1.MakeS1("R", new string[]{"R-s1-1"});
            SendEventsAndReset(epService, listener, s1Events);
    
            s2Events = SupportBean_S2.MakeS2("R", new string[]{"R-s2-1"});
            SendEvent(epService, s2Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]}}, GetAndResetNewEvents(listener));
    
            // Test s2 ... s0 with 1 rows, s1 with 2 rows
            //
            s0Events = SupportBean_S0.MakeS0("S", new string[]{"S-s0-1"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s1Events = SupportBean_S1.MakeS1("S", new string[]{"S-s1-1", "S-s1-2"});
            SendEventsAndReset(epService, listener, s1Events);
    
            s2Events = SupportBean_S2.MakeS2("S", new string[]{"S-s2-1"});
            SendEvent(epService, s2Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]},
                    new object[] {s0Events[0], s1Events[1], s2Events[0]}}, GetAndResetNewEvents(listener));
    
            // Test s2 ... s0 with 2 rows, s1 with 0 rows
            //
            s0Events = SupportBean_S0.MakeS0("T", new string[]{"T-s0-1", "T-s0-2"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s2Events = SupportBean_S2.MakeS2("T", new string[]{"T-s2-1"});
            SendEvent(epService, s2Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {null, null, s2Events[0]}}, GetAndResetNewEvents(listener));   // no s0 events as they depend on s1
    
            // Test s2 ... s0 with 2 rows, s1 with 1 rows
            //
            s0Events = SupportBean_S0.MakeS0("U", new string[]{"U-s0-1", "U-s0-2"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s1Events = SupportBean_S1.MakeS1("U", new string[]{"U-s1-1"});
            SendEventsAndReset(epService, listener, s1Events);
    
            s2Events = SupportBean_S2.MakeS2("U", new string[]{"U-s2-1"});
            SendEvent(epService, s2Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]},
                    new object[] {s0Events[1], s1Events[0], s2Events[0]}}, GetAndResetNewEvents(listener));
    
            // Test s2 ... s0 with 2 rows, s1 with 2 rows
            //
            s0Events = SupportBean_S0.MakeS0("V", new string[]{"V-s0-1", "V-s0-2"});
            SendEventsAndReset(epService, listener, s0Events);
    
            s1Events = SupportBean_S1.MakeS1("V", new string[]{"V-s1-1", "V-s1-2"});
            SendEventsAndReset(epService, listener, s1Events);
    
            s2Events = SupportBean_S2.MakeS2("V", new string[]{"V-s2-1"});
            SendEvent(epService, s2Events);
            EPAssertionUtil.AssertSameAnyOrder(new object[][]{
                    new object[] {s0Events[0], s1Events[0], s2Events[0]},
                    new object[] {s0Events[0], s1Events[1], s2Events[0]},
                    new object[] {s0Events[1], s1Events[0], s2Events[0]},
                    new object[] {s0Events[1], s1Events[1], s2Events[0]}}, GetAndResetNewEvents(listener));
    
            epService.EPAdministrator.DestroyAllStatements();
        }
    
        private void SendEventsAndReset(EPServiceProvider epService, SupportUpdateListener listener, object[] events) {
            SendEvent(epService, events);
            listener.Reset();
        }
    
        private void SendEvent(EPServiceProvider epService, object[] events) {
            for (int i = 0; i < events.Length; i++) {
                epService.EPRuntime.SendEvent(events[i]);
            }
        }
    
        private object[][] GetAndResetNewEvents(SupportUpdateListener listener) {
            EventBean[] newEvents = listener.LastNewData;
            Assert.IsNotNull(newEvents, "no events received");
            listener.Reset();
            return ArrayHandlingUtil.GetUnderlyingEvents(newEvents, new string[]{"s0", "s1", "s2"});
        }
    }
} // end of namespace
