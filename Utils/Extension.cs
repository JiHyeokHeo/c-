using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class Extension
    {
        public static void Swap<T>(this List<T> list, T a, T b)
        {
            int aIdx = -1;
            int bIdx = -1;

            for (int i = 0; i < list.Count; i++)
            {
                if (Equals(list[i], a))
                    aIdx = i;

                if (Equals(list[i], b))
                    bIdx = i;
            }

            if (aIdx != -1 && bIdx != -1)
            {
                T tmp = list[aIdx];
                list[aIdx] = list[bIdx];
                list[bIdx] = tmp;
            }
            else
            {
                Debug.Log("Swap 실패");
            }
        }

        public static void Swap<T>(this List<List<T>> list, T a, T b)
        {
            int rowA = -1; 
            int colA = -1;
            int rowB = -1;
            int colB = -1;

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count; j++)
                {
                    if (list[i][j] == null)
                        continue;

                    if (Equals(list[i][j], a))
                    {
                        rowA = i;
                        colA = j;
                    }
                    if (Equals(list[i][j], b))
                    {
                        rowB = i;
                        colB = j;
                    }
                }
            }

            if (rowA != -1 && rowB != -1)
            {
                T tmp = list[rowA][colA];
                list[rowA][colA] = list[rowB][colB];
                list[rowB][colB] = tmp;
            }
            else
            {
                Debug.Log("Swap 오류");
            }
        }

        public static void Swap<T, T1>(this List<List<Tuple<T, T1>>> list, T a, T b)
        {
            int rowA = -1;
            int colA = -1;
            int rowB = -1;
            int colB = -1;

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count; j++)
                {
                    if (list[i][j] == null)
                        continue;

                    if (Equals(list[i][j].Item1, a))
                    {
                        rowA = i;
                        colA = j;
                    }
                    if (Equals(list[i][j].Item1, b))
                    {
                        rowB = i;
                        colB = j;
                    }
                }
            }

            if (rowA != -1 && rowB != -1)
            {
                Tuple<T,T1> tmp = list[rowA][colA];
                list[rowA][colA] = list[rowB][colB];
                list[rowB][colB] = tmp;
            }
            else
            {
                Debug.Log("Swap 오류");
            }
        }

        public static T Find<T>(this List<List<T>> list, T a)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count; j ++)
                {
                    if (a.Equals(list[i][j]))
                        return list[i][j];
                }
            }

            return default(T); 
        }

        public static Tuple<int, int> FindIndexByTuple<T>(this List<List<T>> list, T a)
        {
            int tempA = -1;
            int tempB = -1;

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count; j++)
                {
                    if (a.Equals(list[i][j]))
                    {
                        tempA = i;
                        tempB = j;
                    }
                }
            }

            if (tempA != -1 || tempB != -1)
                return new Tuple<int, int>(tempA, tempB);

            return null;
        }

        public static Tuple<int, int> FindIndexByTuple<T, T1>(this List<List<Tuple<T, T1>>> list, T a)
        {
            if (list == null || a == null)
            {
                Debug.LogError("리스트나 찾고 있는 오브젝트가 NULL 입니다");
                return null;
            }

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count; j++)
                {
                    if (a.Equals(list[i][j].Item1))
                    {
                        return new Tuple<int, int>(i, j);
                    }
                }
            }

            Debug.Log("찾지 못했습니다");
            return null;
        }
    }
}
