using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public class Lab06 : System.MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 i 2 - szukanie trasy w nieplynacej rzece
        /// </summary>
        /// <param name="w"> Odległość między brzegami rzeki</param>
        /// <param name="l"> Długość fragmentu rzeki </param>
        /// <param name="lilie"> Opis lilii na rzece </param>
        /// <param name="sila"> Siła żabki - maksymalny kwadrat długości jednego skoku </param>
        /// <param name="start"> Początkowa pozycja w metrach od lewej strony </param>
        /// <returns> (int total, (int x, int y)[] route) - total - suma sił koniecznych do wszystkich skoków, route -
        /// lista par opisujących skoki. Opis jednego skoku (x,y) to dystans w osi x i dystans w osi y, jaki skok pokonuje</returns>
        public (int total, (int, int)[] route) Lab06_FindRoute(int w, int l, int[,] lilie, int sila, int start)
        {
            var graph = new DiGraph<int>(w * l + 2);
            int my_start = w * l;
            int my_end = w * l + 1;
            
            for (int i = 0; i < w; i++)
                for (int j = 0; j < l; j++)
                    for (int k = 0; k < w; k++)
                        for (int p = 0; p < l; p++)
                            if (lilie[i, j] == 1 && lilie[k, p] == 1 && !(i == k && j == p) && (i - k) * (i - k) + (j - p) * (j - p) <= sila)
                                graph.AddEdge(i * l + j, k * l + p, (i - k) * (i - k) + (j - p) * (j - p));

            for (int i = 0; i < w; i++)
                for (int j = 0; j < l; j++)
                    if (lilie[i, j] == 1 && (-1 - i) * (-1 - i) + (start - j) * (start - j) <= sila)
                        graph.AddEdge(my_start, i * l + j, (-1 - i) * (-1 - i) + (start - j) * (start - j));

            for (int i = 0; i < w; i++)
                 if ((w - i) * (w - i) <= sila)
                    for (int j = 0; j < l; j++) graph.AddEdge(i * l + j, my_end, (w - i) * (w - i));

            if ((-1 - w) * (-1 - w) <= sila) graph.AddEdge(my_start, my_end, (-1 - w) * (-1 - w));
            var pom = Paths.Dijkstra<int>(graph, my_start);
            if(!pom.Reachable(my_start,my_end))
                return (0, null);

            var PathRes = pom.GetPath(my_start, my_end);
            var route = new (int, int)[PathRes.Length - 1];
            if (route.Length == 1) {
                route[0] = (w + 1, 0);
                return (pom.GetDistance(my_start, my_end), route);
            }
            route[0] = (PathRes[1] / l + 1, PathRes[1] % l - start);
            for (int i = 2; i < PathRes.Length - 1; i++) {
                route[i - 1] = (PathRes[i] / l - PathRes[i - 1] / l, PathRes[i] % l - PathRes[i - 1] % l);
            }
            route[PathRes.Length - 2] = (w - PathRes[PathRes.Length - 2] / l, 0);
            return (pom.GetDistance(my_start, my_end),route);
        }

        public void WriteLilie(int w, int l, int[,] lilie) {
            Console.WriteLine();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < l; j++)
                {
                    Console.Write(lilie[i, j] + " ");
                }
                Console.WriteLine();
            }

        }
        /// <summary>
        /// Etap 3 i 4 - szukanie trasy w nieplynacej rzece
        /// </summary>
        /// <param name="w"> Odległość między brzegami rzeki</param>
        /// <param name="l"> Długość fragmentu rzeki </param>
        /// <param name="lilie"> Opis lilii na rzece </param>
        /// <param name="sila"> Siła żabki - maksymalny kwadrat długości jednego skoku </param>
        /// <param name="start"> Początkowa pozycja w metrach od lewej strony </param>
        /// <param name="max_skok"> Maksymalna ilość skoków </param>
        /// <param name="v"> Prędkość rzeki </param>
        /// <returns> (int total, (int x, int y)[] route) - total - suma sił koniecznych do wszystkich skoków, route -
        /// lista par opisujących skoki. Opis jednego skoku (x,y) to dystans w osi x i dystans w osi y, jaki skok pokonuje</returns>
        public (int total, (int, int)[] route) Lab06_FindRouteFlowing(int w, int l, int[,] lilie, int sila, int start, int max_skok, int v)
        {
            // uzupełnić
            int vertCount = w * l + 1;//liczba wierzcholkow jednej warstwy pomocniczego grafu (nie uwazgledniajac brzegu)
            var graph = new DiGraph<int>(vertCount*l + 1);//dla kazdego z wierzcholkow (poza brzegiem) robie kopie zalezna od "czasu"
            // Każdy wierzchołek identyfikuje przez dwa indeksy i oznaczam go jako X(Y), gdzie X to numer wierzchołka (tak jak w poprzednim wariancie), a Y to czas 
            // Konstruuję graf w taki sposób, że dobieram się do:
            //X = vertex % vertCount
            //Y = vertex / vertCount
            
            int my_start = w * l;
            int my_end = vertCount * l;
            //tu bede dodawal wszystkie krawedzie na jeziorze
            for (int i = 0; i < w; i++) 
                for (int j = 0; j < l; j++)
                    for (int time1 = 0; time1 < l; time1++)
                    {
                        int u = (i * l + j) + ((time1*v)%l) * vertCount;
                        for (int k = 0; k < w; k++)
                            for (int p = 0; p < l; p++)
                                {
                                    int currX = ((j + time1 * v) % l);
                                    int time2 = time1 + 1;
                                    int nextX = ((p + time2 * v) % l);
                                    int neededPower = (i - k) * (i - k) + (currX - nextX) * (currX - nextX);
                                    int Vertex = (k * l + p) + ((time2 * v) % l) * vertCount;
                                    if (lilie[i, j] == 1 && lilie[k, p] == 1 && !(i == k && j == p) && neededPower <= sila)
                                        graph.AddEdge(u, Vertex, neededPower); 
                                }
                    }

            //tu bede dodawal wszystkie krawedzie oznaczajace zabke skaczaca w miejscu na starcie
            for (int time1 = 0; time1 < l; time1++) {
                if(my_start + (((time1 + 1) * v) % l) * vertCount != my_start + ((time1 * v) % l) * vertCount)
                graph.AddEdge(my_start + ((time1 * v) % l) * vertCount, my_start + (((time1 + 1) * v) % l) * vertCount,0);
            }

            for (int i = 0; i < w; i++)
                for (int j = 0; j < l; j++)
                    for (int time1 = 0; time1 < l; time1++)
                    {
                        int myStartVertex = my_start + ((time1 * v) % l) * vertCount;
                        int currX = start;
                        int time2 = time1 + 1;
                        int nextX = ((j + time2 * v) % l);
                        int neededPower = (-1 - i) * (-1 - i) + (currX - nextX) * (currX - nextX);
                        int Vertex = (i * l + j) + ((time2 * v) % l) * vertCount;
                        if (lilie[i, j] == 1 && neededPower <= sila)
                            graph.AddEdge(myStartVertex, Vertex, neededPower);
                    }

            for (int i = 0; i < w; i++)
                if ((w - i) * (w - i) <= sila)
                    for (int j = 0; j < l; j++)
                        for (int time1 = 0; time1 < l; time1++) {
                            int u = (i * l + j) + ((time1 * v) % l) * vertCount;
                            graph.AddEdge(u, my_end, (w - i) * (w - i));
                        }

            if ((-1 - w) * (-1 - w) <= sila) graph.AddEdge(my_start, my_end, (-1 - w) * (-1 - w));

            int[,] distances = new int[vertCount*l+1,max_skok+1];
            int[,] prev = new int[vertCount*l+1, max_skok+1]; 
            for (int i = 0; i < vertCount*l+1; i++)
                for(int j = 0;j<=max_skok;j++) distances[i,j] = int.MaxValue - 10000;
            for(int i = 0;i<=max_skok;i++)distances[my_start,i] = 0;
            for (int i = 1; i <= max_skok; i++) {
                foreach (var e in graph.BFS().SearchAll()) {

                    if (distances[e.To, i] > distances[e.From, i - 1] + e.weight)
                    {
                        
                        distances[e.To, i] = distances[e.From, i - 1] + e.weight;
                        prev[e.To, i] = e.From;
                    }
                    
                }
            }
            //WriteLilie(w, l, lilie);
            //Console.WriteLine("vertCount is " + vertCount);
            //foreach (var e in graph.BFS<int>().SearchAll()) {
              //  Console.WriteLine("Edge from : " + e.From + " Edge to : " + e.To + " weight: " + e.weight);
            //}
            int minDist = int.MaxValue - 10000;
            int minLength = int.MaxValue - 10000;
            for (int i = 0; i <= max_skok; i++) if (distances[my_end, i] < minDist) { minDist = distances[my_end, i]; minLength = i; }
            if (minDist == int.MaxValue - 10000) return (0, null);
            var route = new (int, int)[minLength];

            int s = minLength;
            int ActualVert = prev[my_end, s] % vertCount;
            route[s-1] = (w - ActualVert / l, 0);
            int pointer = prev[my_end, s];
            int pomIt = s;
            while (pointer % vertCount != my_start) pointer = prev[pointer,--pomIt];
            for (int i = 0; i < pomIt - 1; i++) route[i] = (0, 0);
            pointer = prev[my_end, s];
            int fixMod = 100000 * l;
            for (int iter = s - 2; iter > pomIt-1; iter--) {
                ActualVert = prev[pointer, iter+1] % vertCount;
                int currTime = iter+1; 
                route[iter] = (pointer%vertCount / l - ActualVert / l, (pointer % vertCount + currTime * v + fixMod) % l - (ActualVert + (currTime-1) * v + fixMod) % l);
                
                pointer = prev[pointer, iter+1];
                
            }
            //if()
            route[pomIt-1] = (pointer%vertCount / l + 1, (pointer % vertCount + v * pomIt) % l - start);
            //for (int i = 0; i < minLength; i++) Console.WriteLine(route[i]);
            return (minDist, route);

        }
    }
}
