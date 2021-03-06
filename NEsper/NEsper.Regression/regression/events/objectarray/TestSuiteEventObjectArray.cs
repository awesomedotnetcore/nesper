///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using NUnit.Framework;

using System;

using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.compat.logging;
using com.espertech.esper.supportregression.execution;

namespace com.espertech.esper.regression.events.objectarray
{
    [TestFixture]
    public class TestSuiteEventObjectArray
    {
        [Test]
        public void TestExecEventObjectArray() {
            RegressionRunner.Run(new ExecEventObjectArray());
        }
    
        [Test]
        public void TestExecEventObjectArrayNestedMap() {
            RegressionRunner.Run(new ExecEventObjectArrayNestedMap());
        }
    
        [Test]
        public void TestExecEventObjectArrayInheritanceConfigInit() {
            RegressionRunner.Run(new ExecEventObjectArrayInheritanceConfigInit());
        }
    
        [Test]
        public void TestExecEventObjectArrayInheritanceConfigRuntime() {
            RegressionRunner.Run(new ExecEventObjectArrayInheritanceConfigRuntime());
        }
    
        [Test]
        public void TestExecEventObjectArrayConfiguredStatic() {
            RegressionRunner.Run(new ExecEventObjectArrayConfiguredStatic());
        }
    
        [Test]
        public void TestExecEventObjectArrayTypeUpdate() {
            RegressionRunner.Run(new ExecEventObjectArrayTypeUpdate());
        }
    
        [Test]
        public void TestExecEventObjectArrayEventNestedPono() {
            RegressionRunner.Run(new ExecEventObjectArrayEventNestedPono());
        }
    
        [Test]
        public void TestExecEventObjectArrayEventNested() {
            RegressionRunner.Run(new ExecEventObjectArrayEventNested());
        }
    }
} // end of namespace
