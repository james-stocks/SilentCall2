using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace Amadues
{

    public class Stat : IComparable
    {
        public String name;
        public double value;
        
        public Stat()
        {
            name = "STAT_UNDEFINED";
            value = 0;
        }
        public Stat(String aName)
        {
            name = aName;
            value = 0;
        }
        public Stat(String aName, double aValue)
        {
            name = aName;
            value = aValue;
        }

        int IComparable.CompareTo(object obj)
        {
            Stat s = (Stat)obj;
            return String.Compare(this.name, s.name);
        }
    }

    //[Serializable()]
    public class Stats
    {
        public const double STAT_MAX = 9999999999999999;
        public List<Stat> statList;
        public Stats()
        {
            statList = new List<Stat>();
        }

        public void IncrementStat(String aName)
        {
            if (statList.Count == 0)
            {
                statList.Add(new Stat(aName, 1));
            }
            else
            {
                bool found = false;
                for (int i = 0; i < statList.Count; i++)
                {
                    if (statList[i].name == aName)
                    {
                        found = true;
                        if (statList[i].value + 1 <= STAT_MAX) statList[i].value++;
                        break;
                    }
                }
                if (!found)
                {
                    statList.Add(new Stat(aName, 1));
                }
            }
        }

        public void DecrementStat(String aName)
        {
            if (statList.Count > 0)
            {
                for (int i = 0; i < statList.Count; i++)
                {
                    if (statList[i].name == aName)
                    {
                        if (statList[i].value > 0) statList[i].value -= 1;
                    }
                }
            }
        }

        public void IncreaseStat(String aName, double aValue)
        {
            if (statList.Count == 0)
            {
                statList.Add(new Stat(aName, aValue));
            }
            else
            {
                bool found = false;
                for (int i = 0; i < statList.Count; i++)
                {
                    if (statList[i].name == aName)
                    {
                        found = true;
                        statList[i].value = Math.Min(statList[i].value + aValue, STAT_MAX);
                        break;
                    }
                }
                if (!found)
                {
                    statList.Add(new Stat(aName, aValue));
                }
            }
        }

        public void SetStat(String aName, double aValue)
        {
            if (statList.Count == 0)
            {
                statList.Add(new Stat(aName, aValue));
            }
            else
            {
                bool found = false;
                for (int i = 0; i < statList.Count; i++)
                {
                    if (statList[i].name == aName)
                    {
                        found = true;
                        statList[i].value = aValue;
                        break;
                    }
                }
                if (!found)
                {
                    statList.Add(new Stat(aName, aValue));
                }
            }
        }

        public double GetStatValue(String aName)
        {
            if (statList.Count == 0) return 0;
            for (int i = 0; i < statList.Count; i++)
            {
                if (statList[i].name == aName)
                {
                    return statList[i].value;
                }
            }
            return 0;
        }

        public void AddStats(Stats otherStats)
        {
            if (statList.Count == 0)
            {
                statList = otherStats.statList;
            }
            else
            {
                if (otherStats.statList.Count > 0)
                {
                    for (int i = 0; i < otherStats.statList.Count; i++)
                    {
                        bool found = false;
                        for (int j = 0; j < statList.Count; j++)
                        {
                            if (statList[j].name == otherStats.statList[i].name)
                            {
                                found = true;
                                statList[j].value = Math.Min(statList[j].value + otherStats.statList[i].value, STAT_MAX);
                                break;
                            } 
                        }
                        if (!found)
                        {
                            SetStat(otherStats.statList[i].name, otherStats.statList[i].value);
                        }
                    }
                }
            }
        }

        public void Sort()
        {
            statList.Sort();
        }
    }
}