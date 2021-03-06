/* Last modified by Mark Snyder - 2006                  */
/* Derived from works by Kenneth De Jong - 2005          */
/* Licensed under the Academic Free License version 2.1 */
using System;

namespace WorldSim.Interface
{
    /// <summary>
    /// Some useful random number generators based on class Random.
    /// </summary>
    [Serializable]
    public class ECRandom : Random
    {
        private bool m_bHaveNextGaussian;
        private double m_fNextGaussian;

        /// <summary>
        /// Constructor for the ECRandom object.
        /// </summary>
        /// <param name="nSeed">Seed to use for the random number series.</param>
        public ECRandom(int nSeed)
            : base(nSeed)
        {
        }

        /// <summary>
        /// Retrieve the next Gaussian floating point random number with
        /// a mean of 0 and and a std of 1.
        /// </summary>
        /// <returns>A value usually between -2.0 and 2.0.</returns>
        public double NextGaussian()
        {
            lock (this)
            {
                if (m_bHaveNextGaussian)
                {
                    m_bHaveNextGaussian = false;
                    return m_fNextGaussian;
                }
                else
                {
                    double v1, v2, s;
                    do
                    {
                        v1 = 2 * NextDouble() - 1; // between -1.0 and 1.0
                        v2 = 2 * NextDouble() - 1; // between -1.0 and 1.0
                        s = v1 * v1 + v2 * v2;
                    } while (s >= 1 || s == 0);
                    double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
                    m_fNextGaussian = v2 * multiplier;
                    m_bHaveNextGaussian = true;
                    return v1 * multiplier;
                }
            }
        }
    }
}