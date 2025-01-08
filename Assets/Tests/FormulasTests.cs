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
        public void ConcurrencyEvaluation()
        {
            ExpressionEvaluator evaluator = new();
            
            TextAsset formulasRaw = Resources.Load<TextAsset>("StatsConfig");
            Dictionary<string, string> formulas =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(formulasRaw.text);
            
            float DemocratsPower = 50f;
            float NationalistsPower = 55f;
            float FederationistsPower = 45f;

            float NationalistsDemocratsConcurrency = Convert.ToSingle(
                evaluator.Evaluate(formulas["NationalistsDemocratsConcurrency"],
                    new {DemocratsPower, NationalistsPower})
            );
            float NationalistsFederationistsConcurrency = Convert.ToSingle(
                evaluator.Evaluate(formulas["NationalistsFederationistsConcurrency"],
                    new {FederationistsPower, NationalistsPower})
            );
            float DemocratsFederationistsConcurrency = Convert.ToSingle(
                evaluator.Evaluate(formulas["DemocratsFederationistsConcurrency"],
                    new {DemocratsPower, FederationistsPower})
            );

            Assert.AreEqual(5f, NationalistsDemocratsConcurrency, tolerance);
            Assert.AreEqual(5f, DemocratsFederationistsConcurrency, tolerance);
            Assert.AreEqual(10f, NationalistsFederationistsConcurrency, tolerance);
            
            NationalistsPower = Convert.ToSingle(
                evaluator.Evaluate(formulas["NationalistsPower"],
                    new
                    {
                        NationalistsPower,
                        NationalistsDemocratsConcurrency,
                        NationalistsFederationistsConcurrency
                    })
            );
            DemocratsPower = Convert.ToSingle(
                evaluator.Evaluate(formulas["DemocratsPower"],
                    new
                    {
                        DemocratsPower,
                        NationalistsDemocratsConcurrency,
                        DemocratsFederationistsConcurrency
                    })
            );
            FederationistsPower = Convert.ToSingle(
                evaluator.Evaluate(formulas["FederationistsPower"],
                    new
                    {
                        FederationistsPower,
                        NationalistsFederationistsConcurrency,
                        DemocratsFederationistsConcurrency
                    })
            );
            
            Assert.AreEqual(50f, NationalistsPower, tolerance);
            Assert.AreEqual(45, DemocratsPower, tolerance);
            Assert.AreEqual(40f, FederationistsPower, tolerance);
        }
    }
}
