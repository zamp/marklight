using System;
using MarkLight;

namespace Marklight.Themes
{
    /// <summary>
    /// Tracks the specificity score of a style selector.
    /// </summary>
    public struct StyleSpecificity : IComparable<StyleSpecificity>
    {
        /// <summary>
        /// 3 element array of specificity scores where higher values in lower index elements 
        /// indicate higher specificity.
        /// 
        /// Index 0 indicates the number of element ID's specified in the style selector.
        /// Index 1 indicates the number of classes specified in the style selector.
        /// Index 2 indicates the number of elements specified in the style selector.
        /// </summary>
        public readonly int[] Scores;

        /// <summary>
        /// Determine if the struct is valid (Not created with empty constructor)
        /// </summary>
        public bool IsValid
        {
            get { return Scores != null; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="scores">The specificity scores.</param>
        public StyleSpecificity(int[] scores)
        {
            Scores = scores;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="style">The StyleSelector instance to score for specificity.</param>
        public StyleSpecificity(StyleSelector style) : this(style.StyleClass)
        {
            if (style.SelectorType.HasFlag(StyleSelectorType.Element))
                Scores[2]++;

            if (style.SelectorType.HasFlag(StyleSelectorType.Id))
                Scores[0]++;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cl">The StyleClass instance to score for specificity.</param>
        public StyleSpecificity(StyleClass cl)
        {
            Scores = new int[3];

            if (cl.IsSet)
            {
                foreach (var name in cl.ClassNames)
                {
                    for (var i = 0; i < name.Length; i++)
                    {
                        var ch = name[i];

                        if (i == 0 && ch != '#' && ch != '.')
                        {
                            Scores[2]++;
                            continue;
                        }

                        switch (ch)
                        {
                            case '#':
                                Scores[0]++;
                                break;
                            case '.':
                            case '[':
                            case ':':
                                Scores[1]++;
                                break;
                        }
                    }
                }
            }
        }

        public int CompareTo(StyleSpecificity other)
        {
            if (other.Scores == null || Scores == null)
                return 0;

            for (var i = 0; i < 3; i++)
            {
                var result = CompareIndex(other, i);
                if (result != 0)
                    return result;
            }

            return 0;
        }

        private int CompareIndex(StyleSpecificity other, int index)
        {
            if (Scores[index] < other.Scores[index])
                return -1;

            return Scores[index] > other.Scores[index] ? 1 : 0;
        }
        
        public static StyleSpecificity operator +(StyleSpecificity spec1, StyleSpecificity spec2)
        {
            var scores = new int[3];
            for (var i = 0; i < 3; i++)
            {
                var s1 = spec1.IsValid ? spec1.Scores[i] : 0;
                var s2 = spec2.IsValid ? spec2.Scores[i] : 0;
                scores[i] = s1 + s2;
            }
            return new StyleSpecificity(scores);
        }
    }
}