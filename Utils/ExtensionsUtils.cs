using System.Collections.Generic;
using Server.Shared.State;
namespace Utils;
public static class ListExtensions
{
    // This method deletes all values of a given enum from the list
    public static void DeleteAll(this List<Role> list, Role value)
    {
        // Loop through the list and remove the matching values
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] == value)
            {
                list.RemoveAt(i);
            }
        }
    }
}