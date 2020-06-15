﻿using CW.Soloist.CompositionService.CompositionStrategies.UtilEnums;
using CW.Soloist.CompositionService.MusicTheory;
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
        #region Shuffle<T>()
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
        #endregion

        #region Sort()
        /// <summary>
        /// Sorts a note sequence in ascending/descending order according 
        /// to the notes' pitches. 
        /// <para> The sorting order is determined by the <paramref name="sortOrder"/> parameter. </para>
        /// <para> Hold notes and rest notes retain their original positions.
        /// The rest of the notes are sorted and reordered accordingly, before,
        /// and after the hold and rest notes.</para>
        /// </summary>
        /// <remarks> 
        /// The sorting routine might work in place, i.e., 
        /// it is possible that the original list is changed during the process. 
        /// </remarks>
        /// <param name="noteList"> The note sequence that needs to be sorted. </param>
        /// <param name="sortOrder"> The sort ordering, i.e., ascending or descending. </param>
        /// <returns> The note sequence reordered by the requested sorting order. </returns>
        public static IList<INote> Sort(this IList<INote> noteList, SortOrder sortOrder = SortOrder.Ascending)
        {
            /* optimization attempt: if list contains no rest or hold notes,
             * just sort the list as is with no further processing. */
            if (!noteList.Any(note => note.Pitch == NotePitch.RestNote 
                                   || note.Pitch == NotePitch.HoldNote))
            {
                return sortOrder == SortOrder.Ascending ?
                    noteList.OrderBy(note => note.Pitch).ToList() :
                    noteList.OrderByDescending(note => note.Pitch).ToList();
            }
            
            // list contains hold/rest notes. filter them out for sorting
            var soundedNotes = noteList
                .Where(note => note.Pitch != NotePitch.RestNote
                            && note.Pitch != NotePitch.HoldNote);

            // sort the filtered sounded notes according to sort order parameter 
            IList<INote> sortedNotes;
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                default:
                    sortedNotes = soundedNotes.OrderBy(note => note.Pitch).ToList();
                    break;
                case SortOrder.Descending:
                    sortedNotes = soundedNotes.OrderByDescending(note => note.Pitch).ToList();
                    break;
            }

            // apply the sorting order in the original note sequence 
            for (int i = 0, j = 0; i < noteList.Count - 1; i++)
            {
                // skip rest and hold notes, which retain their original position
                if (noteList[i].Pitch == NotePitch.RestNote ||
                     noteList[i].Pitch == NotePitch.HoldNote)
                    continue;

                // update current position with the appropriate note 
                noteList.RemoveAt(i);
                noteList.Insert(i, sortedNotes[j++]);
            }

            // return sorted sequence 
            return noteList;
        }
        #endregion
    }
}
