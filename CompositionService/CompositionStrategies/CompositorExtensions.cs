using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW.Soloist.CompositionService.CompositionStrategies
{
    /// <summary>
    /// Extension methods for compositors usage. 
    /// </summary>
    internal static class CompositorExtensions
    {
        /// <summary>
        /// Shuffles a list of elements of type <typeparamref name="T"/> in place 
        /// using <a href="https://bit.ly/2ArSevh">Durstenfeld's implementation</a>  
        ///  of the <a href="https://bit.ly/2B5jhfQ"> Fisher-Yates shuffle algorithm.</a>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            // initialization 
            Random randomizer = new Random();
            int j; // random index 
            T temp; // type of elemens in the list 

            for (int i = 0; i < list.Count - 1; i++)
            {
                // set a random position from the dynamic decreasing range 
                j = randomizer.Next(i, list.Count);

                // swap current element's position with the random generated position 
                temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
