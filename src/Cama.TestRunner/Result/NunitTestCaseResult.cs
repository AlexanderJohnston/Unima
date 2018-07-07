﻿using System;

namespace Testura.Module.TestRunner.Result
{
    [Serializable]
    public class NUnitTestCaseResult
    {
        /// <summary>
        /// Gets or sets the full name of this test
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the short name of this test
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets if this test was a success
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error text of this test
        /// </summary>
        public string InnerText { get; set; }
    }
}
