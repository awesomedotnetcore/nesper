///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using NUnit.Framework;

namespace com.espertech.esper.supportregression.rowrecog
{
    public class SupportTestCaseItem {
        private string testdata;
        private string[] expected;
    
        public SupportTestCaseItem(string testdata, string[] expected) {
            this.testdata = testdata;
            this.expected = expected;
        }
    
        public string GetTestdata() {
            return testdata;
        }
    
        public string[] GetExpected() {
            return expected;
        }
    }
} // end of namespace