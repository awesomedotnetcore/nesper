///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using NUnit.Framework;

namespace com.espertech.esper.epl.expression.time
{
    [TestFixture]
	public class TestTimeAbacusMicroseconds
    {
	    private readonly TimeAbacus _abacus = TimeAbacusMicroseconds.INSTANCE;

        [Test]
	    public void TestDeltaFor() {
	        Assert.AreEqual(0, _abacus.DeltaForSecondsNumber(0));
	        Assert.AreEqual(1000000, _abacus.DeltaForSecondsNumber(1));
	        Assert.AreEqual(5000000, _abacus.DeltaForSecondsNumber(5));
	        Assert.AreEqual(123000, _abacus.DeltaForSecondsNumber(0.123));
	        Assert.AreEqual(1, _abacus.DeltaForSecondsNumber(0.000001));
	        Assert.AreEqual(10, _abacus.DeltaForSecondsNumber(0.000010));

	        Assert.AreEqual(0, _abacus.DeltaForSecondsNumber(0.0000001));
	        Assert.AreEqual(1, _abacus.DeltaForSecondsNumber(0.000000999999));
	        Assert.AreEqual(5000000, _abacus.DeltaForSecondsNumber(5.0000001));
	        Assert.AreEqual(5000001, _abacus.DeltaForSecondsNumber(5.000000999999));

	        for (int i = 1; i < 1000000; i++) {
	            double d = ((double) i) / 1000000;
	            Assert.AreEqual((long) i, _abacus.DeltaForSecondsNumber(d));
	            Assert.AreEqual((long) i, _abacus.DeltaForSecondsDouble(d));
	        }
	    }
	}
} // end of namespace
