using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;

namespace ASD
{
    
    public class Lab04 : System.MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - szukanie trasy z miasta start_v do miasta end_v, startując w dniu day
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Indeks wierzchołka odpowiadającego miastu startowemu</param>
        /// <param name="end_v">Indeks wierzchołka odpowiadającego miastu docelowemu</param>
        /// <param name="day">Dzień startu (w tym dniu należy wyruszyć z miasta startowego)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        
        public (bool result, int[] route) Lab04_FindRoute(DiGraph<int> g, int start_v, int end_v, int day, int days_number)
        {
            //klucz-wierzchołek, wartość-wierzchołek, z którego dotarliśmy do tego wierzchołka
            bool[] visited = new bool[days_number * g.VertexCount];
            int i = 0;
            Stack<int> s = new Stack<int>();
            LinkedList<int> routeList = new LinkedList<int>();
            int[] parents = new int[g.VertexCount * days_number];
            DiGraph pom = new DiGraph(g.VertexCount * days_number, new DictionaryGraphRepresentation()); //graf pomocniczy, każdy wierzchołek z g jest reprezentowany przez days_number wierzchołków i odpowiada, dniu, w ktorym wyruszamy
            //przykładowo 6(1) to wierzchołek 6 w grafie g o dniu 1
            //X-nr wierzcholka
            //Y-nr dnia
            //aby dobrać się do X w wierzchołku X(Y), robimy X(Y) % g.VertexCount
            //aby dobrać się do Y w wierzchołku X(Y), robimy X(Y) / g.VertexCount
            foreach (var e in g.DFS<int>().SearchAll()) {
                
                int u = e.From + e.weight * g.VertexCount;
                int X1 = e.To;
                int Y1 = (e.weight + 1) % days_number;//w wierzchołku v jesteśmy dzień później
                int v = X1 + Y1 * g.VertexCount;
                pom.AddEdge(u, v);
            }
            foreach (var e in g.DFS<int>().SearchAll())
            {

                int u = e.From*days_number + e.weight;
                int X1 = e.To;
                int Y1 = (e.weight + 1) % days_number;//w wierzchołku v jesteśmy dzień później
                int v = X1*days_number + Y1;
                pom.AddEdge(u, v);
            }
            int myStart = start_v*days_number + (day % days_number);
            bool done = false;
            s.Push(myStart);
            
            int curr = -1;

            while (s.Count > 0) {
                curr = s.Pop();
                if (curr / days_number == end_v) { done = true; break; }
                if (!visited[curr])
                {
                    //routeList.AddLast(curr);
                    visited[curr] = true;
                    
                }
                int counter = 0;
                foreach (var v in pom.OutNeighbors(curr))
                {
                    if (!visited[v]) { parents[v] = curr; s.Push(v); counter++; }
                    
                }
                

            }
            
            if(!done)return (false, null);

            while (curr != myStart) {

                routeList.AddFirst(curr);
                curr = parents[curr];
            }
            routeList.AddFirst(myStart);
            int[] route = new int[routeList.Count];
            
            foreach (var v in routeList) {
                route[i++] = v / days_number;
            }
            
            return (true, route);
        }

        /// <summary>
        /// Etap 2 - szukanie trasy z jednego z miast z tablicy start_v do jednego z miast z tablicy end_v (startować można w dowolnym dniu)
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Tablica z indeksami wierzchołków startowych (trasę trzeba zacząć w jednym z nich)</param>
        /// <param name="end_v">Tablica z indeksami wierzchołków docelowych (trasę trzeba zakończyć w jednym z nich)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        public (bool result, int[] route) Lab04_FindRouteSets(DiGraph<int> g, int[] start_v, int[] end_v, int days_number)
        {
            
            int start_v_pom = g.VertexCount * days_number; //pomocniczy wierzchołek połączony ze wszystkimi z start_v(superźródło)
            int end_v_pom = g.VertexCount * days_number + 1; //pomocniczy wierzchołek połączony ze wszystkimi z end_v(superujście)
            int[] parents = new int[g.VertexCount * days_number + 2];
            DiGraph pom = new DiGraph(g.VertexCount * days_number + 2, new DictionaryGraphRepresentation()); //graf pomocniczy, każdy wierzchołek z g jest reprezentowany przez days_number wierzchołków i odpowiada, dniu, w ktorym wyruszamy
            //przykładowo 6(1) to wierzchołek 6 w grafie g o dniu 1
            //X-nr wierzcholka
            //Y-nr dnia
            //aby dobrać się do X w wierzchołku X(Y), robimy X(Y) % g.VertexCount
            //aby dobrać się do Y w wierzchołku X(Y), robimy X(Y) / g.VertexCount
            foreach (var e in g.DFS<int>().SearchAll()) //O(V+E)
            {
                int u = e.From + e.weight * g.VertexCount;
                int X1 = e.To;
                int Y1 = (e.weight + 1) % days_number;//w wierzchołku v jesteśmy dzień później
                int v = X1 + Y1 * g.VertexCount;
                pom.AddEdge(u, v);
            }
            for(int i = 0;i<start_v.Length;i++) {//O(start_v.Length * days_number)
                for (int j = 0; j < days_number; j++) {
                    pom.AddEdge(start_v_pom, start_v[i] + g.VertexCount * j);
                }
            }
            for (int i = 0; i < end_v.Length; i++) ////O(end_v.Length * days_number)
            {
                for (int j = 0; j < days_number; j++)
                {
                    pom.AddEdge(end_v[i] + g.VertexCount * j,end_v_pom);
                }
            }
            Stack<int> s = new Stack<int>();
            bool[] visited = new bool[days_number * g.VertexCount + 2];
            int k = 0;
            LinkedList<int> routeList = new LinkedList<int>();
            bool done = false;
            s.Push(start_v_pom);

            int curr = -1;
            while (s.Count > 0)
            {
                curr = s.Pop();
                if (curr == end_v_pom) { done = true; break; }
                if (!visited[curr])
                {
                    
                    visited[curr] = true;

                }
                int counter = 0;
                foreach (var v in pom.OutNeighbors(curr))
                {
                    if (!visited[v]) { parents[v] = curr; s.Push(v); counter++; }
                }

            }

            if (!done) return (false, null);
            while (curr != start_v_pom)
            {
                routeList.AddFirst(curr);
                curr = parents[curr];
            }
            routeList.RemoveLast(); //usuwamy sztucznie dodane superujście
            int[] route = new int[routeList.Count];

            foreach (var v in routeList)
            {
                route[k++] = v % g.VertexCount;
            }
            
            return (true, route);
        }
    }
}
