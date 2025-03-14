using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;
using Newtonsoft.Json;

namespace Tests
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class FormulasTests
    {
        private const double tolerance = 0.00001;

        [Test]
        public void ModuloEvaluation()
        {
            ExpressionEvaluator evaluator = new();
            Assert.AreEqual(0, Convert.ToSingle(evaluator.Evaluate("2 % 2")), tolerance);
            Assert.AreEqual(1, Convert.ToSingle(evaluator.Evaluate("1 % 2")), tolerance);
            Assert.AreEqual(1, Convert.ToSingle(evaluator.Evaluate("13 % 2")), tolerance);
            Assert.AreEqual(0, Convert.ToSingle(evaluator.Evaluate("-2 % 2")), tolerance);
            Assert.AreEqual(-1, Convert.ToSingle(evaluator.Evaluate("-13 % 2")), tolerance);
        }

        [Test]
        public void CompareEvaluation()
        {
            ExpressionEvaluator evaluator = new();
            Assert.AreEqual(0, Convert.ToSingle(evaluator.Evaluate("1 > 2")), tolerance);
            Assert.AreEqual(0, Convert.ToSingle(evaluator.Evaluate("2 > 2")), tolerance);
            Assert.AreEqual(1, Convert.ToSingle(evaluator.Evaluate("2 > 1")), tolerance);

            Assert.AreEqual(1, Convert.ToSingle(evaluator.Evaluate("1 < 2")), tolerance);
            Assert.AreEqual(0, Convert.ToSingle(evaluator.Evaluate("1 < 1")), tolerance);
            Assert.AreEqual(0, Convert.ToSingle(evaluator.Evaluate("1 < 0")), tolerance);

            Assert.AreEqual(0, Convert.ToSingle(evaluator.Evaluate("1 ≥ 2")), tolerance);
            Assert.AreEqual(1, Convert.ToSingle(evaluator.Evaluate("2 ≥ 2")), tolerance);
            Assert.AreEqual(1, Convert.ToSingle(evaluator.Evaluate("2 ≥ 1")), tolerance);

            Assert.AreEqual(1, Convert.ToSingle(evaluator.Evaluate("1 ≤ 2")), tolerance);
            Assert.AreEqual(1, Convert.ToSingle(evaluator.Evaluate("1 ≤ 1")), tolerance);
            Assert.AreEqual(0, Convert.ToSingle(evaluator.Evaluate("1 ≤ 0")), tolerance);

            Assert.AreEqual(1, Convert.ToSingle(evaluator.Evaluate("1 = 1")), tolerance);
            Assert.AreEqual(0, Convert.ToSingle(evaluator.Evaluate("1 = 0")), tolerance);
            Assert.AreEqual(0, Convert.ToSingle(evaluator.Evaluate("1 = 2")), tolerance);
        }

        [Test]
        public void OrderEvaluation()
        {
            ExpressionEvaluator evaluator = new();
            Assert.AreEqual(1, Convert.ToSingle(evaluator.Evaluate("1 + 1 * 0")), tolerance);
            Assert.AreEqual(1, Convert.ToSingle(evaluator.Evaluate("1 * 0 + 1")), tolerance);
            Assert.AreEqual(0, Convert.ToSingle(evaluator.Evaluate("(1 + 1) * 0")), tolerance);
        }
    }
}
