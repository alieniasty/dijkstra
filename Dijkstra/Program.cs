namespace Dijkstra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        private static Dictionary<string, Dictionary<string, int>> _graphToLookup =
            new()
            {
                { "a", new Dictionary<string, int> { { "b", 7 }, { "c", 12 } } }, // a is directed towards b and c and the cost is 7 and 12
                { "b", new Dictionary<string, int> { { "c", 2 }, { "d", 9 } } }, // and so on..
                { "c", new Dictionary<string, int> { { "e", 10 }} },
                { "d", new Dictionary<string, int> { { "f", 1 }} },
                { "e", new Dictionary<string, int> { { "d", 4 }, { "f", 5 }} },
                { "f", new Dictionary<string, int> () } // end of graph
            };

        private static List<Visitation> visitations = new();

        public static void FindPath(Dictionary<string, Dictionary<string, int>> graph)
        {
            int TotalCostOfPreviousVisitation(KeyValuePair<string, int> child)
            {
                return visitations.First(v => v.Is.Equals(child.Key)).Cost;
            }

            int TotalCostOfCurrentVisitation(KeyValuePair<string, int> child, KeyValuePair<string, Dictionary<string, int>> node)
            {
                return child.Value + visitations.Single(v => v.Is.Equals(node.Key)).Cost;
            }

            bool VisitationHappenedAlready(KeyValuePair<string, int> child)
            {
                return visitations.Select(v => v.Is).ToList().Contains(child.Key);
            }

            foreach (var node in graph)
            {
                foreach (var child in node.Value)
                {
                    if (VisitationHappenedAlready(child))
                    {
                        if (TotalCostOfPreviousVisitation(child) <= TotalCostOfCurrentVisitation(child, node)) continue; // Don't bother. The last checked node had a better cost with its parent.
                        visitations.RemoveAll(v => v.Is.Equals(child.Key));
                        visitations.Add(new Visitation
                        {
                            Is = child.Key,
                            From = node.Key,
                            Cost = TotalCostOfCurrentVisitation(child, node)
                        });
                    }
                    else
                    {
                        if (visitations.Count < node.Value.Count) // Special case for graph entry
                        {
                            visitations.Add(new Visitation
                            {
                                Is = child.Key,
                                From = node.Key,
                                Cost = child.Value
                                // The cost of getting here is the current cost. Not the cost of parent because if already confirmed there are no parents.
                            });
                        }
                        else
                        {
                            visitations.Add(new Visitation
                            {
                                Is = child.Key,
                                From = node.Key,
                                Cost = TotalCostOfCurrentVisitation(child, node)
                                // The cost of getting here is the current cost + cost of getting to parent. Unless there is no parent then just cost of getting here.
                            });
                        }
                    }
                }
            }

            foreach (var visitation in visitations)
            {
                Console.WriteLine($"{visitation.From} -> {visitation.Is} -> {visitation.Cost}");
            }
            
            Console.WriteLine($"Smallest cost is: {visitations.Last().Cost}");

            visitations.Reverse();
            var lastCheckpoint = visitations.First();
            var shortestPath = $"{lastCheckpoint.Is} ->";
            
            foreach (var visitation in visitations)
            {
                if (visitation.Is.Equals(lastCheckpoint.From))
                {
                    shortestPath += $" {visitation.Is} ->";
                    lastCheckpoint = visitation;
                }
            }
            
            Console.WriteLine("Shortest path is: " + shortestPath + $" {visitations.Last().From}");
            Console.ReadLine();
        }


        static void Main(string[] args)
        {
            FindPath(_graphToLookup);
        }
    }

    class Visitation
    {
        public string Is { get; set; }
        public string From { get; set; }
        public int Cost { get; set; }
    }
}
