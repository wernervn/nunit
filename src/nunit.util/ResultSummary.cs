﻿// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Xml;

namespace NUnit.Util
{
    /// <summary>
    /// Summary description for ResultSummary.
    /// </summary>
    public class ResultSummary
    {
        private int resultCount = 0;
        private int testsRun = 0;
        private int failureCount = 0;
        private int errorCount = 0;
        private int successCount = 0;
        private int inconclusiveCount = 0;
        private int skipCount = 0;
        private int ignoreCount = 0;
        private int notRunnable = 0;

        private double time = 0.0d;
        private string name;

        public ResultSummary() { }

        public ResultSummary(XmlNode result)
        {
            if (result.Name != "test-run")
                throw new InvalidOperationException("Expected <test-run> as top-level element but was <" + result.Name + ">");

            this.name = GetAttribute(result, "name");
            //this.time = GetAttribute(topNode, "time", 0.0);
            //this.time = double.Parse(result.GetAttribute("time"), System.Globalization.CultureInfo.InvariantCulture);

            Summarize(result);
        }

        private void Summarize(XmlNode node)
        {
            switch (node.Name)
            {
                case "test-case":
                    resultCount++;

                    string outcome = GetAttribute(node, "result");
                    string label = GetAttribute(node, "label");
                    if (label != null)
                        outcome = label;

                    switch (outcome)
                    {
                        case "Passed":
                            successCount++;
                            testsRun++;
                            break;
                        case "Failed":
                            failureCount++;
                            testsRun++;
                            break;
                        case "Error":
                        case "Cancelled":
                            errorCount++;
                            testsRun++;
                            break;
                        case "Inconclusive":
                            inconclusiveCount++;
                            testsRun++;
                            break;
                        case "NotRunnable":
                            notRunnable++;
                            //errorCount++;
                            break;
                        case "Ignored":
                            ignoreCount++;
                            break;
                        case "Skipped":
                        default:
                            skipCount++;
                            break;
                    }
                    break;

                //case "test-suite":
                //case "test-fixture":
                //case "method-group":
                default:
                    foreach (XmlNode childResult in node.ChildNodes)
                        Summarize(childResult);
                    break;
            }
        }

        public string Name
        {
            get { return name; }
        }

        public bool Success
        {
            get { return failureCount == 0; }
        }

        /// <summary>
        /// Returns the number of test cases for which results
        /// have been summarized. Any tests excluded by use of
        /// Category or Explicit attributes are not counted.
        /// </summary>
        public int ResultCount
        {
            get { return resultCount; }
        }

        /// <summary>
        /// Returns the number of test cases actually run, which
        /// is the same as ResultCount, less any Skipped, Ignored
        /// or NonRunnable tests.
        /// </summary>
        public int TestsRun
        {
            get { return testsRun; }
        }

        /// <summary>
        /// Returns the number of tests that passed
        /// </summary>
        public int Passed
        {
            get { return successCount; }
        }

        /// <summary>
        /// Returns the number of test cases that had an error.
        /// </summary>
        public int Errors
        {
            get { return errorCount; }
        }

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Failures
        {
            get { return failureCount; }
        }

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Inconclusive
        {
            get { return inconclusiveCount; }
        }

        /// <summary>
        /// Returns the number of test cases that were not runnable
        /// due to errors in the signature of the class or method.
        /// Such tests are also counted as Errors.
        /// </summary>
        public int NotRunnable
        {
            get { return notRunnable; }
        }

        /// <summary>
        /// Returns the number of test cases that were skipped.
        /// </summary>
        public int Skipped
        {
            get { return skipCount; }
        }

        public int Ignored
        {
            get { return ignoreCount; }
        }

        public double Time
        {
            get { return time; }
        }

        public int TestsNotRun
        {
            get { return skipCount + ignoreCount + notRunnable; }
        }

        public int ErrorsAndFailures
        {
            get { return errorCount + failureCount; }
        }

        #region Helper Methods

        public static string GetAttribute(XmlNode result, string name)
        {
            var attr = result.Attributes[name];

            if (attr == null)
                return null;

            return attr.Value;
        }

        public static double GetAttribute(XmlNode result, string name, double defaultValue)
        {
            var attr = result.Attributes[name];

            if (attr == null)
                return defaultValue;

            return double.Parse(attr.Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
