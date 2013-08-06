using System;
using System.Collections.Generic;
using System.Text;

namespace WorldSim.Interface
{
    public class RouletteWheel<theType>
    {
        private System.Collections.ArrayList m_list;
        private static ECRandom m_rand;

        private class RWElement
        {
            // properties
            private RouletteWheel<theType> m_parent;
            public RouletteWheel<theType> Parent { get { return m_parent; } }
            private double m_dValue;
            public double Value { get { return m_dValue; } }
            private theType m_objElement;
            public theType Element { get { return m_objElement; } }

            // constructor
            public RWElement(RouletteWheel<theType> parent, double dValue, theType objElement)
            {
                m_parent = parent;
                m_dValue = dValue;
                m_objElement = objElement;
            }
        }

        /// <summary> Standardconstructor, creates an empty Instance.</summary>
        public RouletteWheel(ECRandom rand)
        {
            m_list = new System.Collections.ArrayList(100);
            m_rand = rand;
        }

        /// <summary> add is for adding an Object to the instances List of Objects for choice.</summary>
        /// <param name="value">absolute Weight of the Object
        /// </param>
        /// <param name="anObject">Object to add
        /// </param>
        public virtual void Add(double dValue, theType anObject)
        {
            if (double.IsNaN(dValue) || dValue < 0)
                throw new ApplicationException("Bad value added to Roulettewheel, " + dValue);
            RWElement e = new RWElement(this, dValue, anObject);
            m_list.Add(e);
        }

        /// <summary> remove removes an object from the instances internal list of objects.</summary>
        /// <param name="AnObject">the object to remove
        /// </param>
        public virtual void Remove(theType anObject)
        {
            while(m_list.Contains(anObject))
                m_list.Remove(anObject);
            m_list.TrimToSize();
        }

        /// <summary> sum_values calculates the sum of absolute wheigths of elements contained
        /// in the instances list.
        /// </summary>
        /// <returns> sum of the weights
        /// </returns>
        public virtual double Sum
        {
            get
            {
                double sum = 0;
                foreach (RWElement e in m_list)
                    sum += e.Value;
                return sum;
            }
        }

        /// <summary> choice performs an roulettewheel-choice with the elements of the
        /// instances list.
        /// </summary>
        /// <returns> chosen Object
        /// </returns>
        public virtual theType Choice
        {
            get
            {
                double sum = 0;
                double thresh = m_rand.NextDouble();
                if (thresh < 0)
                    thresh = -thresh;
                thresh = thresh * Sum;
                foreach (RWElement e in m_list)
                {
                    if ((sum += e.Value) >= thresh)
                        return e.Element;
                }
                //return null;
                throw new ApplicationException("RouletteWheel: Could not find a suitable choice and one should have been found.");
            }
        }

        /// <summary> removes all entries from Roulettewheel.</summary>
        public virtual void Clear()
        {
            m_list.Clear();
        }
    }
}
